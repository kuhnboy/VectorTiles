using Google.Protobuf.Collections;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VectorTiles.VectorTile;

namespace VectorTiles.Tests
{
	[TestClass]
	public class AttributeDecoderTests
	{
		[TestMethod]
		public void DecodeAttributes()
		{
			const string key1 = "name";
			const string value1 = "ABCD";
			const string key2 = "id";
			const long value2 = 123;

			var keys = new RepeatedField<string> { key1, key2 };
			var values = new RepeatedField<Tile.Types.Value>
				{ new Tile.Types.Value { StringValue = value1 }, new Tile.Types.Value { IntValue = value2 } };
			var tags = new RepeatedField<uint> { 0, 0, 1, 1 };

			var result = new AttributeDecoder().DecodeAttributes(keys, values, tags);

			Assert.AreEqual(value1, result[key1]);
			Assert.AreEqual(value2, result[key2]);
		}
	}
}
