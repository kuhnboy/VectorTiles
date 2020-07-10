using System.Collections.Generic;
using NetTopologySuite.Geometries;
using NetTopologySuite.Geometries.Utilities;
using NetTopologySuite.Operation.Polygonize;

namespace NetTopologySuite.Clipper
{
	public class PolygonClipper
	{
		public Geometry ClipPolygon(Geometry polygon, IPolygonal clipPolygonal)
		{
			var clipPolygon = (Geometry)clipPolygonal;
			var nodedLinework = polygon.Boundary.Union(clipPolygon.Boundary);
			var polygons = Polygonize(nodedLinework);

			// only keep polygons which are inside the input
			var output = new List<Geometry>();
			for (var i = 0; i < polygons.NumGeometries; i++)
			{
				var candpoly = (Polygon)polygons.GetGeometryN(i);
				var interiorPoint = candpoly.InteriorPoint;
				if (polygon.Contains(interiorPoint) &&
				    clipPolygon.Contains(interiorPoint))
				{
					output.Add(candpoly);
				}
			}

			return polygon.Factory.BuildGeometry(output);
		}

		private static Geometry Polygonize(Geometry geometry)
		{
			var lines = LineStringExtracter.GetLines(geometry);

			var polygonizer = new Polygonizer();
			polygonizer.Add(lines);
			var polys = polygonizer.GetPolygons();

			var polyArray = GeometryFactory.ToGeometryArray(polys);

			return geometry.Factory.CreateGeometryCollection(polyArray);
		}
	}
}
