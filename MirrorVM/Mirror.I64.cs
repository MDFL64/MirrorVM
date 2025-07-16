using System;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace MirrorVM
{
	struct Op_I64_EqualZero<A> : Expr<int> where A : struct, Expr<long>
	{
		[MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
		public int Run( ref Registers reg, Span<long> frame, WasmInstance inst ) => default( A ).Run( ref reg, frame, inst ) == 0 ? 1 : 0;
	}
	struct Op_I64_Equal<A, B> : Expr<int> where A : struct, Expr<long> where B : struct, Expr<long>
	{
		[MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
		public int Run( ref Registers reg, Span<long> frame, WasmInstance inst ) => default( A ).Run( ref reg, frame, inst ) == default( B ).Run( ref reg, frame, inst ) ? 1 : 0;
	}
	struct Op_I64_NotEqual<A, B> : Expr<int> where A : struct, Expr<long> where B : struct, Expr<long>
	{
		[MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
		public int Run( ref Registers reg, Span<long> frame, WasmInstance inst ) => default( A ).Run( ref reg, frame, inst ) != default( B ).Run( ref reg, frame, inst ) ? 1 : 0;
	}
	struct Op_I64_GreaterEqual_S<A, B> : Expr<int> where A : struct, Expr<long> where B : struct, Expr<long>
	{
		[MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
		public int Run( ref Registers reg, Span<long> frame, WasmInstance inst ) => default( A ).Run( ref reg, frame, inst ) >= default( B ).Run( ref reg, frame, inst ) ? 1 : 0;
	}
	struct Op_I64_Greater_S<A, B> : Expr<int> where A : struct, Expr<long> where B : struct, Expr<long>
	{
		[MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
		public int Run( ref Registers reg, Span<long> frame, WasmInstance inst ) => default( A ).Run( ref reg, frame, inst ) > default( B ).Run( ref reg, frame, inst ) ? 1 : 0;
	}
	struct Op_I64_LessEqual_S<A, B> : Expr<int> where A : struct, Expr<long> where B : struct, Expr<long>
	{
		[MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
		public int Run( ref Registers reg, Span<long> frame, WasmInstance inst ) => default( A ).Run( ref reg, frame, inst ) <= default( B ).Run( ref reg, frame, inst ) ? 1 : 0;
	}
	struct Op_I64_Less_S<A, B> : Expr<int> where A : struct, Expr<long> where B : struct, Expr<long>
	{
		[MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
		public int Run( ref Registers reg, Span<long> frame, WasmInstance inst ) => default( A ).Run( ref reg, frame, inst ) < default( B ).Run( ref reg, frame, inst ) ? 1 : 0;
	}
	struct Op_I64_GreaterEqual_U<A, B> : Expr<int> where A : struct, Expr<long> where B : struct, Expr<long>
	{
		[MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
		public int Run( ref Registers reg, Span<long> frame, WasmInstance inst ) => (ulong)default( A ).Run( ref reg, frame, inst ) >= (ulong)default( B ).Run( ref reg, frame, inst ) ? 1 : 0;
	}
	struct Op_I64_Greater_U<A, B> : Expr<int> where A : struct, Expr<long> where B : struct, Expr<long>
	{
		[MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
		public int Run( ref Registers reg, Span<long> frame, WasmInstance inst ) => (ulong)default( A ).Run( ref reg, frame, inst ) > (ulong)default( B ).Run( ref reg, frame, inst ) ? 1 : 0;
	}
	struct Op_I64_LessEqual_U<A, B> : Expr<int> where A : struct, Expr<long> where B : struct, Expr<long>
	{
		[MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
		public int Run( ref Registers reg, Span<long> frame, WasmInstance inst ) => (ulong)default( A ).Run( ref reg, frame, inst ) <= (ulong)default( B ).Run( ref reg, frame, inst ) ? 1 : 0;
	}
	struct Op_I64_Less_U<A, B> : Expr<int> where A : struct, Expr<long> where B : struct, Expr<long>
	{
		[MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
		public int Run( ref Registers reg, Span<long> frame, WasmInstance inst ) => (ulong)default( A ).Run( ref reg, frame, inst ) < (ulong)default( B ).Run( ref reg, frame, inst ) ? 1 : 0;
	}
	// END COMPARISONS

	struct Const_I64<C> : Expr<long>
		where C : struct, Const
	{
		[MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
		public long Run( ref Registers reg, Span<long> frame, WasmInstance inst ) => default( C ).Run();
	}
	struct Op_I64_Add<A, B> : Expr<long> where A : struct, Expr<long> where B : struct, Expr<long>
	{
		[MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
		public long Run( ref Registers reg, Span<long> frame, WasmInstance inst ) => default( A ).Run( ref reg, frame, inst ) + default( B ).Run( ref reg, frame, inst );
	}
	struct Op_I64_Sub<A, B> : Expr<long> where A : struct, Expr<long> where B : struct, Expr<long>
	{
		[MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
		public long Run( ref Registers reg, Span<long> frame, WasmInstance inst ) => default( A ).Run( ref reg, frame, inst ) - default( B ).Run( ref reg, frame, inst );
	}
	struct Op_I64_Mul<A, B> : Expr<long> where A : struct, Expr<long> where B : struct, Expr<long>
	{
		[MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
		public long Run( ref Registers reg, Span<long> frame, WasmInstance inst ) => default( A ).Run( ref reg, frame, inst ) * default( B ).Run( ref reg, frame, inst );
	}
	struct Op_I64_Div_S<A, B> : Expr<long> where A : struct, Expr<long> where B : struct, Expr<long>
	{
		[MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
		public long Run( ref Registers reg, Span<long> frame, WasmInstance inst ) => default( A ).Run( ref reg, frame, inst ) / default( B ).Run( ref reg, frame, inst );
	}
	struct Op_I64_Div_U<A, B> : Expr<long> where A : struct, Expr<long> where B : struct, Expr<long>
	{
		[MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
		public long Run( ref Registers reg, Span<long> frame, WasmInstance inst ) => (long)((ulong)default( A ).Run( ref reg, frame, inst ) / (ulong)default( B ).Run( ref reg, frame, inst ));
	}
	struct Op_I64_Rem_S<A, B> : Expr<long> where A : struct, Expr<long> where B : struct, Expr<long>
	{
		[MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
		public long Run( ref Registers reg, Span<long> frame, WasmInstance inst )
		{
			long a = default( A ).Run( ref reg, frame, inst );
			long b = default( B ).Run( ref reg, frame, inst );
			// I tried to find a better way -- casting to longs is slower
			if ( a == -9223372036854775808 && b == -1 )
			{
				return 0;
			}
			else
			{
				return a % b;
			}
		}
	}
	struct Op_I64_Rem_U<A, B> : Expr<long> where A : struct, Expr<long> where B : struct, Expr<long>
	{
		[MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
		public long Run( ref Registers reg, Span<long> frame, WasmInstance inst ) => (long)((ulong)default( A ).Run( ref reg, frame, inst ) % (ulong)default( B ).Run( ref reg, frame, inst ));
	}
	struct Op_I64_And<A, B> : Expr<long> where A : struct, Expr<long> where B : struct, Expr<long>
	{
		[MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
		public long Run( ref Registers reg, Span<long> frame, WasmInstance inst ) => default( A ).Run( ref reg, frame, inst ) & default( B ).Run( ref reg, frame, inst );
	}
	struct Op_I64_Or<A, B> : Expr<long> where A : struct, Expr<long> where B : struct, Expr<long>
	{
		[MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
		public long Run( ref Registers reg, Span<long> frame, WasmInstance inst ) => default( A ).Run( ref reg, frame, inst ) | default( B ).Run( ref reg, frame, inst );
	}
	struct Op_I64_Xor<A, B> : Expr<long> where A : struct, Expr<long> where B : struct, Expr<long>
	{
		[MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
		public long Run( ref Registers reg, Span<long> frame, WasmInstance inst ) => default( A ).Run( ref reg, frame, inst ) ^ default( B ).Run( ref reg, frame, inst );
	}
	struct Op_I64_ShiftLeft<A, B> : Expr<long> where A : struct, Expr<long> where B : struct, Expr<long>
	{
		[MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
		public long Run( ref Registers reg, Span<long> frame, WasmInstance inst ) => default( A ).Run( ref reg, frame, inst ) << (int)default( B ).Run( ref reg, frame, inst );
	}
	struct Op_I64_ShiftRight_S<A, B> : Expr<long> where A : struct, Expr<long> where B : struct, Expr<long>
	{
		[MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
		public long Run( ref Registers reg, Span<long> frame, WasmInstance inst ) => default( A ).Run( ref reg, frame, inst ) >> (int)default( B ).Run( ref reg, frame, inst );
	}
	struct Op_I64_ShiftRight_U<A, B> : Expr<long> where A : struct, Expr<long> where B : struct, Expr<long>
	{
		[MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
		public long Run( ref Registers reg, Span<long> frame, WasmInstance inst ) => default( A ).Run( ref reg, frame, inst ) >>> (int)default( B ).Run( ref reg, frame, inst );
	}
	struct Op_I64_RotateLeft<A, B> : Expr<long> where A : struct, Expr<long> where B : struct, Expr<long>
	{
		[MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
		public long Run( ref Registers reg, Span<long> frame, WasmInstance inst ) => (long)BitOperations.RotateLeft( (ulong)default( A ).Run( ref reg, frame, inst ), (int)default( B ).Run( ref reg, frame, inst ) );
	}
	struct Op_I64_RotateRight<A, B> : Expr<long> where A : struct, Expr<long> where B : struct, Expr<long>
	{
		[MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
		public long Run( ref Registers reg, Span<long> frame, WasmInstance inst ) => (long)BitOperations.RotateRight( (ulong)default( A ).Run( ref reg, frame, inst ), (int)default( B ).Run( ref reg, frame, inst ) );
	}
	struct Op_I64_LeadingZeros<A> : Expr<long> where A : struct, Expr<long>
	{
		[MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
		public long Run( ref Registers reg, Span<long> frame, WasmInstance inst ) => BitOperations.LeadingZeroCount( (ulong)default( A ).Run( ref reg, frame, inst ) );
	}
	struct Op_I64_TrailingZeros<A> : Expr<long> where A : struct, Expr<long>
	{
		[MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
		public long Run( ref Registers reg, Span<long> frame, WasmInstance inst ) => BitOperations.TrailingZeroCount( (ulong)default( A ).Run( ref reg, frame, inst ) );
	}
	struct Op_I64_PopCount<A> : Expr<long> where A : struct, Expr<long>
	{
		[MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
		public long Run( ref Registers reg, Span<long> frame, WasmInstance inst ) => BitOperations.PopCount( (ulong)default( A ).Run( ref reg, frame, inst ) );
	}
	struct Op_I64_Extend8_S<A> : Expr<long> where A : struct, Expr<long>
	{
		[MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
		public long Run( ref Registers reg, Span<long> frame, WasmInstance inst ) => (sbyte)default( A ).Run( ref reg, frame, inst );
	}
	struct Op_I64_Extend16_S<A> : Expr<long> where A : struct, Expr<long>
	{
		[MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
		public long Run( ref Registers reg, Span<long> frame, WasmInstance inst ) => (short)default( A ).Run( ref reg, frame, inst );
	}
	struct Op_I64_Extend32_S<A> : Expr<long> where A : struct, Expr<long>
	{
		[MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
		public long Run( ref Registers reg, Span<long> frame, WasmInstance inst ) => (int)default( A ).Run( ref reg, frame, inst );
	}

	// CONVERSIONS
	struct Op_I64_Extend_I32_S<A> : Expr<long> where A : struct, Expr<int>
	{
		[MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
		public long Run( ref Registers reg, Span<long> frame, WasmInstance inst ) => default( A ).Run( ref reg, frame, inst );
	}
	struct Op_I64_Extend_I32_U<A> : Expr<long> where A : struct, Expr<int>
	{
		[MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
		public long Run( ref Registers reg, Span<long> frame, WasmInstance inst ) => (uint)default( A ).Run( ref reg, frame, inst );
	}
	struct Op_I64_Truncate_F32_S<A> : Expr<long> where A : struct, Expr<float>
	{
		[MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
		public long Run( ref Registers reg, Span<long> frame, WasmInstance inst ) => checked((long)default( A ).Run( ref reg, frame, inst ));
	}
	struct Op_I64_Truncate_F32_U<A> : Expr<long> where A : struct, Expr<float>
	{
		[MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
		public long Run( ref Registers reg, Span<long> frame, WasmInstance inst ) => (long)checked((ulong)default( A ).Run( ref reg, frame, inst ));
	}
	struct Op_I64_Truncate_F64_S<A> : Expr<long> where A : struct, Expr<double>
	{
		[MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
		public long Run( ref Registers reg, Span<long> frame, WasmInstance inst ) => checked((long)default( A ).Run( ref reg, frame, inst ));
	}
	struct Op_I64_Truncate_F64_U<A> : Expr<long> where A : struct, Expr<double>
	{
		[MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
		public long Run( ref Registers reg, Span<long> frame, WasmInstance inst ) => (long)checked((ulong)default( A ).Run( ref reg, frame, inst ));
	}
	struct Op_I64_TruncateSat_F32_S<A> : Expr<long> where A : struct, Expr<float>
	{
		[MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
		public long Run( ref Registers reg, Span<long> frame, WasmInstance inst ) => (long)default( A ).Run( ref reg, frame, inst );
	}
	struct Op_I64_TruncateSat_F32_U<A> : Expr<long> where A : struct, Expr<float>
	{
		[MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
		public long Run( ref Registers reg, Span<long> frame, WasmInstance inst ) => (long)(ulong)default( A ).Run( ref reg, frame, inst );
	}
	struct Op_I64_TruncateSat_F64_S<A> : Expr<long> where A : struct, Expr<double>
	{
		[MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
		public long Run( ref Registers reg, Span<long> frame, WasmInstance inst ) => (long)default( A ).Run( ref reg, frame, inst );
	}
	struct Op_I64_TruncateSat_F64_U<A> : Expr<long> where A : struct, Expr<double>
	{
		[MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
		public long Run( ref Registers reg, Span<long> frame, WasmInstance inst ) => (long)(ulong)default( A ).Run( ref reg, frame, inst );
	}
	struct Op_I64_Reinterpret_F64<A> : Expr<long> where A : struct, Expr<double>
	{
		[MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
		public long Run( ref Registers reg, Span<long> frame, WasmInstance inst ) => BitConverter.DoubleToInt64Bits( default( A ).Run( ref reg, frame, inst ) );
	}
}
