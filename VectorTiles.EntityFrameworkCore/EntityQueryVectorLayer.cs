using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;

namespace VectorTiles.EntityFrameworkCore
{
	public class EntityQueryVectorLayer<T, TD> where T : class where TD : DbContext
	{
		public TD Context { get; set; }
		public Func<TD, DbSet<T>> DbSet { get; set; }
		public FormattableString Query { get; set; }
		public Func<T, Geometry> GeoProperty { get; set; }
		public Func<T, IEnumerable<(string, object)>> Attributes { get; set; }

		public string Name { get; }

		public EntityQueryVectorLayer(string name)
		{
			Name = name;
		}
	}
}
