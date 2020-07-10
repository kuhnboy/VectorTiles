using System.Collections.Generic;
using Xunit;

namespace VectorTiles.Tests
{
	public class AttributeEncoderTests
	{
		[Fact]
		public void EncodeAttributes()
		{
			const string Key1 = "name";
			const string Value1 = "ABCD";
			const string Key2 = "id";
			const long Value2 = 123;

			var allKeys = new Dictionary<string, uint>();
			var allValues = new Dictionary<object, uint>();

			var featureValues = new (string, object)[]
			{
				(Key1, Value1),
				(Key2, Value2)
			};

			var result = new AttributeEncoder().EncodeAttributes(allKeys, allValues, featureValues);

			Assert.Equal(2, allKeys.Count);
			Assert.Equal(2, allValues.Count);
			Assert.Equal(0u, result[0]);
			Assert.Equal(0u, result[1]);
			Assert.Equal(0u, allKeys[Key1]);
			Assert.Equal(1u, allKeys[Key2]);
			Assert.Equal(0u, allValues[Value1]);
			Assert.Equal(1u, allValues[Value2]);
		}
	}
}
