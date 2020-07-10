namespace VectorTiles
{
	/// <summary>
	/// For some reason Google's protobuf library doesn't expose these functions publically.
	/// </summary>
	internal static class ZigZagEncoding
	{
		/// <summary>
		/// Encode a 32-bit value with ZigZag encoding.
		/// </summary>
		/// <remarks>
		/// ZigZag encodes signed integers into values that can be efficiently
		/// encoded with varint.  (Otherwise, negative values must be 
		/// sign-extended to 64 bits to be varint encoded, thus always taking
		/// 10 bytes on the wire.)
		/// </remarks>
		public static uint EncodeZigZag32(int n)
		{
			// Note:  the right-shift must be arithmetic
			return (uint)((n << 1) ^ (n >> 31));
		}

		/// <summary>
		/// Decode a 32-bit value with ZigZag encoding.
		/// </summary>
		/// <remarks>
		/// ZigZag encodes signed integers into values that can be efficiently
		/// encoded with varint.  (Otherwise, negative values must be 
		/// sign-extended to 64 bits to be varint encoded, thus always taking
		/// 10 bytes on the wire.)
		/// </remarks>
		internal static int DecodeZigZag32(uint n)
		{
			return (int)(n >> 1) ^ -(int)(n & 1);
		}
	}
}
