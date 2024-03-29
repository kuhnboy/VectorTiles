using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace VectorTiles
{
	public class VectorLayer<T>(string name, IAsyncEnumerable<T> items, Func<T, Geometry> geoProperty, Func<T, IEnumerable<(string, object)>> attributes)
		where T : class
	{
		public IAsyncEnumerable<T> Items { get; set; } = items;
		public Func<T, Geometry> GeoProperty { get; set; } = geoProperty;
		public Func<T, IEnumerable<(string, object)>> Attributes { get; set; } = attributes;

		public string Name { get; } = name;
	}
}
