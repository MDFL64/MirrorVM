using System;
using System.Runtime.CompilerServices;

namespace MirrorVM
{
	interface Const
	{
		long Run();
	}

	interface Expr<T>
	{
		T Run( ref Registers reg, Span<long> frame, WasmInstance inst );
	}

	interface Stmt
	{
		void Run( ref Registers reg, Span<long> frame, WasmInstance inst );
	}

	interface Terminator
	{
		void Run( ref Registers reg, Span<long> frame, WasmInstance inst );
	}

	struct D0 : Const { public long Run() => 0; }
	struct D1 : Const { public long Run() => 1; }
	struct D2 : Const { public long Run() => 2; }
	struct D3 : Const { public long Run() => 3; }
	struct D4 : Const { public long Run() => 4; }
	struct D5 : Const { public long Run() => 5; }
	struct D6 : Const { public long Run() => 6; }
	struct D7 : Const { public long Run() => 7; }
	struct D8 : Const { public long Run() => 8; }
	struct D9 : Const { public long Run() => 9; }
	struct DA : Const { public long Run() => 0xA; }
	struct DB : Const { public long Run() => 0xB; }
	struct DC : Const { public long Run() => 0xC; }
	struct DD : Const { public long Run() => 0xD; }
	struct DE : Const { public long Run() => 0xE; }
	struct DF : Const { public long Run() => 0xF; }

	struct Num<A, B> : Const
		where A : struct, Const
		where B : struct, Const
	{
		[MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
		public long Run()
		{
			return default( A ).Run() << 4 | default( B ).Run();
		}
	}

	struct Num<A, B, C, D> : Const
		where A : struct, Const
		where B : struct, Const
		where C : struct, Const
		where D : struct, Const
	{
		[MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
		public long Run()
		{
			return default( A ).Run() << 12 | default( B ).Run() << 8 | default( C ).Run() << 4 | default( D ).Run();
		}
	}

	struct Num<A, B, C, D, E, F, G, H> : Const
		where A : struct, Const
		where B : struct, Const
		where C : struct, Const
		where D : struct, Const
		where E : struct, Const
		where F : struct, Const
		where G : struct, Const
		where H : struct, Const
	{
		[MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
		public long Run()
		{
			return default( A ).Run() << 28 | default( B ).Run() << 24 | default( C ).Run() << 20 | default( D ).Run() << 16 |
				default( E ).Run() << 12 | default( F ).Run() << 8 | default( G ).Run() << 4 | default( H ).Run();
		}
	}

	struct Num<A, B, C, D, E, F, G, H, I, J, K, L, M, N, O, P> : Const
		where A : struct, Const
		where B : struct, Const
		where C : struct, Const
		where D : struct, Const
		where E : struct, Const
		where F : struct, Const
		where G : struct, Const
		where H : struct, Const
		where I : struct, Const
		where J : struct, Const
		where K : struct, Const
		where L : struct, Const
		where M : struct, Const
		where N : struct, Const
		where O : struct, Const
		where P : struct, Const
	{
		[MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
		public long Run()
		{
			return
				default( A ).Run() << 60 | default( B ).Run() << 56 | default( C ).Run() << 52 | default( D ).Run() << 48 |
				default( E ).Run() << 44 | default( F ).Run() << 40 | default( G ).Run() << 36 | default( H ).Run() << 32 |
				default( I ).Run() << 28 | default( J ).Run() << 24 | default( K ).Run() << 20 | default( L ).Run() << 16 |
				default( M ).Run() << 12 | default( N ).Run() << 8 | default( O ).Run() << 4 | default( P ).Run();
		}
	}

	struct Neg<A> : Const
		where A : struct, Const
	{
		[MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
		public long Run()
		{
			return -default( A ).Run();
		}
	}

	public interface ICallable
	{
		public void Call( Span<long> frame, WasmInstance inst );

		public void SetBody( object body, string name );
	}

	public class FunctionWrapper : ICallable
	{
		private Action<Span<long>, WasmInstance> Wrapped;

		public FunctionWrapper( Action<Span<long>, WasmInstance> wrapped )
		{
			Wrapped = wrapped;
		}

		public void Call( Span<long> frame, WasmInstance inst )
		{
			Wrapped( frame, inst );
		}

		public void SetBody( object body, string name )
		{
			throw new NotImplementedException();
		}
	}

	class Function<BODY> : ICallable
		where BODY : struct, Stmt
	{
		BODY Body;
		string Name;

		[MethodImpl( MethodImplOptions.NoInlining | MethodImplOptions.AggressiveOptimization )]
		public void Call( Span<long> frame, WasmInstance inst )
		{
			/*for (int i = 0; i < FunctionStatic.Depth; i++)
			{
				Console.Write(" ");
			}
			Console.Write("enter " + Name + "( " + frame[0]);
			for (int i = 1; i < 8; i++)
			{
				Console.Write(", " + frame[i]);
			}
			Console.WriteLine(" )");
			FunctionStatic.Depth++;*/
			//Log.Info( "enter " + Name );

			Registers reg = default;
			Body.Run( ref reg, frame, inst );

			//Log.Info( "exit " + Name );

			/*FunctionStatic.Depth--;
			for (int i = 0; i < FunctionStatic.Depth; i++)
			{
				Console.Write(" ");
			}
			Console.WriteLine("exit " + Name);*/
		}

		public void SetBody( object body, string name )
		{
			Body = (BODY)body;
			Name = name;
		}
	}
}
