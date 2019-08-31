using System;
namespace Ryne.Utility.Math
{
	sealed class Bitmask 
	{
		static int Negate(int value)
		{
			return -value;
		}

		static uint Negate(uint value)
		{
			return ~value + 1;
		}

		static long Negate(long value)
		{
			return -value;
		}

		static ulong Negate(ulong value)
		{
			return ~value + 1;
		}

		// Returns the bitmask with given bit set to 1
		public static int GetMask(int bit)
		{
			return 1 << bit;
		}

        public static bool Check(int mask, int flag) 
		{
            return (mask & flag) > 0;
        }

		public static bool CheckBit(int mask, int bit)
        {
            int flag = (int)1 << bit;
            return (mask & flag) > 0;
        }

		public static int SetBit(int mask, int bit)
		{
			int flag = (int)1 << bit;
			return mask | flag;
		}

		public static int ClearBit(int mask, int bit)
		{
			int flag = (int)1 << bit;
			return mask & ~flag;
		}

		public static int SetBitTo(int mask, int bit, int value)
		{
			int flagMask = Negate(value) ^ mask;
			int flag = (int)1 << bit;
			return mask ^ (flagMask & flag);
		}

        public static bool Check(long mask, long flag) 
		{
            return (mask & flag) > 0;
        }

		public static bool CheckBit(long mask, int bit)
        {
            long flag = (long)1 << bit;
            return (mask & flag) > 0;
        }

		public static long SetBit(long mask, int bit)
		{
			long flag = (long)1 << bit;
			return mask | flag;
		}

		public static long ClearBit(long mask, int bit)
		{
			long flag = (long)1 << bit;
			return mask & ~flag;
		}

		public static long SetBitTo(long mask, int bit, long value)
		{
			long flagMask = Negate(value) ^ mask;
			long flag = (long)1 << bit;
			return mask ^ (flagMask & flag);
		}

        public static bool Check(uint mask, uint flag) 
		{
            return (mask & flag) > 0;
        }

		public static bool CheckBit(uint mask, int bit)
        {
            uint flag = (uint)1 << bit;
            return (mask & flag) > 0;
        }

		public static uint SetBit(uint mask, int bit)
		{
			uint flag = (uint)1 << bit;
			return mask | flag;
		}

		public static uint ClearBit(uint mask, int bit)
		{
			uint flag = (uint)1 << bit;
			return mask & ~flag;
		}

		public static uint SetBitTo(uint mask, int bit, uint value)
		{
			uint flagMask = Negate(value) ^ mask;
			uint flag = (uint)1 << bit;
			return mask ^ (flagMask & flag);
		}

        public static bool Check(ulong mask, ulong flag) 
		{
            return (mask & flag) > 0;
        }

		public static bool CheckBit(ulong mask, int bit)
        {
            ulong flag = (ulong)1 << bit;
            return (mask & flag) > 0;
        }

		public static ulong SetBit(ulong mask, int bit)
		{
			ulong flag = (ulong)1 << bit;
			return mask | flag;
		}

		public static ulong ClearBit(ulong mask, int bit)
		{
			ulong flag = (ulong)1 << bit;
			return mask & ~flag;
		}

		public static ulong SetBitTo(ulong mask, int bit, ulong value)
		{
			ulong flagMask = Negate(value) ^ mask;
			ulong flag = (ulong)1 << bit;
			return mask ^ (flagMask & flag);
		}

	}
}
