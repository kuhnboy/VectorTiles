using System;
using System.Collections.Generic;
using Google.Protobuf.Collections;
using NetTopologySuite.Geometries;
using VectorTiles.VectorTile;

namespace VectorTiles
{
	internal class GeometryDecoder
	{
		public Geometry DecodeGeometry(RepeatedField<uint> geom, Tile.Types.GeomType geomType)
		{
			switch (geomType)
			{
				case Tile.Types.GeomType.Polygon:
					return DecodePolygon(geom);
				case Tile.Types.GeomType.Point:
					return DecodePoint(geom);
				default:
					throw new NotImplementedException();
			}
		}

		private Geometry DecodePoint(RepeatedField<uint> geom)
		{
			long x = 0;
			long y = 0;

			List<Point> coords = new List<Point>();

			uint length = 0;
			var i = 0;

			while (i < geom.Count)
			{
				if (length <= 0)
				{
					var commandInfo = geom[i++];

					length = commandInfo >> 3;
				}

				var dx = geom[i++];
				var dy = geom[i++];

				length--;

				var ldx = ZigZagEncoding.DecodeZigZag32(dx);
				var ldy = ZigZagEncoding.DecodeZigZag32(dy);

				x += ldx;
				y += ldy;

				coords.Add(new Point(x, y));
			}

			return coords.Count == 1 ? coords[0] : (Geometry)new MultiPoint(coords.ToArray());
		}

		private Geometry DecodePolygon(RepeatedField<uint> geom)
		{
			long x = 0;
			long y = 0;

			var ringList = new List<LinearRing>();
			List<Coordinate> coords = null;

			uint length = 0;
			uint command = 0;
			var i = 0;

			while (i < geom.Count)
			{
				if (length <= 0)
				{
					var commandInfo = geom[i++];

					command = commandInfo & 0x7;
					length = commandInfo >> 3;
				}

				if (length > 0)
				{
					if (command == (uint)Command.MoveTo)
					{
						if (coords != null)
						{
							ringList.Add(new LinearRing(coords.ToArray()));
						}

						coords = new List<Coordinate>();
					}
				}

				if (command == (uint)Command.ClosePath)
				{
					if (coords.Count != 0)
					{
						coords.Add(coords[0]);
					}

					length--;
				}
				else
				{
					var dx = geom[i++];
					var dy = geom[i++];

					length--;

					var ldx = ZigZagEncoding.DecodeZigZag32(dx);
					var ldy = ZigZagEncoding.DecodeZigZag32(dy);

					x += ldx;
					y += ldy;

					coords.Add(new Coordinate { X = x, Y = y });
				}
			}

			if (coords != null)
			{
				ringList.Add(new LinearRing(coords.ToArray()));
			}

			var polygonList = new List<Polygon>();

			LinearRing currentExterior = null;
			var currentInterior = new List<LinearRing>();

			foreach (var ring in ringList)
			{
				if (!ring.IsCCW)
				{
					if (currentExterior == null)
					{
						throw new Exception(
							"Invalid format as exterior does not exist. Exterior ring must be counterclockwise");
					}

					currentInterior.Add(ring);
				}
				else
				{
					if (currentExterior != null)
					{
						polygonList.Add(new Polygon(currentExterior, currentInterior.ToArray()));
					}

					currentExterior = ring;
					currentInterior = new List<LinearRing>();
				}
			}

			if (currentExterior != null)
			{
				polygonList.Add(new Polygon(currentExterior, currentInterior.ToArray()));
			}

			return polygonList.Count == 1 ? polygonList[0] : (Geometry)new MultiPolygon(polygonList.ToArray());
		}
	}
}
