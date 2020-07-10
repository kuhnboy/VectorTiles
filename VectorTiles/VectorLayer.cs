using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace VectorTiles
{
	public class VectorLayer<T> where T : class
	{
		public IAsyncEnumerable<T> Items { get; set; }
		public Func<T, Geometry> GeoProperty { get; set; }
		public Func<T, IEnumerable<(string, object)>> Attributes { get; set; }

		public string Name { get; }

		public VectorLayer(string name)
		{
			Name = name;
		}
	}
}
