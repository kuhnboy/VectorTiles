using Google.Protobuf.Collections;
using VectorTiles.VectorTile;
using Xunit;

namespace VectorTiles.Tests
{
	public class AttributeDecoderTests
	{
		[Fact]
		public void DecodeAttributes()
		{
			const string Key1 = "name";
			const string Value1 = "ABCD";
			const string Key2 = "id";
			const long Value2 = 123;

			var keys = new RepeatedField<string> { Key1, Key2 };
			var values = new RepeatedField<Tile.Types.Value>
				{ new Tile.Types.Value { StringValue = Value1 }, new Tile.Types.Value { IntValue = Value2 } };
			var tags = new RepeatedField<uint> { 0, 0, 1, 1 };

			var result = new AttributeDecoder().DecodeAttributes(keys, values, tags);

			Assert.Equal(Value1, result[Key1]);
			Assert.Equal(Value2, result[Key2]);
		}
	}
}
