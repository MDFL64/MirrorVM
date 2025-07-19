
using System;
using System.Runtime.CompilerServices;

namespace MirrorVM
{
	struct Memory_GetSize : Expr<int>
	{
		[MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
		public int Run( ref Registers reg, Span<long> frame, WasmInstance inst ) => inst.Memory.Length / 65536;
	}

	struct Memory_Grow<ARG> : Expr<int> where ARG : struct, Expr<int>
	{
		[MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
		public int Run( ref Registers reg, Span<long> frame, WasmInstance inst )
		{
			int old_size = inst.Memory.Length / 65536;
			int page_count = default( ARG ).Run( ref reg, frame, inst );
			if ( page_count < 0 || page_count + old_size > inst.MemoryPageLimit )
			{
				return -1;
			}
			inst.GrowMemory( page_count );
			return old_size;
		}
	}

	// operations that work on individual bytes are written slightly differently,
	// since indexing actually works with longs, but the AsSpan method does not

	// i32 loads
	struct Memory_I32_Load<ADDR, OFFSET> : Expr<int> where ADDR : struct, Expr<int> where OFFSET : struct, Const
	{
		[MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
		public int Run( ref Registers reg, Span<long> frame, WasmInstance inst )
		{
			uint addr = (uint)default( ADDR ).Run( ref reg, frame, inst );
			uint offset = (uint)default( OFFSET ).Run();
			return BitConverter.ToInt32( inst.Memory.AsSpan( (int)checked(addr + offset) ) );
		}
	}
	struct Memory_I32_Load8_S<ADDR, OFFSET> : Expr<int> where ADDR : struct, Expr<int> where OFFSET : struct, Const
	{
		[MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
		public int Run( ref Registers reg, Span<long> frame, WasmInstance inst )
		{
			long addr = (uint)default( ADDR ).Run( ref reg, frame, inst );
			long offset = (uint)default( OFFSET ).Run();
			return (sbyte)inst.Memory[addr + offset];
		}
	}
	struct Memory_I32_Load8_U<ADDR, OFFSET> : Expr<int> where ADDR : struct, Expr<int> where OFFSET : struct, Const
	{
		[MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
		public int Run( ref Registers reg, Span<long> frame, WasmInstance inst )
		{
			long addr = (uint)default( ADDR ).Run( ref reg, frame, inst );
			long offset = (uint)default( OFFSET ).Run();
			return inst.Memory[addr + offset];
		}
	}
	struct Memory_I32_Load16_S<ADDR, OFFSET> : Expr<int> where ADDR : struct, Expr<int> where OFFSET : struct, Const
	{
		[MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
		public int Run( ref Registers reg, Span<long> frame, WasmInstance inst )
		{
			uint addr = (uint)default( ADDR ).Run( ref reg, frame, inst );
			uint offset = (uint)default( OFFSET ).Run();
			return BitConverter.ToInt16( inst.Memory.AsSpan( (int)checked(addr + offset) ) );
		}
	}
	struct Memory_I32_Load16_U<ADDR, OFFSET> : Expr<int> where ADDR : struct, Expr<int> where OFFSET : struct, Const
	{
		[MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
		public int Run( ref Registers reg, Span<long> frame, WasmInstance inst )
		{
			uint addr = (uint)default( ADDR ).Run( ref reg, frame, inst );
			uint offset = (uint)default( OFFSET ).Run();
			return BitConverter.ToUInt16( inst.Memory.AsSpan( (int)checked(addr + offset) ) );
		}
	}

	// i64 loads
	struct Memory_I64_Load<ADDR, OFFSET> : Expr<long> where ADDR : struct, Expr<int> where OFFSET : struct, Const
	{
		[MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
		public long Run( ref Registers reg, Span<long> frame, WasmInstance inst )
		{
			uint addr = (uint)default( ADDR ).Run( ref reg, frame, inst );
			uint offset = (uint)default( OFFSET ).Run();
			return BitConverter.ToInt64( inst.Memory.AsSpan( (int)checked(addr + offset) ) );
		}
	}
	struct Memory_I64_Load8_S<ADDR, OFFSET> : Expr<long> where ADDR : struct, Expr<int> where OFFSET : struct, Const
	{
		[MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
		public long Run( ref Registers reg, Span<long> frame, WasmInstance inst )
		{
			long addr = (uint)default( ADDR ).Run( ref reg, frame, inst );
			long offset = (uint)default( OFFSET ).Run();
			return (sbyte)inst.Memory[addr + offset];
		}
	}
	struct Memory_I64_Load8_U<ADDR, OFFSET> : Expr<long> where ADDR : struct, Expr<int> where OFFSET : struct, Const
	{
		[MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
		public long Run( ref Registers reg, Span<long> frame, WasmInstance inst )
		{
			long addr = (uint)default( ADDR ).Run( ref reg, frame, inst );
			long offset = (uint)default( OFFSET ).Run();
			return inst.Memory[addr + offset];
		}
	}
	struct Memory_I64_Load16_S<ADDR, OFFSET> : Expr<long> where ADDR : struct, Expr<int> where OFFSET : struct, Const
	{
		[MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
		public long Run( ref Registers reg, Span<long> frame, WasmInstance inst )
		{
			uint addr = (uint)default( ADDR ).Run( ref reg, frame, inst );
			uint offset = (uint)default( OFFSET ).Run();
			return BitConverter.ToInt16( inst.Memory.AsSpan( (int)checked(addr + offset) ) );
		}
	}
	struct Memory_I64_Load16_U<ADDR, OFFSET> : Expr<long> where ADDR : struct, Expr<int> where OFFSET : struct, Const
	{
		[MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
		public long Run( ref Registers reg, Span<long> frame, WasmInstance inst )
		{
			uint addr = (uint)default( ADDR ).Run( ref reg, frame, inst );
			uint offset = (uint)default( OFFSET ).Run();
			return BitConverter.ToUInt16( inst.Memory.AsSpan( (int)checked(addr + offset) ) );
		}
	}
	struct Memory_I64_Load32_S<ADDR, OFFSET> : Expr<long> where ADDR : struct, Expr<int> where OFFSET : struct, Const
	{
		[MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
		public long Run( ref Registers reg, Span<long> frame, WasmInstance inst )
		{
			uint addr = (uint)default( ADDR ).Run( ref reg, frame, inst );
			uint offset = (uint)default( OFFSET ).Run();
			return BitConverter.ToInt32( inst.Memory.AsSpan( (int)checked(addr + offset) ) );
		}
	}
	struct Memory_I64_Load32_U<ADDR, OFFSET> : Expr<long> where ADDR : struct, Expr<int> where OFFSET : struct, Const
	{
		[MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
		public long Run( ref Registers reg, Span<long> frame, WasmInstance inst )
		{
			uint addr = (uint)default( ADDR ).Run( ref reg, frame, inst );
			uint offset = (uint)default( OFFSET ).Run();
			return BitConverter.ToUInt32( inst.Memory.AsSpan( (int)checked(addr + offset) ) );
		}
	}

	struct Memory_F64_Load<ADDR, OFFSET> : Expr<double> where ADDR : struct, Expr<int> where OFFSET : struct, Const
	{
		[MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
		public double Run( ref Registers reg, Span<long> frame, WasmInstance inst )
		{
			uint addr = (uint)default( ADDR ).Run( ref reg, frame, inst );
			uint offset = (uint)default( OFFSET ).Run();
			return BitConverter.ToDouble( inst.Memory.AsSpan( (int)checked(addr + offset) ) );
		}
	}

	struct Memory_F32_Load<ADDR, OFFSET> : Expr<float> where ADDR : struct, Expr<int> where OFFSET : struct, Const
	{
		[MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
		public float Run( ref Registers reg, Span<long> frame, WasmInstance inst )
		{
			uint addr = (uint)default( ADDR ).Run( ref reg, frame, inst );
			uint offset = (uint)default( OFFSET ).Run();
			return BitConverter.ToSingle( inst.Memory.AsSpan( (int)checked(addr + offset) ) );
		}
	}

	// store

	// i32 stores
	struct Memory_I32_Store<VALUE, ADDR, OFFSET> : Stmt
		where VALUE : struct, Expr<int>
		where ADDR : struct, Expr<int>
		where OFFSET : struct, Const
	{
		[MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
		public void Run( ref Registers reg, Span<long> frame, WasmInstance inst )
		{
			int value = default( VALUE ).Run( ref reg, frame, inst );
			uint addr = (uint)default( ADDR ).Run( ref reg, frame, inst );
			uint offset = (uint)default( OFFSET ).Run();
			if ( !BitConverter.TryWriteBytes( inst.Memory.AsSpan( (int)checked(addr + offset) ), value ) )
			{
				throw new IndexOutOfRangeException();
			}
		}
	}
	struct Memory_I32_Store8<VALUE, ADDR, OFFSET> : Stmt
		where VALUE : struct, Expr<int>
		where ADDR : struct, Expr<int>
		where OFFSET : struct, Const
	{
		[MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
		public void Run( ref Registers reg, Span<long> frame, WasmInstance inst )
		{
			byte value = (byte)default( VALUE ).Run( ref reg, frame, inst );
			long addr = (uint)default( ADDR ).Run( ref reg, frame, inst );
			long offset = (uint)default( OFFSET ).Run();
			inst.Memory[addr + offset] = value;
		}
	}
	struct Memory_I32_Store16<VALUE, ADDR, OFFSET> : Stmt
		where VALUE : struct, Expr<int>
		where ADDR : struct, Expr<int>
		where OFFSET : struct, Const
	{
		[MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
		public void Run( ref Registers reg, Span<long> frame, WasmInstance inst )
		{
			short value = (short)default( VALUE ).Run( ref reg, frame, inst );
			uint addr = (uint)default( ADDR ).Run( ref reg, frame, inst );
			uint offset = (uint)default( OFFSET ).Run();
			if ( !BitConverter.TryWriteBytes( inst.Memory.AsSpan( (int)checked(addr + offset) ), value ) )
			{
				throw new IndexOutOfRangeException();
			}
		}
	}

	// i64 stores
	struct Memory_I64_Store<VALUE, ADDR, OFFSET> : Stmt
		where VALUE : struct, Expr<long>
		where ADDR : struct, Expr<int>
		where OFFSET : struct, Const
	{
		[MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
		public void Run( ref Registers reg, Span<long> frame, WasmInstance inst )
		{
			long value = default( VALUE ).Run( ref reg, frame, inst );
			uint addr = (uint)default( ADDR ).Run( ref reg, frame, inst );
			uint offset = (uint)default( OFFSET ).Run();
			if ( !BitConverter.TryWriteBytes( inst.Memory.AsSpan( (int)checked(addr + offset) ), value ) )
			{
				throw new IndexOutOfRangeException();
			}
		}
	}
	struct Memory_I64_Store8<VALUE, ADDR, OFFSET> : Stmt
		where VALUE : struct, Expr<long>
		where ADDR : struct, Expr<int>
		where OFFSET : struct, Const
	{
		[MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
		public void Run( ref Registers reg, Span<long> frame, WasmInstance inst )
		{
			byte value = (byte)default( VALUE ).Run( ref reg, frame, inst );
			long addr = (uint)default( ADDR ).Run( ref reg, frame, inst );
			long offset = (uint)default( OFFSET ).Run();
			inst.Memory[addr + offset] = value;
		}
	}
	struct Memory_I64_Store16<VALUE, ADDR, OFFSET> : Stmt
		where VALUE : struct, Expr<long>
		where ADDR : struct, Expr<int>
		where OFFSET : struct, Const
	{
		[MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
		public void Run( ref Registers reg, Span<long> frame, WasmInstance inst )
		{
			short value = (short)default( VALUE ).Run( ref reg, frame, inst );
			uint addr = (uint)default( ADDR ).Run( ref reg, frame, inst );
			uint offset = (uint)default( OFFSET ).Run();
			if ( !BitConverter.TryWriteBytes( inst.Memory.AsSpan( (int)checked(addr + offset) ), value ) )
			{
				throw new IndexOutOfRangeException();
			}
		}
	}
	struct Memory_I64_Store32<VALUE, ADDR, OFFSET> : Stmt
		where VALUE : struct, Expr<long>
		where ADDR : struct, Expr<int>
		where OFFSET : struct, Const
	{
		[MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
		public void Run( ref Registers reg, Span<long> frame, WasmInstance inst )
		{
			int value = (int)default( VALUE ).Run( ref reg, frame, inst );
			uint addr = (uint)default( ADDR ).Run( ref reg, frame, inst );
			uint offset = (uint)default( OFFSET ).Run();
			if ( !BitConverter.TryWriteBytes( inst.Memory.AsSpan( (int)checked(addr + offset) ), value ) )
			{
				throw new IndexOutOfRangeException();
			}
		}
	}

	// float stores
	struct Memory_F32_Store<VALUE, ADDR, OFFSET> : Stmt
		where VALUE : struct, Expr<float>
		where ADDR : struct, Expr<int>
		where OFFSET : struct, Const
	{
		[MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
		public void Run( ref Registers reg, Span<long> frame, WasmInstance inst )
		{
			float value = default( VALUE ).Run( ref reg, frame, inst );
			uint addr = (uint)default( ADDR ).Run( ref reg, frame, inst );
			uint offset = (uint)default( OFFSET ).Run();
			if ( !BitConverter.TryWriteBytes( inst.Memory.AsSpan( (int)checked(addr + offset) ), value ) )
			{
				throw new IndexOutOfRangeException();
			}
		}
	}
	struct Memory_F64_Store<VALUE, ADDR, OFFSET> : Stmt
		where VALUE : struct, Expr<double>
		where ADDR : struct, Expr<int>
		where OFFSET : struct, Const
	{
		[MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
		public void Run( ref Registers reg, Span<long> frame, WasmInstance inst )
		{
			double value = default( VALUE ).Run( ref reg, frame, inst );
			uint addr = (uint)default( ADDR ).Run( ref reg, frame, inst );
			uint offset = (uint)default( OFFSET ).Run();
			if ( !BitConverter.TryWriteBytes( inst.Memory.AsSpan( (int)checked(addr + offset) ), value ) )
			{
				throw new IndexOutOfRangeException();
			}
		}
	}

	struct Memory_Fill<LEN, VAL, PTR> : Stmt
		where LEN : struct, Expr<int>
		where VAL : struct, Expr<int>
		where PTR : struct, Expr<int>
	{
		[MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
		public void Run( ref Registers reg, Span<long> frame, WasmInstance inst )
		{
			int len = default( LEN ).Run( ref reg, frame, inst );
			byte val = (byte)default( VAL ).Run( ref reg, frame, inst );
			int ptr = default( PTR ).Run( ref reg, frame, inst );
			//Log.Info( "set " + ptr + " = " + val + " x " + len );
			
			for (int i=0;i<len;i++)
			{
				inst.Memory[ptr + i] = val;
			}
		}
	}

	struct Memory_Copy<LEN, SRC, DST> : Stmt
		where LEN : struct, Expr<int>
		where SRC : struct, Expr<int>
		where DST : struct, Expr<int>
	{
		[MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
		public void Run( ref Registers reg, Span<long> frame, WasmInstance inst )
		{
			int len = default( LEN ).Run( ref reg, frame, inst );
			int src = default( SRC ).Run( ref reg, frame, inst );
			int dst = default( DST ).Run( ref reg, frame, inst );

			//Log.Info( "copy " + dst + " = " + src + " x " + len );

			// need to deal with overlapping copies
			if (dst < src)
			{
				for (int i = 0; i < len; i++)
				{
					inst.Memory[dst + i] = inst.Memory[src + i];
				}
			} else
			{
				for (int i = len - 1; i >= 0; i--)
				{
					inst.Memory[dst + i] = inst.Memory[src + i];
				}
			}
		}
	}
}
