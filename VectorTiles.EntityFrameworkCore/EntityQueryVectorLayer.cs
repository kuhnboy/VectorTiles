using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;

namespace VectorTiles.EntityFrameworkCore
{
	public class EntityQueryVectorLayer<T, TD>(string name, TD context, Func<TD, DbSet<T>> dbSet, FormattableString query, Func<T, Geometry> geoProperty, Func<T, IEnumerable<(string, object)>> attributes)
		where T : class
		where TD : DbContext
	{
		public TD Context { get; set; } = context;
		public Func<TD, DbSet<T>> DbSet { get; set; } = dbSet;
		public FormattableString Query { get; set; } = query;
		public Func<T, Geometry> GeoProperty { get; set; } = geoProperty;
		public Func<T, IEnumerable<(string, object)>> Attributes { get; set; } = attributes;

		public string Name { get; } = name;
	}
}
