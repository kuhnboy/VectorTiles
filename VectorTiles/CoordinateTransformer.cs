using System;
using System.Collections.Generic;
using System.Linq;
using NetTopologySuite.Geometries;
using ProjNet.CoordinateSystems;
using ProjNet.CoordinateSystems.Transformations;

namespace VectorTiles
{
	internal class CoordinateTransformer
	{
		private const double EarthRadiusMeters = 6378137;
		private const double EarthCircum = 2.0 * Math.PI * EarthRadiusMeters;
		private const double EarthHalfCircum = EarthCircum / 2.0;
		private const int TileSize = 256;
		private const int VectorTileExtent = 4096;

		private static readonly CoordinateSystemFactory Csf = new();
		private static readonly CoordinateTransformationFactory Ctf = new();
		private static readonly CoordinateSystem Epsg4326 = Csf.CreateFromWkt(
			"GEOGCS[\"WGS 84\",DATUM[\"WGS_1984\",SPHEROID[\"WGS 84\",6378137,298.257223563,AUTHORITY[\"EPSG\",\"7030\"]],AUTHORITY[\"EPSG\",\"6326\"]],PRIMEM[\"Greenwich\",0,AUTHORITY[\"EPSG\",\"8901\"]],UNIT[\"degree\",0.0174532925199433,AUTHORITY[\"EPSG\",\"9122\"]],AUTHORITY[\"EPSG\",\"4326\"]]");

		private static readonly ICoordinateTransformation MetersToLatLon = Ctf.CreateFromCoordinateSystems(ProjectedCoordinateSystem.WebMercator, Epsg4326);
		private static readonly ICoordinateTransformation LatLonToMeters = Ctf.CreateFromCoordinateSystems(Epsg4326, ProjectedCoordinateSystem.WebMercator);

		public Envelope GetTileBounds(int x, int y, int z)
		{
			if (!TileIsValid(x, y, z))
			{
				throw new ArgumentException("Tile xyz invalid.");
			}

			var min = PixelsToMeters(x * TileSize, y * TileSize, z, TileSize);
			var max = PixelsToMeters((x + 1) * TileSize, (y + 1) * TileSize, z, TileSize);

			var (minLon, minLat) = MetersToLatLon.MathTransform.Transform(min.X, min.Y);
			var (maxLon, maxLat) = MetersToLatLon.MathTransform.Transform(max.X, max.Y);

			return new Envelope(minLon, maxLon, -maxLat, -minLat);
		}

		public Geometry GetTileCoordinates(Geometry geometry, int x, int y, int z)
		{
			if (geometry is Polygon p)
			{
				return GetTileCoordinates(p, x, y, z);
			}
			else if (geometry is MultiPolygon mp)
			{
				return GetTileCoordinates(mp, x, y, z);
			}
			else if (geometry is Point pt)
			{
				return GetTileCoordinates(pt, x, y, z);
			}

			throw new ArgumentException("Invalid geometry type.");
		}

		public Polygon GetTileCoordinates(Polygon polygon, int x, int y, int z)
		{
			var exterior = GetTileCoordinates(polygon.ExteriorRing, x, y, z);
			var interiors = polygon.InteriorRings.Select(ring => GetTileCoordinates(ring, x, y, z)).ToArray();

			return new Polygon(exterior, interiors);
		}

		public MultiPolygon GetTileCoordinates(MultiPolygon multiPolygon, int x, int y, int z)
		{
			var polys = new List<Polygon>();

			foreach (Polygon poly in multiPolygon.Geometries)
			{
				polys.Add(GetTileCoordinates(poly, x, y, z));
			}

			return new MultiPolygon(polys.ToArray());
		}

		public LinearRing GetTileCoordinates(LineString line, int x, int y, int z)
		{
			var coords = line.Coordinates.Select(coord => new[] { coord.X, coord.Y }).ToList();

			var meters = LatLonToMeters.MathTransform.TransformList(coords);

			var min = PixelsToMeters(x * TileSize, y * TileSize, z, TileSize);
			var max = PixelsToMeters((x + 1) * TileSize, (y + 1) * TileSize, z, TileSize);

			foreach (var meter in meters)
			{
				var newy = Math.Round((meter[1] - -min.Y) / -(max.Y - min.Y) * VectorTileExtent, 0);
				var newx = Math.Round((meter[0] - min.X) / (max.X - min.X) * VectorTileExtent, 0);

				meter[0] = newx;
				meter[1] = newy;
			}

			return new LinearRing(meters.Select(x => new Coordinate(x[0], x[1])).ToArray());
		}

		public Point GetTileCoordinates(Point point, int x, int y, int z)
		{
			var (newx, newy) = LatLonToMeters.MathTransform.Transform(point.X, point.Y);

			var min = PixelsToMeters(x * TileSize, y * TileSize, z, TileSize);
			var max = PixelsToMeters((x + 1) * TileSize, (y + 1) * TileSize, z, TileSize);

			newy = Math.Round((newy - -min.Y) / -(max.Y - min.Y) * VectorTileExtent, 0);
			newx = Math.Round((newx - min.X) / (max.X - min.X) * VectorTileExtent, 0);

			return new Point(newx, newy);
		}

		private static Coordinate PixelsToMeters(double px, double py, int zoom, int tileSize)
		{
			var res = Resolution(zoom, tileSize);
			var mx = (px * res) - EarthHalfCircum;
			var my = (py * res) - EarthHalfCircum;
			return new Coordinate(mx, my);
		}

		private static double Resolution(int zoom, int tileSize)
		{
			return EarthCircum / (tileSize * Math.Pow(2, zoom));
		}

		private bool TileIsValid(int x, int y, int z)
		{
			var size = Math.Pow(2, z);

			return x >= 0 && y >= 0 && !(x >= size) && !(y >= size);
		}
	}
}
