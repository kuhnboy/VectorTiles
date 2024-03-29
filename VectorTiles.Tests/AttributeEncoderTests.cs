using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace VectorTiles.Tests
{
	[TestClass]
	public class AttributeEncoderTests
	{
		[TestMethod]
		public void EncodeAttributes()
		{
			const string key1 = "name";
			const string value1 = "ABCD";
			const string key2 = "id";
			const long value2 = 123;

			var allKeys = new Dictionary<string, uint>();
			var allValues = new Dictionary<object, uint>();

			var featureValues = new (string, object)[]
			{
				(key1, value1),
				(key2, value2)
			};

			var result = new AttributeEncoder().EncodeAttributes(allKeys, allValues, featureValues);

			Assert.AreEqual(2, allKeys.Count);
			Assert.AreEqual(2, allValues.Count);
			Assert.AreEqual(0u, result[0]);
			Assert.AreEqual(0u, result[1]);
			Assert.AreEqual(0u, allKeys[key1]);
			Assert.AreEqual(1u, allKeys[key2]);
			Assert.AreEqual(0u, allValues[value1]);
			Assert.AreEqual(1u, allValues[value2]);
		}
	}
}
