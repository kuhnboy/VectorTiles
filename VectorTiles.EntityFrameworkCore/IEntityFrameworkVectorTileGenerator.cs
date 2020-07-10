using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace VectorTiles.EntityFrameworkCore
{
	public interface IEntityFrameworkVectorTileGenerator
	{
		Task<Stream> GenerateTile<T, TD>(IEnumerable<EntityQueryVectorLayer<T, TD>> layers, int x, int y, int z) where T : class where TD : DbContext;
	}
}
