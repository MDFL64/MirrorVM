using System;
using System.Runtime.CompilerServices;

namespace MirrorVM
{
	struct StaticCall<FUNC_INDEX, FRAME_INDEX> : Stmt
		where FUNC_INDEX : struct, Const
		where FRAME_INDEX : struct, Const
	{
		[MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
		public void Run( ref Registers reg, Span<long> frame, WasmInstance inst )
		{
			var arg_span = frame.Slice( (int)default( FRAME_INDEX ).Run() );
			var func = inst.Functions[default( FUNC_INDEX ).Run()];
			func.Call( arg_span, inst );
		}
	}

	struct DynamicCall<FUNC_INDEX, TABLE_INDEX, FRAME_INDEX, SIG_ID> : Stmt
		where FUNC_INDEX : struct, Expr<int>
		where TABLE_INDEX : struct, Const
		where FRAME_INDEX : struct, Const
		where SIG_ID : struct, Const
	{
		[MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
		public void Run( ref Registers reg, Span<long> frame, WasmInstance inst )
		{
			var arg_span = frame.Slice( (int)default( FRAME_INDEX ).Run() );
			int func_index = default( FUNC_INDEX ).Run( ref reg, frame, inst );
			int table_index = (int)default( TABLE_INDEX ).Run();
			var pair = inst.DynamicCallTable[table_index][func_index];
			int expected_sig_id = (int)default( SIG_ID ).Run();
			if ( pair.SigId != expected_sig_id )
			{
				throw new Exception( "dynamic call type error: " + pair.SigId + " != " + expected_sig_id + " / " + table_index + ", " + func_index );
			}
			pair.Callable.Call( arg_span, inst );
		}
	}

	interface ArgRead
	{
		void Run( Span<long> args, ref Registers reg, Span<long> frame );
	}

	struct ArgReadNone : ArgRead
	{
		[MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
		public void Run( Span<long> args, ref Registers reg, Span<long> frame ) { }
	}

	struct ArgReadR0<INDEX, NEXT> : ArgRead where INDEX : struct, Const where NEXT : struct, ArgRead
	{
		[MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
		public void Run( Span<long> args, ref Registers reg, Span<long> frame )
		{
			reg.R0 = args[(int)default( INDEX ).Run()];
			default( NEXT ).Run( args, ref reg, frame );
		}
	}

	struct ArgReadR1<INDEX, NEXT> : ArgRead where INDEX : struct, Const where NEXT : struct, ArgRead
	{
		[MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
		public void Run( Span<long> args, ref Registers reg, Span<long> frame )
		{
			reg.R1 = args[(int)default( INDEX ).Run()];
			default( NEXT ).Run( args, ref reg, frame );
		}
	}

	struct ArgReadR2<INDEX, NEXT> : ArgRead where INDEX : struct, Const where NEXT : struct, ArgRead
	{
		[MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
		public void Run( Span<long> args, ref Registers reg, Span<long> frame )
		{
			reg.R2 = args[(int)default( INDEX ).Run()];
			default( NEXT ).Run( args, ref reg, frame );
		}
	}

	struct ArgReadR3<INDEX, NEXT> : ArgRead where INDEX : struct, Const where NEXT : struct, ArgRead
	{
		[MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
		public void Run( Span<long> args, ref Registers reg, Span<long> frame )
		{
			reg.R3 = args[(int)default( INDEX ).Run()];
			default( NEXT ).Run( args, ref reg, frame );
		}
	}

	struct ArgReadR4<INDEX, NEXT> : ArgRead where INDEX : struct, Const where NEXT : struct, ArgRead
	{
		[MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
		public void Run( Span<long> args, ref Registers reg, Span<long> frame )
		{
			reg.R4 = args[(int)default( INDEX ).Run()];
			default( NEXT ).Run( args, ref reg, frame );
		}
	}

	struct ArgReadR5<INDEX, NEXT> : ArgRead where INDEX : struct, Const where NEXT : struct, ArgRead
	{
		[MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
		public void Run( Span<long> args, ref Registers reg, Span<long> frame )
		{
			reg.R5 = args[(int)default( INDEX ).Run()];
			default( NEXT ).Run( args, ref reg, frame );
		}
	}

	struct ArgReadR6<INDEX, NEXT> : ArgRead where INDEX : struct, Const where NEXT : struct, ArgRead
	{
		[MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
		public void Run( Span<long> args, ref Registers reg, Span<long> frame )
		{
			reg.R6 = args[(int)default( INDEX ).Run()];
			default( NEXT ).Run( args, ref reg, frame );
		}
	}

	struct ArgReadFrame<INDEX_ARG, INDEX_FRAME, NEXT> : ArgRead
		where INDEX_ARG : struct, Const
		where INDEX_FRAME : struct, Const
		where NEXT : struct, ArgRead
	{
		[MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
		public void Run( Span<long> args, ref Registers reg, Span<long> frame )
		{
			frame[(int)default( INDEX_FRAME ).Run()] = args[(int)default( INDEX_ARG ).Run()];
			default( NEXT ).Run( args, ref reg, frame );
		}
	}

	interface ArgWrite
	{
		void Run( Registers reg, Span<long> frame, WasmInstance inst );
	}

	struct ArgWriteNone : ArgWrite
	{
		public void Run( Registers reg, Span<long> frame, WasmInstance inst ) { }
	}

	struct ArgWriteI32<EXPR, INDEX, NEXT> : ArgWrite
		where EXPR : struct, Expr<int>
		where INDEX : struct, Const
		where NEXT : struct, ArgWrite
	{
		[MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
		public void Run( Registers reg, Span<long> frame, WasmInstance inst )
		{
			frame[(int)default( INDEX ).Run()] = default( EXPR ).Run( ref reg, frame, inst );
			default( NEXT ).Run( reg, frame, inst );
		}
	}

	struct ArgWriteI64<EXPR, INDEX, NEXT> : ArgWrite
		where EXPR : struct, Expr<long>
		where INDEX : struct, Const
		where NEXT : struct, ArgWrite
	{
		[MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
		public void Run( Registers reg, Span<long> frame, WasmInstance inst )
		{
			frame[(int)default( INDEX ).Run()] = default( EXPR ).Run( ref reg, frame, inst );
			default( NEXT ).Run( reg, frame, inst );
		}
	}

	struct ArgWriteF32<EXPR, INDEX, NEXT> : ArgWrite
		where EXPR : struct, Expr<float>
		where INDEX : struct, Const
		where NEXT : struct, ArgWrite
	{
		[MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
		public void Run( Registers reg, Span<long> frame, WasmInstance inst )
		{
			frame[(int)default( INDEX ).Run()] = BitConverter.SingleToUInt32Bits( default( EXPR ).Run( ref reg, frame, inst ) );
			default( NEXT ).Run( reg, frame, inst );
		}
	}

	struct ArgWriteF64<EXPR, INDEX, NEXT> : ArgWrite
		where EXPR : struct, Expr<double>
		where INDEX : struct, Const
		where NEXT : struct, ArgWrite
	{
		[MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
		public void Run( Registers reg, Span<long> frame, WasmInstance inst )
		{
			frame[(int)default( INDEX ).Run()] = BitConverter.DoubleToInt64Bits( default( EXPR ).Run( ref reg, frame, inst ) );
			default( NEXT ).Run( reg, frame, inst );
		}
	}
}
