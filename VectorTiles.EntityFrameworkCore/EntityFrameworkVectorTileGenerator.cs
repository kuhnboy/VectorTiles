using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace VectorTiles.EntityFrameworkCore
{
	public class EntityFrameworkVectorTileGenerator : IEntityFrameworkVectorTileGenerator
	{
		private readonly IVectorTileGenerator vectorTileGenerator;

		public EntityFrameworkVectorTileGenerator(IVectorTileGenerator vectorTileGenerator)
		{
			this.vectorTileGenerator = vectorTileGenerator;
		}

		public async Task<Stream> GenerateTile<T, TD>(IEnumerable<EntityQueryVectorLayer<T, TD>> layers, int x, int y, int z) where T : class where TD : DbContext
		{
			var tileLayers = layers.Select(layer => new VectorLayer<T>(layer.Name)
			{
				Items = layer.DbSet(layer.Context).FromSqlInterpolated(layer.Query).AsAsyncEnumerable(), Attributes = layer.Attributes,
				GeoProperty = layer.GeoProperty
			});

			return await vectorTileGenerator.GenerateTile(tileLayers, x, y, z);
		}
	}
}
