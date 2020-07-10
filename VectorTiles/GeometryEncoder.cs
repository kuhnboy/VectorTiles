using System;
using System.Collections.Generic;
using System.Linq;
using Google.Protobuf.Collections;
using NetTopologySuite.Geometries;
using VectorTiles.VectorTile;

namespace VectorTiles
{
	internal class GeometryEncoder
	{
		public (RepeatedField<uint> geom, Tile.Types.GeomType geomType) EncodeGeometry(Geometry geometry)
		{
			if (geometry is Polygon || geometry is MultiPolygon)
			{
				return (EncodePolygon(geometry), Tile.Types.GeomType.Polygon);
			}
			else if (geometry is Point || geometry is MultiPoint)
			{
				return (EncodePoint(geometry), Tile.Types.GeomType.Point);
			}
			else
			{
				throw new NotImplementedException();
			}
		}

		private RepeatedField<uint> EncodePoint(Geometry geometry)
		{
			var result = new RepeatedField<uint>();

			var points = new List<Point>();

			if (geometry is Point p)
			{
				points.Add(p);
			}
			else
			{
				points.AddRange(((MultiPoint)geometry).Geometries.Select(point => (Point)point));
			}

			int x = 0;
			int y = 0;

			CreatePoint(result, points, ref x, ref y);

			return result;
		}

		private RepeatedField<uint> EncodePolygon(Geometry geometry)
		{
			var result = new RepeatedField<uint>();

			var polygons = new List<Polygon>();

			if (geometry is Polygon p)
			{
				polygons.Add(p);
			}
			else
			{
				polygons.AddRange(((MultiPolygon)geometry).Geometries.Select(poly => (Polygon)poly));
			}

			int x = 0;
			int y = 0;

			foreach (var polygon in polygons)
			{
				CreatePath(result, polygon.ExteriorRing, ref x, ref y);

				foreach (var interior in polygon.InteriorRings)
				{
					CreatePath(result, interior, ref x, ref y);
				}
			}

			return result;
		}

		private void CreatePoint(RepeatedField<uint> result, List<Point> points, ref int x, ref int y)
		{
			result.Add(CreateCommand(Command.MoveTo, (uint)points.Count));

			for (var i = 0; i < points.Count; i++)
			{
				var coord = points[i];

				var dx = (int)coord.X - x;
				var dy = (int)coord.Y - y;

				result.Add(ZigZagEncoding.EncodeZigZag32(dx));
				result.Add(ZigZagEncoding.EncodeZigZag32(dy));

				x += dx;
				y += dy;
			}
		}

		private void CreatePath(RepeatedField<uint> result, LineString ring, ref int x, ref int y)
		{
			result.Add(CreateCommand(Command.MoveTo, 1));

			var coord = ring.Coordinates[0];

			var dx = (int)coord.X - x;
			var dy = (int)coord.Y - y;

			result.Add(ZigZagEncoding.EncodeZigZag32(dx));
			result.Add(ZigZagEncoding.EncodeZigZag32(dy));

			x += dx;
			y += dy;

			result.Add(CreateCommand(Command.LineTo, (uint)(ring.Count - 2)));

			for (var i = 1; i < ring.Count - 1; i++)
			{
				coord = ring[i];

				dx = (int)coord.X - x;
				dy = (int)coord.Y - y;

				result.Add(ZigZagEncoding.EncodeZigZag32(dx));
				result.Add(ZigZagEncoding.EncodeZigZag32(dy));

				x += dx;
				y += dy;
			}

			result.Add(CreateCommand(Command.ClosePath, 1));
		}

		private uint CreateCommand(Command command, uint count)
		{
			return ((uint)command & 0x7) | (count << 3);
		}
	}
}
