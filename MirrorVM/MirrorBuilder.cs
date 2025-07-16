using System;
using System.IO;
using System.Text;

namespace MirrorVM
{
	class MirrorBuilder
	{
		public static Stream CompileLogFile;

		public static ICallable Compile( IRBody ir_body, string dump_name = null )
		{
			ControlFlowOptimizer.Optimize( ir_body.Entry, dump_name );

			var body = CompileBody( ir_body.Entry, dump_name );

			List<Type> func_args = [body.GetType()];

			var func_ty = MakeGeneric( typeof( Function<> ), func_args.ToArray() );

			if ( CompileLogFile != null && dump_name != null )
			{
				var bytes = Encoding.UTF8.GetBytes( dump_name + " :: " + func_ty + "\n" );
				CompileLogFile.Write( bytes );
			}

			var func = (ICallable)MakeInstance( func_ty );
			func.SetBody( body, dump_name );
			return func;
		}

		private static object CompileBody( Block initial_block, string dump_name )
		{
			Type result_ty;
			if ( initial_block.Terminator is Return )
			{
				DebugIR.Dump( initial_block, dump_name, false );

				result_ty = CompileStatements( initial_block.Statements );
				return MakeInstance( result_ty );
			}

			var blocks = initial_block.GatherBlocks();
			var ordered_blocks = new List<Block>();

			// clear indices
			foreach ( var block in blocks )
			{
				block.Index = -1;
			}

			// entry block is zero
			blocks[0].Index = 0;
			ordered_blocks.Add( blocks[0] );

			// number jump tables
			foreach ( var block in blocks )
			{
				if ( block.Terminator is JumpTable jt )
				{
					var next_blocks = jt.GetNextBlocks();
					for ( int i = 0; i < next_blocks.Count; i++ )
					{
						var next = next_blocks[i];
						if ( next.Index != -1 )
						{
							next = jt.AddIntermediateBlock( i );
						}
						next.Index = ordered_blocks.Count;
						ordered_blocks.Add( next );
					}
				}
			}
			// number remaining blocks
			foreach ( var block in blocks )
			{
				if ( block.Index == -1 )
				{
					block.Index = ordered_blocks.Count;
					ordered_blocks.Add( block );
				}
			}

			if ( dump_name != null && ordered_blocks.Count <= 1000 )
			{
				DebugIR.Dump( initial_block, dump_name, false );
			}

			List<Type> CompiledBlocks = new List<Type>();
			foreach ( var block in ordered_blocks )
			{
				Type block_ty = CompileStatements( block.Statements );

				var final_ty = block.Terminator.BuildMirror( block_ty );
				//Console.WriteLine("> term "+DebugType(final_ty));
				CompiledBlocks.Add( final_ty );
			}

			int block_limit;
			Type dispatch_loop_type;
			if ( CompiledBlocks.Count <= 10 )
			{
				block_limit = 10;
				dispatch_loop_type = typeof( DispatchLoop10<,,,,,,,,,> );
			}
			else if ( CompiledBlocks.Count <= 25 )
			{
				block_limit = 25;
				dispatch_loop_type = typeof( DispatchLoop25<,,,,,,,,,,,,,,,,,,,,,,,,> );
			}
			else if ( CompiledBlocks.Count <= 50 )
			{
				block_limit = 50;
				dispatch_loop_type = typeof( DispatchLoop50<,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,> );
			}
			else if ( CompiledBlocks.Count <= 100 )
			{
				block_limit = 100;
				dispatch_loop_type = typeof( DispatchLoop100<,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,> );
			}
			else
			{
				block_limit = 200;
				dispatch_loop_type = typeof( DispatchLoop200<,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,> );
			}
			if ( CompiledBlocks.Count > block_limit )
			{
				//Console.WriteLine("block count = " + CompiledBlocks.Count);
				var result = new DispatchLoopArray();
				result.Blocks = new Terminator[CompiledBlocks.Count];
				result.DebugName = dump_name;
				for ( int i = 0; i < CompiledBlocks.Count; i++ )
				{
					result.Blocks[i] = (Terminator)MakeInstance( CompiledBlocks[i] );
				}

				return result;
			}
			while ( CompiledBlocks.Count < block_limit )
			{
				CompiledBlocks.Add( typeof( TermVoid ) );
			}

			result_ty = MakeGeneric( dispatch_loop_type, CompiledBlocks.ToArray() );
			return MakeInstance( result_ty );
		}

		public static Type CompileStatements( List<(Destination, Expression)> stmts )
		{
			if ( stmts.Count == 0 )
			{
				return typeof( End );
			}

			List<List<Type>> chunks = [];
			List<Type> current_chunk = [];
			foreach ( var stmt in stmts )
			{
				if ( stmt.Item2 is DebugExpression )
				{
					continue;
				}
				else if ( stmt.Item2 is SplitExpression )
				{
					chunks.Add( current_chunk );
					current_chunk = [];
					continue;
				}
				current_chunk.Add( CompileStatement( stmt ) );
			}
			chunks.Add( current_chunk );
			chunks.Reverse();

			Type res = null;
			foreach ( var c in chunks )
			{
				int level = 0;
				var chunk = c;

				if ( res != null )
				{
					var no_inline = MakeGeneric( typeof( NoInline<> ), [res] );
					chunk.Add( no_inline );
				}

				while ( chunk.Count > 1 )
				{
					chunk = BuildStatementTree( level, chunk );
					level++;
				}

				if ( chunk.Count > 0 )
				{
					res = chunk[0];
				}
			}

			return res ?? typeof( End );
		}

		private static Type CompileStatement( (Destination, Expression) stmt_pair )
		{
			var (dest, source) = stmt_pair;

			if ( dest == null )
			{
				if ( source is StatementExpression stmt )
				{
					return stmt.BuildStatement();
				}

				var source_ty = source.BuildMirror();
				var val_ty = ConvertValType( source.Type );
				return MakeGeneric( typeof( ExprStmt<,> ), [source_ty, val_ty] );
			}
			else
			{
				var source_ty = source.BuildMirror();
				return dest.BuildDestination( source_ty );
			}
		}

		private static List<Type> BuildStatementTree( int level, List<Type> nodes )
		{
			List<Type> result = [];

			var bundle_ty = (level % 8) switch
			{
				0 => typeof( Stmts1<,,,> ),
				1 => typeof( Stmts2<,,,> ),
				2 => typeof( Stmts3<,,,> ),
				3 => typeof( Stmts4<,,,> ),
				4 => typeof( Stmts5<,,,> ),
				5 => typeof( Stmts6<,,,> ),
				6 => typeof( Stmts7<,,,> ),
				7 => typeof( Stmts8<,,,> ),
				_ => null
			};

			for ( int i = 0; i < nodes.Count; i += 4 )
			{
				if ( i + 1 == nodes.Count )
				{
					result.Add( nodes[i] );
				}
				else
				{
					var args = new Type[4];
					args[0] = nodes[i];
					for ( int j = 1; j < 4; j++ )
					{
						if ( i + j < nodes.Count )
						{
							args[j] = nodes[i + j];
						}
						else
						{
							args[j] = typeof( End );
						}
					}

					result.Add( MakeGeneric( bundle_ty, args ) );
				}
			}

			return result;
		}

		public static Type ConvertValType( ValType ty )
		{
			switch ( ty )
			{
				case ValType.I32: return typeof( int );
				case ValType.I64: return typeof( long );
				case ValType.F32: return typeof( float );
				case ValType.F64: return typeof( double );
			}
			throw new Exception( "todo convert type " + ty );
		}

		public static Type MakeGeneric( Type base_ty, Type[] args )
		{
#if SANDBOX
			for ( int i = 0; i < 100; i++ )
			{
				var bty = TypeLibrary.GetType( base_ty );
				if ( bty == null )
				{
					Log.Info( "retry " + base_ty );
					continue;
				}
				return bty.MakeGenericType( args );
			}
			throw new Exception( "bad basetype " + base_ty );
#else
        return base_ty.MakeGenericType(args);
#endif
		}

		public static object MakeInstance(Type ty) {
#if SANDBOX
			return TypeLibrary.Create<object>( ty );
#else
			return Activator.CreateInstance(ty);
#endif
		}

		public static Type MakeConstant( long x )
		{
			if ( x < 0 && x != long.MinValue )
			{
				return MakeGeneric( typeof( Neg<> ), [MakeConstant( -x )] );
			}
			ulong n = (ulong)x;
			if ( n < 16 )
			{
				return GetDigit( n );
			}
			else if ( n < 256 )
			{
				return MakeGeneric( typeof( Num<,> ), [GetDigit( n >> 4 ), GetDigit( n )] );
			}
			else if ( n < 65536 )
			{
				return MakeGeneric( typeof( Num<,,,> ), [
					GetDigit(n>>12),GetDigit(n>>8),GetDigit(n>>4),GetDigit(n)
				] );
			}
			else if ( n < 4294967296 )
			{
				return MakeGeneric( typeof( Num<,,,,,,,> ), [
					GetDigit(n>>28),GetDigit(n>>24),GetDigit(n>>20),GetDigit(n>>16),
				GetDigit(n>>12),GetDigit(n>>8),GetDigit(n>>4),GetDigit(n)
				] );
			}
			else
			{
				return MakeGeneric( typeof( Num<,,,,,,,,,,,,,,,> ), [
					GetDigit(n>>60),GetDigit(n>>56),GetDigit(n>>52),GetDigit(n>>48),
				GetDigit(n>>44),GetDigit(n>>40),GetDigit(n>>36),GetDigit(n>>32),
				GetDigit(n>>28),GetDigit(n>>24),GetDigit(n>>20),GetDigit(n>>16),
				GetDigit(n>>12),GetDigit(n>>8),GetDigit(n>>4),GetDigit(n)
				] );
			}
		}

		private static Type GetDigit( ulong n )
		{
			switch ( n & 0xF )
			{
				case 0: return typeof( D0 );
				case 1: return typeof( D1 );
				case 2: return typeof( D2 );
				case 3: return typeof( D3 );
				case 4: return typeof( D4 );
				case 5: return typeof( D5 );
				case 6: return typeof( D6 );
				case 7: return typeof( D7 );
				case 8: return typeof( D8 );
				case 9: return typeof( D9 );
				case 0xA: return typeof( DA );
				case 0xB: return typeof( DB );
				case 0xC: return typeof( DC );
				case 0xD: return typeof( DD );
				case 0xE: return typeof( DE );
				case 0xF: return typeof( DF );
			}
			throw new Exception( "die" );
		}
	}

	class StatementNode
	{
		public int Tier;
		public bool NoInline;
		public object[] Children = new object[4];
	}
}
