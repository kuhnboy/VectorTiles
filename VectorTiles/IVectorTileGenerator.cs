using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace VectorTiles
{
	public interface IVectorTileGenerator
	{
		Task<Stream> GenerateTile<T>(IEnumerable<VectorLayer<T>> layers, int x, int y, int z) where T : class;
	}
}
