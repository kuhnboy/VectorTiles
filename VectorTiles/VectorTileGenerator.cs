using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Google.Protobuf;
using NetTopologySuite.Clipper;
using NetTopologySuite.Geometries;
using VectorTiles.VectorTile;

namespace VectorTiles
{
	internal class VectorTileGenerator : IVectorTileGenerator
	{
		// Each time we touch stuff it slows things down. At higher altitude zoom levels,
		// there's a low chance we will violate any coordinate system extents because we are dealing
		// with field data and not state or country polygons. We can use this to just not clip at higher zooms.
		private const int MinimumClippingZoom = 12;

		private readonly GeometryEncoder geometryEncoder = new GeometryEncoder();
		private readonly AttributeEncoder attributeEncoder = new AttributeEncoder();
		private readonly CoordinateTransformer coordinateTransformer = new CoordinateTransformer();
		private readonly PolygonClipper polygonClipper = new PolygonClipper();

		public async Task<Stream> GenerateTile<T>(IEnumerable<VectorLayer<T>> layers, int x, int y, int z) where T : class
		{
			var tile = new Tile();

			foreach (var layer in layers)
			{
				tile.Layers.Add(await BuildLayer(layer, x, y, z));
			}

			return WriteOutput(tile);
		}

		private async Task<Tile.Types.Layer> BuildLayer<T>(VectorLayer<T> layer, int x, int y, int z) where T : class
		{
			var tileLayer = new Tile.Types.Layer
			{
				Name = layer.Name,
				Version = 2
			};

			var features = new List<Tile.Types.Feature>();

			var allKeys = new Dictionary<string, uint>();
			var allValues = new Dictionary<object, uint>();

			// Set the bounds that we will clip geometries to as the local tile coordinate system is 0-4096
			// and has an overflow at about 16000. If we overflow the tile won't draw correctly therefore we
			// clip to just outside the tile. 
			var tileBounds = GetTileClipBounds(x, y, z);

			await foreach (var item in layer.Items)
			{
				var geometry = layer.GeoProperty(item);
				if (!(geometry is Polygon || geometry is MultiPolygon || geometry is Point || geometry is MultiPoint))
				{
					// Ignore things that aren't polygons for now.
					continue;
				}

				var clippedGeometry = z < MinimumClippingZoom ? geometry : ClipToTile(geometry, tileBounds);
				if (clippedGeometry != null)
				{
					var transformed = coordinateTransformer.GetTileCoordinates(clippedGeometry, x, y, z);

					var (geom, geomType) = geometryEncoder.EncodeGeometry(transformed);

					var tags = attributeEncoder.EncodeAttributes(allKeys, allValues, layer.Attributes(item));

					var feature = new Tile.Types.Feature { Type = geomType };
					feature.Geometry.AddRange(geom);
					feature.Tags.AddRange(tags);

					features.Add(feature);
				}
			}

			var keys = new string[allKeys.Count];
			foreach (var allKey in allKeys)
			{
				keys[allKey.Value] = allKey.Key;
			}

			tileLayer.Keys.AddRange(keys);

			var values = new Tile.Types.Value[allValues.Count];
			foreach (var allValue in allValues)
			{
				values[allValue.Value] = attributeEncoder.EmitValue(allValue.Key);
			}

			tileLayer.Values.AddRange(values);

			tileLayer.Features.AddRange(features);

			return tileLayer;
		}

		private Stream WriteOutput(Tile tile)
		{
			var memoryStream = new MemoryStream();
			using var codedOutputStream = new CodedOutputStream(memoryStream, true);

			tile.WriteTo(codedOutputStream);

			codedOutputStream.Flush();
			memoryStream.Seek(0, SeekOrigin.Begin);

			return memoryStream;
		}

		private Polygon GetTileClipBounds(int x, int y, int z)
		{
			var tileBounds = coordinateTransformer.GetTileBounds(x, y, z);

			// If we clip *to* the tile we will see things like polygon outlines
			// at the edge of each tile which doesn't make a contiguous shape when viewed with adjacent tiles.
			tileBounds.ExpandBy(tileBounds.Width * .1, tileBounds.Height * .1);

			return new Polygon(new LinearRing(new[]
			{
				new Coordinate(tileBounds.MinX, tileBounds.MinY),
				new Coordinate(tileBounds.MaxX, tileBounds.MinY),
				new Coordinate(tileBounds.MaxX, tileBounds.MaxY),
				new Coordinate(tileBounds.MinX, tileBounds.MaxY),
				new Coordinate(tileBounds.MinX, tileBounds.MinY)
			}));
		}

		private Geometry ClipToTile(Geometry geometry, Polygon tileBounds)
		{
			if (geometry is Point p)
			{
				return tileBounds.Contains(p) ? geometry : null;
			}

			if (geometry is MultiPoint mp)
			{
				return new MultiPoint(mp.Geometries.Where(tileBounds.Contains).Cast<Point>().ToArray());
			}

			return polygonClipper.ClipPolygon(geometry, tileBounds);
		}
	}
}
