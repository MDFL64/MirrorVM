using System.Runtime.CompilerServices;

namespace MirrorVM
{
	// I don't know that NaN canonicalization is really worth supporting, but it doesn't seem to hurt performance much.
	class FloatHelper
	{
		[MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
		public static float CanonicalF32( float a )
		{
			return float.IsNaN(a) ? float.NaN : a;
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
		public static double CanonicalF64( double a )
		{
			return double.IsNaN(a) ? double.NaN : a;
		}
	}

	enum RegAllocMode
	{
		None, // only use registers for returns
		Basic, // allocates first 7 variables to registers
		Enhanced, // allocates most used variables to registers
	}

	class Config
	{
		public const bool DUMP_IR = false;

		public const int BLOCK_SPLIT_BUDGET = 99999;

		public const RegAllocMode REG_ALLOC_MODE = RegAllocMode.Basic;
		public const long REG_ALLOC_LOOP_WEIGHT = 1;

		public static int GetRegisterCount()
		{
			if ( REG_ALLOC_MODE != RegAllocMode.None )
			{
				return 7;
			}
			else
			{
				return 0;
			}
		}
	}
}
