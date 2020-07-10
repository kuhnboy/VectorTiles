using System.Runtime.CompilerServices;
using Microsoft.Extensions.DependencyInjection;

[assembly: InternalsVisibleTo("VectorTiles.Tests")]

namespace VectorTiles
{
	public static class VectorTileExtensions
	{
		/// <summary>
		/// Add services for vector tile generation.
		/// </summary>
		/// <param name="services"></param>
		public static void AddVectorTile(this IServiceCollection services)
		{
			services.AddSingleton<IVectorTileGenerator, VectorTileGenerator>();
		}
	}
}
