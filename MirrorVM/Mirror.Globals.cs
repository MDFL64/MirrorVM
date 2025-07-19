using System;
using System.Runtime.CompilerServices;

namespace MirrorVM
{
	struct GetGlobal_I32<INDEX> : Expr<int> where INDEX : struct, Const
	{
		[MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
		public int Run( ref Registers reg, Span<long> frame, WasmInstance inst ) =>
			(int)inst.Globals[(int)default( INDEX ).Run()];
	}

	struct GetGlobal_I64<INDEX> : Expr<long> where INDEX : struct, Const
	{
		[MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
		public long Run( ref Registers reg, Span<long> frame, WasmInstance inst ) =>
			inst.Globals[(int)default( INDEX ).Run()];
	}

	struct GetGlobal_F32<INDEX> : Expr<float> where INDEX : struct, Const
	{
		[MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
		public float Run( ref Registers reg, Span<long> frame, WasmInstance inst ) =>
			BitConverter.Int32BitsToSingle( (int)inst.Globals[(int)default( INDEX ).Run()] );
	}

	struct GetGlobal_F64<INDEX> : Expr<double> where INDEX : struct, Const
	{
		[MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
		public double Run( ref Registers reg, Span<long> frame, WasmInstance inst ) =>
			BitConverter.Int64BitsToDouble( inst.Globals[(int)default( INDEX ).Run()] );
	}

	// setters

	struct SetGlobal_I32<INDEX, VALUE> : Stmt where INDEX : struct, Const where VALUE : struct, Expr<int>
	{
		[MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
		public void Run( ref Registers reg, Span<long> frame, WasmInstance inst )
		{
			inst.Globals[(int)default( INDEX ).Run()] = (uint)default( VALUE ).Run( ref reg, frame, inst );
		}
	}

	struct SetGlobal_I64<INDEX, VALUE> : Stmt where INDEX : struct, Const where VALUE : struct, Expr<long>
	{
		[MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
		public void Run( ref Registers reg, Span<long> frame, WasmInstance inst )
		{
			inst.Globals[(int)default( INDEX ).Run()] = default( VALUE ).Run( ref reg, frame, inst );
		}
	}

	struct SetGlobal_F32<INDEX, VALUE> : Stmt where INDEX : struct, Const where VALUE : struct, Expr<float>
	{
		[MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
		public void Run( ref Registers reg, Span<long> frame, WasmInstance inst )
		{
			inst.Globals[(int)default( INDEX ).Run()] = BitConverter.SingleToUInt32Bits( default( VALUE ).Run( ref reg, frame, inst ) );
		}
	}

	struct SetGlobal_F64<INDEX, VALUE> : Stmt where INDEX : struct, Const where VALUE : struct, Expr<double>
	{
		[MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
		public void Run( ref Registers reg, Span<long> frame, WasmInstance inst )
		{
			inst.Globals[(int)default( INDEX ).Run()] = BitConverter.DoubleToInt64Bits( default( VALUE ).Run( ref reg, frame, inst ) );
		}
	}
}
