using Google.Protobuf.Collections;
using NetTopologySuite.IO;
using VectorTiles.VectorTile;
using Xunit;

namespace VectorTiles.Tests
{
	public class GeometryEncoderTests
	{
		[Fact]
		public void EncodeGeometry_Polygon()
		{
			var wkt = new WKTReader().Read("POLYGON ((1818 2642, 1818 2628, 1862 2628, 1862 2641, 1818 2642))");

			var expectedResult = new RepeatedField<uint>
			{
				new uint[]
				{
					9, 3636, 5284, 26, 0, 27, 88, 0, 0, 26, 15
				}
			};

			var (geometry, type) = new GeometryEncoder().EncodeGeometry(wkt);

			Assert.Equal(Tile.Types.GeomType.Polygon, type);
			Assert.Equal(expectedResult, geometry);
		}

		[Fact]
		public void EncodeGeometry_MultiPolygon()
		{
			var wkt = new WKTReader().Read("MULTIPOLYGON (((2857 3095, 2826 3092, 2825 3091, 2826 3091, 2829 3091, 2831 3091, 2832 3090, 2855 3092, 2856 3092, 2857 3095)), ((2861 3099, 2827 3096, 2827 3095, 2827 3094, 2826 3093, 2857 3095, 2858 3096, 2859 3096, 2860 3097, 2861 3097, 2862 3098, 2862 3099, 2861 3099)), ((2837 3103, 2835 3101, 2834 3101, 2827 3100, 2827 3098, 2827 3097, 2862 3100, 2863 3103, 2861 3104, 2859 3104, 2857 3103, 2855 3103, 2854 3103, 2852 3103, 2844 3103, 2837 3103)))");

			var expectedResult = new RepeatedField<uint>
			{
				new uint[]
				{
					9, 5714, 6190, 66, 61, 5, 1, 1, 2, 0, 6, 0, 4, 0, 2, 1, 46, 4, 2, 0, 15, 9, 10, 14, 90, 67,
					5, 0, 1, 0, 1, 1, 1, 62, 4, 2, 2, 2, 0, 2, 2, 2, 0, 2, 2, 0, 2, 15, 9, 49, 8, 114, 3, 3, 1,
					0, 13, 1, 0, 3, 0, 1, 70, 6, 2, 6, 3, 2, 3, 0, 3, 1, 3, 0, 1, 0, 3, 0, 15, 0, 15
				}
			};


			var (geometry, type) = new GeometryEncoder().EncodeGeometry(wkt);

			Assert.Equal(Tile.Types.GeomType.Polygon, type);
			Assert.Equal(expectedResult, geometry);
		}

		[Fact]
		public void EncodeGeometry_MultiPolygon_InnerRing()
		{
			var wkt = new WKTReader().Read(
				"MULTIPOLYGON (((2792 3425, 2792 3424, 2791 3423, 2789 3421, 2796 3421, 2801 3425, 2792 3425)), ((2781 3426, 2780 3422, 2781 3421, 2782 3421, 2786 3421, 2784 3425, 2782 3426, 2781 3426)), ((2793 3429, 2792 3425, 2802 3425, 2804 3426, 2803 3427, 2802 3427, 2802 3428, 2803 3428, 2803 3429, 2793 3429)), ((2795 3433, 2794 3431, 2793 3430, 2793 3429, 2808 3429, 2812 3432, 2795 3433)), ((2813 3436, 2813 3435, 2814 3434, 2816 3434, 2819 3436, 2813 3436)), ((2784 3437, 2783 3434, 2786 3434, 2788 3434, 2790 3436, 2790 3437, 2784 3437)), ((2807 3440, 2809 3437, 2820 3437, 2824 3440, 2807 3440)), ((2782 3441, 2782 3440, 2783 3439, 2783 3438, 2791 3437, 2792 3439, 2793 3440, 2793 3441, 2782 3441)), ((2806 3444, 2807 3441, 2825 3440, 2829 3444, 2806 3444), (2819 3444, 2821 3444, 2821 3443, 2821 3442, 2821 3441, 2820 3441, 2819 3441, 2819 3442, 2818 3443, 2819 3444, 2819 3444)), ((2778 3445, 2779 3444, 2779 3443, 2781 3442, 2793 3441, 2793 3442, 2794 3443, 2794 3444, 2795 3444, 2795 3445, 2778 3445)), ((2806 3448, 2806 3445, 2830 3444, 2834 3448, 2821 3448, 2820 3447, 2818 3447, 2817 3448, 2806 3448)), ((2778 3449, 2778 3446, 2798 3445, 2798 3446, 2798 3449, 2778 3449)), ((2826 3453, 2823 3452, 2821 3452, 2820 3451, 2821 3449, 2835 3448, 2836 3449, 2836 3450, 2836 3452, 2835 3452, 2835 3453, 2828 3453, 2826 3453)), ((2805 3452, 2805 3451, 2805 3449, 2817 3449, 2817 3450, 2817 3452, 2805 3452)), ((2778 3453, 2778 3450, 2798 3449, 2797 3452, 2778 3453)))");

			var expectedResult = new RepeatedField<uint>()
			{
				new uint[]
				{
					9, 5584, 6850, 42, 0, 1, 1, 1, 3, 3, 14, 0, 10, 8, 15, 9, 39, 2, 50, 1, 7, 2, 1, 2, 0, 8, 0, 3, 8,
					3, 2,
					15, 9, 22, 6, 66, 1, 7, 20, 0, 4, 2, 1, 2, 1, 0, 0, 2, 2, 0, 0, 2, 15, 9, 15, 8, 42, 1, 3, 1, 1, 0,
					1,
					30, 0, 8, 6, 15, 9, 2, 8, 34, 0, 1, 2, 1, 4, 0, 6, 4, 15, 9, 69, 2, 42, 1, 5, 6, 0, 4, 0, 4, 4, 0,
					2,
					15, 9, 34, 6, 26, 4, 5, 22, 0, 8, 6, 15, 9, 83, 2, 58, 0, 1, 2, 1, 0, 1, 16, 1, 2, 4, 2, 2, 0, 2,
					15, 9,
					26, 6, 26, 2, 5, 36, 1, 8, 8, 15, 9, 19, 0, 74, 4, 0, 0, 1, 0, 1, 0, 1, 1, 0, 1, 0, 0, 2, 1, 2, 2,
					2,
					15, 9, 81, 2, 74, 2, 1, 0, 1, 4, 1, 24, 1, 0, 2, 2, 2, 0, 2, 2, 0, 0, 2, 15, 9, 22, 6, 58, 0, 5, 48,
					1,
					8, 8, 25, 0, 1, 1, 3, 0, 1, 2, 15, 9, 77, 2, 34, 0, 5, 40, 1, 0, 2, 0, 6, 15, 9, 56, 8, 90, 5, 1, 3,
					0,
					1, 1, 2, 3, 28, 1, 2, 2, 0, 2, 0, 4, 1, 0, 0, 2, 13, 0, 15, 9, 45, 1, 42, 0, 1, 0, 3, 24, 0, 0, 2,
					0, 4,
					15, 9, 77, 2, 26, 0, 5, 40, 1, 1, 6, 15
				}
			};

			var (geometry, type) = new GeometryEncoder().EncodeGeometry(wkt);

			Assert.Equal(Tile.Types.GeomType.Polygon, type);
			Assert.Equal(expectedResult, geometry);
		}

		[Fact]
		public void EncodeGeometry_Point()
		{
			var wkt = new WKTReader().Read("POINT (1818 2642)");

			var expectedResult = new RepeatedField<uint>
			{
				new uint[]
				{
					9, 3636, 5284
				}
			};

			var (geometry, type) = new GeometryEncoder().EncodeGeometry(wkt);

			Assert.Equal(Tile.Types.GeomType.Point, type);
			Assert.Equal(expectedResult, geometry);
		}

		[Fact]
		public void EncodeGeometry_MultiPoint()
		{
			var wkt = new WKTReader().Read("MULTIPOINT ((1818 2642), (1818 2628), (1862 2628), (1862 2641))");

			var expectedResult = new RepeatedField<uint>
			{
				new uint[]
				{
					33, 3636, 5284, 0, 27, 88, 0, 0, 26
				}
			};

			var (geometry, type) = new GeometryEncoder().EncodeGeometry(wkt);

			Assert.Equal(Tile.Types.GeomType.Point, type);
			Assert.Equal(expectedResult, geometry);
		}
	}
}
