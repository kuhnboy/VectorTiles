using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace VectorTiles.EntityFrameworkCore
{
	public class EntityFrameworkVectorTileGenerator(IVectorTileGenerator vectorTileGenerator)
		: IEntityFrameworkVectorTileGenerator
	{
		public Task<Stream> GenerateTile<T, TD>(IEnumerable<EntityQueryVectorLayer<T, TD>> layers, int x, int y, int z) where T : class where TD : DbContext
		{
			var tileLayers = layers.Select(layer =>
				new VectorLayer<T>(layer.Name,
					layer.DbSet(layer.Context).FromSqlInterpolated(layer.Query).AsAsyncEnumerable(), layer.GeoProperty, layer.Attributes));

			return vectorTileGenerator.GenerateTile(tileLayers, x, y, z);
		}
	}
}
