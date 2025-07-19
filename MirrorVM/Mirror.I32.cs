using System;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace MirrorVM
{
	struct Op_I32_EqualZero<A> : Expr<int> where A : struct, Expr<int>
	{
		[MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
		public int Run( ref Registers reg, Span<long> frame, WasmInstance inst ) => default( A ).Run( ref reg, frame, inst ) == 0 ? 1 : 0;
	}
	struct Op_I32_Equal<A, B> : Expr<int> where A : struct, Expr<int> where B : struct, Expr<int>
	{
		[MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
		public int Run( ref Registers reg, Span<long> frame, WasmInstance inst ) => default( A ).Run( ref reg, frame, inst ) == default( B ).Run( ref reg, frame, inst ) ? 1 : 0;
	}
	struct Op_I32_NotEqual<A, B> : Expr<int> where A : struct, Expr<int> where B : struct, Expr<int>
	{
		[MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
		public int Run( ref Registers reg, Span<long> frame, WasmInstance inst ) => default( A ).Run( ref reg, frame, inst ) != default( B ).Run( ref reg, frame, inst ) ? 1 : 0;
	}
	struct Op_I32_GreaterEqual_S<A, B> : Expr<int> where A : struct, Expr<int> where B : struct, Expr<int>
	{
		[MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
		public int Run( ref Registers reg, Span<long> frame, WasmInstance inst ) => default( A ).Run( ref reg, frame, inst ) >= default( B ).Run( ref reg, frame, inst ) ? 1 : 0;
	}
	struct Op_I32_Greater_S<A, B> : Expr<int> where A : struct, Expr<int> where B : struct, Expr<int>
	{
		[MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
		public int Run( ref Registers reg, Span<long> frame, WasmInstance inst ) => default( A ).Run( ref reg, frame, inst ) > default( B ).Run( ref reg, frame, inst ) ? 1 : 0;
	}
	struct Op_I32_LessEqual_S<A, B> : Expr<int> where A : struct, Expr<int> where B : struct, Expr<int>
	{
		[MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
		public int Run( ref Registers reg, Span<long> frame, WasmInstance inst ) => default( A ).Run( ref reg, frame, inst ) <= default( B ).Run( ref reg, frame, inst ) ? 1 : 0;
	}
	struct Op_I32_Less_S<A, B> : Expr<int> where A : struct, Expr<int> where B : struct, Expr<int>
	{
		[MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
		public int Run( ref Registers reg, Span<long> frame, WasmInstance inst ) => default( A ).Run( ref reg, frame, inst ) < default( B ).Run( ref reg, frame, inst ) ? 1 : 0;
	}
	struct Op_I32_GreaterEqual_U<A, B> : Expr<int> where A : struct, Expr<int> where B : struct, Expr<int>
	{
		[MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
		public int Run( ref Registers reg, Span<long> frame, WasmInstance inst ) => (uint)default( A ).Run( ref reg, frame, inst ) >= (uint)default( B ).Run( ref reg, frame, inst ) ? 1 : 0;
	}
	struct Op_I32_Greater_U<A, B> : Expr<int> where A : struct, Expr<int> where B : struct, Expr<int>
	{
		[MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
		public int Run( ref Registers reg, Span<long> frame, WasmInstance inst ) => (uint)default( A ).Run( ref reg, frame, inst ) > (uint)default( B ).Run( ref reg, frame, inst ) ? 1 : 0;
	}
	struct Op_I32_LessEqual_U<A, B> : Expr<int> where A : struct, Expr<int> where B : struct, Expr<int>
	{
		[MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
		public int Run( ref Registers reg, Span<long> frame, WasmInstance inst ) => (uint)default( A ).Run( ref reg, frame, inst ) <= (uint)default( B ).Run( ref reg, frame, inst ) ? 1 : 0;
	}
	struct Op_I32_Less_U<A, B> : Expr<int> where A : struct, Expr<int> where B : struct, Expr<int>
	{
		[MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
		public int Run( ref Registers reg, Span<long> frame, WasmInstance inst ) => (uint)default( A ).Run( ref reg, frame, inst ) < (uint)default( B ).Run( ref reg, frame, inst ) ? 1 : 0;
	}
	// END COMPARISONS

	struct Const_I32<C> : Expr<int>
		where C : struct, Const
	{
		[MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
		public int Run( ref Registers reg, Span<long> frame, WasmInstance inst ) => (int)default( C ).Run();
	}
	struct Op_I32_Add<A, B> : Expr<int> where A : struct, Expr<int> where B : struct, Expr<int>
	{
		[MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
		public int Run( ref Registers reg, Span<long> frame, WasmInstance inst ) => default( A ).Run( ref reg, frame, inst ) + default( B ).Run( ref reg, frame, inst );
	}
	struct Op_I32_Sub<A, B> : Expr<int> where A : struct, Expr<int> where B : struct, Expr<int>
	{
		[MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
		public int Run( ref Registers reg, Span<long> frame, WasmInstance inst ) => default( A ).Run( ref reg, frame, inst ) - default( B ).Run( ref reg, frame, inst );
	}
	struct Op_I32_Mul<A, B> : Expr<int> where A : struct, Expr<int> where B : struct, Expr<int>
	{
		[MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
		public int Run( ref Registers reg, Span<long> frame, WasmInstance inst ) => default( A ).Run( ref reg, frame, inst ) * default( B ).Run( ref reg, frame, inst );
	}
	struct Op_I32_Div_S<A, B> : Expr<int> where A : struct, Expr<int> where B : struct, Expr<int>
	{
		[MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
		public int Run( ref Registers reg, Span<long> frame, WasmInstance inst ) => default( A ).Run( ref reg, frame, inst ) / default( B ).Run( ref reg, frame, inst );
	}
	struct Op_I32_Div_U<A, B> : Expr<int> where A : struct, Expr<int> where B : struct, Expr<int>
	{
		[MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
		public int Run( ref Registers reg, Span<long> frame, WasmInstance inst ) => (int)((uint)default( A ).Run( ref reg, frame, inst ) / (uint)default( B ).Run( ref reg, frame, inst ));
	}
	struct Op_I32_Rem_S<A, B> : Expr<int> where A : struct, Expr<int> where B : struct, Expr<int>
	{
		[MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
		public int Run( ref Registers reg, Span<long> frame, WasmInstance inst )
		{
			int a = default( A ).Run( ref reg, frame, inst );
			int b = default( B ).Run( ref reg, frame, inst );
			// I tried to find a better way -- casting to longs is slower
			if ( a == -2147483648 && b == -1 )
			{
				return 0;
			}
			else
			{
				return a % b;
			}
		}
	}
	struct Op_I32_Rem_U<A, B> : Expr<int> where A : struct, Expr<int> where B : struct, Expr<int>
	{
		[MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
		public int Run( ref Registers reg, Span<long> frame, WasmInstance inst ) => (int)((uint)default( A ).Run( ref reg, frame, inst ) % (uint)default( B ).Run( ref reg, frame, inst ));
	}
	struct Op_I32_And<A, B> : Expr<int> where A : struct, Expr<int> where B : struct, Expr<int>
	{
		[MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
		public int Run( ref Registers reg, Span<long> frame, WasmInstance inst ) => default( A ).Run( ref reg, frame, inst ) & default( B ).Run( ref reg, frame, inst );
	}
	struct Op_I32_Or<A, B> : Expr<int> where A : struct, Expr<int> where B : struct, Expr<int>
	{
		[MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
		public int Run( ref Registers reg, Span<long> frame, WasmInstance inst ) => default( A ).Run( ref reg, frame, inst ) | default( B ).Run( ref reg, frame, inst );
	}
	struct Op_I32_Xor<A, B> : Expr<int> where A : struct, Expr<int> where B : struct, Expr<int>
	{
		[MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
		public int Run( ref Registers reg, Span<long> frame, WasmInstance inst ) => default( A ).Run( ref reg, frame, inst ) ^ default( B ).Run( ref reg, frame, inst );
	}
	struct Op_I32_ShiftLeft<A, B> : Expr<int> where A : struct, Expr<int> where B : struct, Expr<int>
	{
		[MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
		public int Run( ref Registers reg, Span<long> frame, WasmInstance inst ) => default( A ).Run( ref reg, frame, inst ) << default( B ).Run( ref reg, frame, inst );
	}
	struct Op_I32_ShiftRight_S<A, B> : Expr<int> where A : struct, Expr<int> where B : struct, Expr<int>
	{
		[MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
		public int Run( ref Registers reg, Span<long> frame, WasmInstance inst ) => default( A ).Run( ref reg, frame, inst ) >> default( B ).Run( ref reg, frame, inst );
	}
	struct Op_I32_ShiftRight_U<A, B> : Expr<int> where A : struct, Expr<int> where B : struct, Expr<int>
	{
		[MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
		public int Run( ref Registers reg, Span<long> frame, WasmInstance inst ) => default( A ).Run( ref reg, frame, inst ) >>> default( B ).Run( ref reg, frame, inst );
	}
	struct Op_I32_RotateLeft<A, B> : Expr<int> where A : struct, Expr<int> where B : struct, Expr<int>
	{
		[MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
		public int Run( ref Registers reg, Span<long> frame, WasmInstance inst ) => (int)BitOperations.RotateLeft( (uint)default( A ).Run( ref reg, frame, inst ), default( B ).Run( ref reg, frame, inst ) );
	}
	struct Op_I32_RotateRight<A, B> : Expr<int> where A : struct, Expr<int> where B : struct, Expr<int>
	{
		[MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
		public int Run( ref Registers reg, Span<long> frame, WasmInstance inst ) => (int)BitOperations.RotateRight( (uint)default( A ).Run( ref reg, frame, inst ), default( B ).Run( ref reg, frame, inst ) );
	}
	struct Op_I32_LeadingZeros<A> : Expr<int> where A : struct, Expr<int>
	{
		[MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
		public int Run( ref Registers reg, Span<long> frame, WasmInstance inst ) => BitOperations.LeadingZeroCount( (uint)default( A ).Run( ref reg, frame, inst ) );
	}
	struct Op_I32_TrailingZeros<A> : Expr<int> where A : struct, Expr<int>
	{
		[MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
		public int Run( ref Registers reg, Span<long> frame, WasmInstance inst ) => BitOperations.TrailingZeroCount( (uint)default( A ).Run( ref reg, frame, inst ) );
	}
	struct Op_I32_PopCount<A> : Expr<int> where A : struct, Expr<int>
	{
		[MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
		public int Run( ref Registers reg, Span<long> frame, WasmInstance inst ) => BitOperations.PopCount( (uint)default( A ).Run( ref reg, frame, inst ) );
	}
	struct Op_I32_Extend8_S<A> : Expr<int> where A : struct, Expr<int>
	{
		[MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
		public int Run( ref Registers reg, Span<long> frame, WasmInstance inst ) => (sbyte)default( A ).Run( ref reg, frame, inst );
	}
	struct Op_I32_Extend16_S<A> : Expr<int> where A : struct, Expr<int>
	{
		[MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
		public int Run( ref Registers reg, Span<long> frame, WasmInstance inst ) => (short)default( A ).Run( ref reg, frame, inst );
	}

	// CONVERSIONS
	struct Op_I32_Wrap_I64<A> : Expr<int> where A : struct, Expr<long>
	{
		[MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
		public int Run( ref Registers reg, Span<long> frame, WasmInstance inst ) => (int)default( A ).Run( ref reg, frame, inst );
	}
	struct Op_I32_Truncate_F32_S<A> : Expr<int> where A : struct, Expr<float>
	{
		[MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
		public int Run( ref Registers reg, Span<long> frame, WasmInstance inst ) => checked((int)default( A ).Run( ref reg, frame, inst ));
	}
	struct Op_I32_Truncate_F32_U<A> : Expr<int> where A : struct, Expr<float>
	{
		[MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
		public int Run( ref Registers reg, Span<long> frame, WasmInstance inst ) => (int)checked((uint)default( A ).Run( ref reg, frame, inst ));
	}
	struct Op_I32_Truncate_F64_S<A> : Expr<int> where A : struct, Expr<double>
	{
		[MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
		public int Run( ref Registers reg, Span<long> frame, WasmInstance inst ) => checked((int)default( A ).Run( ref reg, frame, inst ));
	}
	struct Op_I32_Truncate_F64_U<A> : Expr<int> where A : struct, Expr<double>
	{
		[MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
		public int Run( ref Registers reg, Span<long> frame, WasmInstance inst ) => (int)checked((uint)default( A ).Run( ref reg, frame, inst ));
	}
	struct Op_I32_TruncateSat_F32_S<A> : Expr<int> where A : struct, Expr<float>
	{
		[MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
		public int Run( ref Registers reg, Span<long> frame, WasmInstance inst ) => (int)default( A ).Run( ref reg, frame, inst );
	}
	struct Op_I32_TruncateSat_F32_U<A> : Expr<int> where A : struct, Expr<float>
	{
		[MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
		public int Run( ref Registers reg, Span<long> frame, WasmInstance inst ) => (int)(uint)default( A ).Run( ref reg, frame, inst );
	}
	struct Op_I32_TruncateSat_F64_S<A> : Expr<int> where A : struct, Expr<double>
	{
		[MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
		public int Run( ref Registers reg, Span<long> frame, WasmInstance inst ) => (int)default( A ).Run( ref reg, frame, inst );
	}
	struct Op_I32_TruncateSat_F64_U<A> : Expr<int> where A : struct, Expr<double>
	{
		[MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
		public int Run( ref Registers reg, Span<long> frame, WasmInstance inst ) => (int)(uint)default( A ).Run( ref reg, frame, inst );
	}
	struct Op_I32_Reinterpret_F32<A> : Expr<int> where A : struct, Expr<float>
	{
		[MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
		public int Run( ref Registers reg, Span<long> frame, WasmInstance inst ) => BitConverter.SingleToInt32Bits( default( A ).Run( ref reg, frame, inst ) );
	}
}
