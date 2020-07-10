using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using Xunit;

namespace NetTopologySuite.Clipper.Tests
{
	public class PolygonClipperTests
	{
		[Fact]
		public void ClipMultiPolygonTest()
		{
			var wkt = new WKTReader().Read(
				"MULTIPOLYGON(((-80.66859097886578 26.663762716650286, -80.6688230969945 26.656837138659036, -80.667795418133792 26.65682937086758, -80.667686578933086 26.65727471507439, -80.666963512952989 26.65730523411106, -80.6667617362841 26.663742584000332, -80.66859097886578 26.663762716650286)), ((-80.670514647254109 26.663709430760704, -80.670723135400976 26.656841553231835, -80.668894781003658 26.656842673351715, -80.66867462160026 26.663799608104284, -80.670163963623935 26.66379677545433, -80.670514647254109 26.663709430760704)))");

			var clipWkt =
				(Polygon)new WKTReader().Read(
					"POLYGON ((-80.672607421875 26.657277674217585, -80.6671142578125 26.657277674217585, -80.6671142578125 26.662186843258542, -80.672607421875 26.662186843258542, -80.672607421875 26.657277674217585))");

			var clipped = new PolygonClipper().ClipPolygon(wkt, clipWkt);

			Assert.Equal("MULTIPOLYGON (((-80.670709896076929 26.657277674217585, -80.670560868440418 26.662186843258542, -80.668725659213649 26.662186843258542, -80.668881014950856 26.657277674217585, -80.670709896076929 26.657277674217585)), ((-80.668808331975669 26.657277674217585, -80.668643795942316 26.662186843258542, -80.6671142578125 26.662186843258542, -80.6671142578125 26.657298871499595, -80.667616470043413 26.657277674217585, -80.668808331975669 26.657277674217585)))", clipped.ToString());
		}
	}
}
