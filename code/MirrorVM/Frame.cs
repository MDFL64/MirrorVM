using System;

namespace MirrorVM
{
	public class Frame
	{
		int ReturnCount;
		long[] Data;

		public Frame(int return_count, int size = 10_000)
		{
			ReturnCount = return_count;
			Data = new long[size];
		}

		public int GetReturnInt(int index = 0)
		{
			return (int)Data[index];
		}

		public long GetReturnLong(int index = 0)
		{
			return Data[index];
		}

		/*public Frame SetArg(int index, int value)
		{
			Data[ReturnCount + index] = value;
			return this;
		}*/

		public Frame SetArg(int index, long value)
		{
			Data[ReturnCount + index] = value;
			return this;
		}

		public static implicit operator Span<long>(Frame f) => f.Data;
	}
}
