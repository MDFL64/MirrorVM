using System;

namespace MirrorVM
{
	public abstract class Destination : Expression
	{
		public Destination( ValType ty ) : base( ty )
		{

		}

		public abstract Type BuildDestination( Type input );
	}

	public enum LocalKind
	{
		// front-end kinds
		Variable, // front-end wasm variables
		Spill,    // spills from the virtual stack
		Call,     // call arguments and extra return values
				  // used in allocation
		Unallocated,
		// back-end kinds
		Register, // locals that live in registers
		Frame,    // locals that live in the frame

		// frame layout:
		// [extra returns from this function] [call arguments and returns] [variables and spills]
	}

	class Local : Destination
	{
		public int Index;
		public LocalKind Kind;

		public Local( int index, ValType ty, LocalKind kind ) : base( ty )
		{
			Index = index;
			Kind = kind;
		}

		public Local WithType( ValType ty )
		{
			return new Local( Index, ty, Kind );
		}

		public override string ToString()
		{
			switch ( Kind )
			{
				case LocalKind.Variable: return "V" + Index;
				case LocalKind.Spill: return "S" + Index;
				case LocalKind.Call: return "C" + Index;

				case LocalKind.Register: return "R" + Index;
				case LocalKind.Frame: return "F" + Index;

				default: return Kind + "_" + Index;
			}
		}

		public override void Traverse( Action<Expression> f )
		{
			f( this );
		}

		public override Type BuildMirror()
		{
			if ( Kind == LocalKind.Register )
			{
				return Type switch
				{
					ValType.I32 => Registers.OpGetI32(Index),
					ValType.I64 => Registers.OpGetI64(Index),
					ValType.F32 => Registers.OpGetF32(Index),
					ValType.F64 => Registers.OpGetF64(Index),

					ValType.ExternRef => Registers.OpGetI64(Index),
					ValType.FuncRef => Registers.OpGetI64(Index),

					_ => throw new Exception( "register-get out of bounds " + Type + " " + Index )
				};
			}
			else if ( Kind == LocalKind.Frame )
			{
				var ty = Type switch
				{
					ValType.I32 => typeof( GetFrame_I32<> ),
					ValType.I64 => typeof( GetFrame_I64<> ),
					ValType.F32 => typeof( GetFrame_F32<> ),
					ValType.F64 => typeof( GetFrame_F64<> ),

					ValType.FuncRef => typeof( GetFrame_I64<> ),
					ValType.ExternRef => typeof( GetFrame_I64<> ),
					_ => throw new Exception( "frame-get " + Type )
				};
				return MirrorBuilder.MakeGeneric( ty, [MirrorBuilder.MakeConstant( Index )] );
			}
			else
			{
				throw new Exception( "can't handle local get: " + Kind );
			}
		}

		public override Type BuildDestination( Type input )
		{
			if ( Kind == LocalKind.Register )
			{
				Type base_ty = Type switch
				{
					ValType.I32 => Registers.OpSetI32(Index),
					ValType.I64 => Registers.OpSetI64(Index),
					ValType.F32 => Registers.OpSetF32(Index),
					ValType.F64 => Registers.OpSetF64(Index),

					ValType.ExternRef => Registers.OpSetI64(Index),
					ValType.FuncRef => Registers.OpSetI64(Index),

					_ => throw new Exception( "register-set out of bounds " + Type + " " + Index )
				};
				return MirrorBuilder.MakeGeneric( base_ty, [input] );
			}
			else if ( Kind == LocalKind.Frame )
			{
				var ty = Type switch
				{
					ValType.I32 => typeof( SetFrame_I32<,> ),
					ValType.I64 => typeof( SetFrame_I64<,> ),
					ValType.F32 => typeof( SetFrame_F32<,> ),
					ValType.F64 => typeof( SetFrame_F64<,> ),

					ValType.FuncRef => typeof( SetFrame_I64<,> ),
					ValType.ExternRef => typeof( SetFrame_I64<,> ),
					_ => throw new Exception( "frame-set " + Type )
				};
				return MirrorBuilder.MakeGeneric( ty, [MirrorBuilder.MakeConstant( Index ), input] );
			}
			else
			{
				throw new Exception( "can't handle local set: " + Kind );
			}
		}
	}

	class Global : Destination
	{
		public int Index;

		public Global( int index, ValType ty ) : base( ty )
		{
			Index = index;
		}

		public override string ToString()
		{
			return "G" + Index;
		}

		public override Type BuildDestination( Type input )
		{
			var ty = Type switch
			{
				ValType.I32 => typeof( SetGlobal_I32<,> ),
				ValType.I64 => typeof( SetGlobal_I64<,> ),
				ValType.F32 => typeof( SetGlobal_F32<,> ),
				ValType.F64 => typeof( SetGlobal_F64<,> ),

				ValType.ExternRef => typeof( SetGlobal_I64<,> ),
				ValType.FuncRef => typeof( SetGlobal_I64<,> ),
				_ => throw new Exception( "global-set " + Type )
			};
			return MirrorBuilder.MakeGeneric( ty, [MirrorBuilder.MakeConstant( Index ), input] );
		}

		public override Type BuildMirror()
		{
			var ty = Type switch
			{
				ValType.I32 => typeof( GetGlobal_I32<> ),
				ValType.I64 => typeof( GetGlobal_I64<> ),
				ValType.F32 => typeof( GetGlobal_F32<> ),
				ValType.F64 => typeof( GetGlobal_F64<> ),

				ValType.ExternRef => typeof( GetGlobal_I64<> ),
				ValType.FuncRef => typeof( GetGlobal_I64<> ),
				_ => throw new Exception( "global-get " + Type )
			};
			return MirrorBuilder.MakeGeneric( ty, [MirrorBuilder.MakeConstant( Index )] );
		}

		public override void Traverse( Action<Expression> f )
		{
			f( this );
		}
	}

	class MemoryOp : Destination
	{
		private MemSize Size;
		private int Offset;
		public Expression Addr;

		public MemoryOp( ValType ty, MemSize size, Expression addr, int offset ) : base( ty )
		{
			Addr = addr;
			Size = size;
			Offset = offset;
		}

		public override Type BuildDestination( Type input )
		{
			Type base_ty = (Type, Size) switch
			{
				(ValType.I32, MemSize.SAME ) => typeof( Memory_I32_Store<,,> ),
				(ValType.I32, MemSize.I8_S ) => typeof( Memory_I32_Store8<,,> ),
				(ValType.I32, MemSize.I16_S ) => typeof( Memory_I32_Store16<,,> ),

				(ValType.I64, MemSize.SAME ) => typeof( Memory_I64_Store<,,> ),
				(ValType.I64, MemSize.I8_S ) => typeof( Memory_I64_Store8<,,> ),
				(ValType.I64, MemSize.I16_S ) => typeof( Memory_I64_Store16<,,> ),
				(ValType.I64, MemSize.I32_S ) => typeof( Memory_I64_Store32<,,> ),

				(ValType.F32, MemSize.SAME ) => typeof( Memory_F32_Store<,,> ),
				(ValType.F64, MemSize.SAME ) => typeof( Memory_F64_Store<,,> ),

				_ => throw new Exception( "WRITE " + Type + " " + Size )
			};
			return MirrorBuilder.MakeGeneric( base_ty, [
				input,
				Addr.BuildMirror(),
				MirrorBuilder.MakeConstant(Offset)
			] );
		}

		public override void Traverse( Action<Expression> f )
		{
			f( this );
			Addr.Traverse( f );
		}

		public override Type BuildMirror()
		{
			Type base_ty = (Type, Size) switch
			{
				(ValType.I32, MemSize.SAME ) => typeof( Memory_I32_Load<,> ),
				(ValType.I32, MemSize.I8_S ) => typeof( Memory_I32_Load8_S<,> ),
				(ValType.I32, MemSize.I8_U ) => typeof( Memory_I32_Load8_U<,> ),
				(ValType.I32, MemSize.I16_S ) => typeof( Memory_I32_Load16_S<,> ),
				(ValType.I32, MemSize.I16_U ) => typeof( Memory_I32_Load16_U<,> ),

				(ValType.I64, MemSize.SAME ) => typeof( Memory_I64_Load<,> ),
				(ValType.I64, MemSize.I8_S ) => typeof( Memory_I64_Load8_S<,> ),
				(ValType.I64, MemSize.I8_U ) => typeof( Memory_I64_Load8_U<,> ),
				(ValType.I64, MemSize.I16_S ) => typeof( Memory_I64_Load16_S<,> ),
				(ValType.I64, MemSize.I16_U ) => typeof( Memory_I64_Load16_U<,> ),
				(ValType.I64, MemSize.I32_S ) => typeof( Memory_I64_Load32_S<,> ),
				(ValType.I64, MemSize.I32_U ) => typeof( Memory_I64_Load32_U<,> ),

				(ValType.F64, MemSize.SAME ) => typeof( Memory_F64_Load<,> ),
				(ValType.F32, MemSize.SAME ) => typeof( Memory_F32_Load<,> ),
				_ => throw new Exception( "READ " + Type + " " + Size )
			};
			return MirrorBuilder.MakeGeneric( base_ty, [
				Addr.BuildMirror(),
				MirrorBuilder.MakeConstant(Offset)
			] );
		}

		public override string ToString()
		{
			string ty_name = Type.ToString();
			if ( Size != MemSize.SAME )
			{
				ty_name += "_" + Size.ToString();
			}
			if ( Offset != 0 )
			{
				return "M_" + ty_name + "[" + Addr.ToString() + " + " + Offset + "]";
			}
			else
			{
				return "M_" + ty_name + "[" + Addr.ToString() + "]";
			}
		}
	}
}
