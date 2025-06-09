class TrapExpression : Expression
{
    public TrapExpression() : base(ValType.I32) { }

    public override Type BuildMirror()
    {
        return typeof(ExprTrap);
    }

    public override void Traverse(Action<Expression> f)
    {
        f(this);
    }
}

abstract class ControlStatement : Expression
{
    public ControlStatement() : base(ValType.I32) { }

    public abstract string ToString(int depth);
}

class IfStatement : ControlStatement
{
    public Expression Cond;
    public List<(Destination?, Expression)> StmtsThen;
    public List<(Destination?, Expression)> StmtsElse;

    public override Type BuildMirror()
    {
        var cond = Cond.BuildMirror();
        var stmt_then = MirrorBuilder.CompileStatements(StmtsThen);
        var stmt_else = MirrorBuilder.CompileStatements(StmtsElse);

        return MirrorBuilder.MakeGeneric(typeof(ExprIf<,,>), [cond, stmt_then, stmt_else]);
    }

    public override void Traverse(Action<Expression> f)
    {
        throw new Exception("if traversal nyi");
    }

    public override string ToString(int depth)
    {
        string tabs = DebugIR.Tabs(depth);

        string res = "If " + Cond + " {\n";
        res += DebugIR.DumpStatements(depth + 1, StmtsThen);
        res += tabs+"} else {\n";
        res += DebugIR.DumpStatements(depth + 1, StmtsElse);
        res += tabs+"}\n";
        return res;
    }
}
