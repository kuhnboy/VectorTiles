using Microsoft.Extensions.DependencyInjection;

namespace VectorTiles.EntityFrameworkCore
{
	public static class EntityFrameworkVectorTileExtensions
	{
		/// <summary>
		/// Add services for vector tile generation.
		/// </summary>
		/// <param name="services"></param>
		public static void AddEntityFrameworkVectorTile(this IServiceCollection services)
		{
			services.AddVectorTile();

			services.AddSingleton<IEntityFrameworkVectorTileGenerator, EntityFrameworkVectorTileGenerator>();
		}
	}
}
