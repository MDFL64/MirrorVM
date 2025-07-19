using System;

namespace MirrorVM
{
	abstract class StatementExpression : Expression
	{
		public StatementExpression() : base( ValType.I32 ) { }

		public override Type BuildMirror()
		{
			throw new NotSupportedException();
		}

		public abstract Type BuildStatement();
	}

	class TrapExpression : StatementExpression
	{
		public override Type BuildStatement()
		{
			return typeof( StmtTrap );
		}

		public override void Traverse( Action<Expression> f )
		{
			f( this );
		}

		public override string ToString()
		{
			return "Trap";
		}
	}

	class ClearFrameExpression : StatementExpression
	{
		private int Start;
		private int End;

		public ClearFrameExpression(int start, int end)
		{
			Start = start;
			End = end;
		}

		public override Type BuildStatement()
		{
			return MirrorBuilder.MakeGeneric(typeof(ClearFrame<,>), [
				MirrorBuilder.MakeConstant(Start),
				MirrorBuilder.MakeConstant(End)
			]);
		}

		public override void Traverse(Action<Expression> f)
		{
			f(this);
		}

		public override string ToString()
		{
			return "ClearFrame("+Start+","+End+")";
		}
	}

	abstract class ControlStatement : StatementExpression
	{
		public abstract string ToString(int depth);
	}

	class IfStatement : ControlStatement
	{
		public Expression Cond;
		public List<(Destination?, Expression)> StmtsThen;
		public List<(Destination?, Expression)> StmtsElse;

		public override Type BuildStatement()
		{
			var cond = Cond.BuildMirror();
			var stmt_then = MirrorBuilder.CompileStatements( StmtsThen );
			var stmt_else = MirrorBuilder.CompileStatements( StmtsElse );

			return MirrorBuilder.MakeGeneric( typeof( StmtIf<,,> ), [cond, stmt_then, stmt_else] );
		}

		public override void Traverse( Action<Expression> f )
		{
			throw new Exception( "if traversal nyi" );
		}

		public override string ToString( int depth )
		{
			string tabs = DebugIR.Tabs( depth );

			string res = "If " + Cond + " {\n";
			res += DebugIR.DumpStatements( depth + 1, StmtsThen );
			res += tabs + "} else {\n";
			res += DebugIR.DumpStatements( depth + 1, StmtsElse );
			res += tabs + "}\n";
			return res;
		}
	}

	class LoopStatement : ControlStatement
	{
		public Expression Cond;
		public List<(Destination?, Expression)> Stmts;
		public bool LoopValue;

		public override Type BuildStatement()
		{
			var cond = Cond.BuildMirror();
			var body = MirrorBuilder.CompileStatements( Stmts );
			if ( LoopValue )
			{
				return MirrorBuilder.MakeGeneric( typeof( StmtLoopTrue<,> ), [cond, body] );
			}
			else
			{
				return MirrorBuilder.MakeGeneric( typeof( StmtLoopFalse<,> ), [cond, body] );
			}
		}

		public override string ToString( int depth )
		{
			string tabs = DebugIR.Tabs( depth );

			string loop_word = LoopValue ? "While" : "Until";
			string res = "Do {\n";
			res += DebugIR.DumpStatements( depth + 1, Stmts );
			res += tabs + "} " + loop_word + " " + Cond + "\n";
			return res;
		}

		public override void Traverse( Action<Expression> f )
		{
			throw new Exception( "loop traversal nyi" );
		}
	}
}
