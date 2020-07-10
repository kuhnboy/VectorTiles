using System.Collections.Generic;
using Google.Protobuf.Collections;

namespace VectorTiles
{
	internal class AttributeDecoder
	{
		public Dictionary<string, object> DecodeAttributes(RepeatedField<string> keys, RepeatedField<VectorTile.Tile.Types.Value> values, RepeatedField<uint> tags)
		{
			var result = new Dictionary<string, object>();

			for (var i = 0; i < tags.Count; i += 2)
			{
				var keyIndex = tags[i];
				var valueIndex = tags[i + 1];

				result[keys[(int)keyIndex]] = EmitValue(values[(int)valueIndex]);
			}

			return result;
		}

		private object EmitValue(VectorTile.Tile.Types.Value value)
		{
			if (value.HasBoolValue)
			{
				return value.BoolValue;
			}

			if (value.HasDoubleValue)
			{
				return value.DoubleValue;
			}

			if (value.HasFloatValue)
			{
				return value.FloatValue;
			}

			if (value.HasIntValue)
			{
				return value.IntValue;
			}

			if (value.HasSintValue)
			{
				return value.SintValue;
			}

			if (value.HasStringValue)
			{
				return value.StringValue;
			}

			if (value.HasUintValue)
			{
				return value.HasUintValue;
			}

			return null;
		}
	}
}
