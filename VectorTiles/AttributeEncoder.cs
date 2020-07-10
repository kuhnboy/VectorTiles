using System.Collections.Generic;
using Google.Protobuf.Collections;

namespace VectorTiles
{
	internal class AttributeEncoder
	{
		public RepeatedField<uint> EncodeAttributes(Dictionary<string, uint> allKeys, Dictionary<object, uint> allValues, IEnumerable<(string, object)> featureValues)
		{
			var result = new RepeatedField<uint>();

			foreach (var value in featureValues)
			{
				if (!allKeys.TryGetValue(value.Item1, out var keyIndex))
				{
					keyIndex = (uint)allKeys.Count;
					allKeys.Add(value.Item1, keyIndex);
				}

				if (!allValues.TryGetValue(value.Item2, out var valueIndex))
				{
					valueIndex = (uint)allValues.Count;
					allValues.Add(value.Item2, valueIndex);
				}

				result.Add(keyIndex);
				result.Add(valueIndex);
			}

			return result;
		}

		public VectorTile.Tile.Types.Value EmitValue(object value)
		{
			var result = new VectorTile.Tile.Types.Value();

			if (value is bool b)
			{
				result.BoolValue = b;
			}

			if (value is double d)
			{
				result.DoubleValue = d;
			}

			if (value is float f)
			{
				result.FloatValue = f;
			}

			if (value is long l)
			{
				result.IntValue = l;
			}

			//if (value is long l)
			//{
			//	result.SintValue = l;
			//}

			if (value is string s)
			{
				result.StringValue = s;
			}

			if (value is ulong u)
			{
				result.UintValue = u;
			}

			return result;
		}
	}
}
