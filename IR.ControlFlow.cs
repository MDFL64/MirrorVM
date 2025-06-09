class TrapExpression : Expression
{
    public TrapExpression() : base(ValType.Void) { }

    public override Type BuildMirror()
    {
        throw new NotImplementedException();
    }

    public override void Traverse(Action<Expression> f)
    {
        f(this);
    }
}

abstract class ControlStatement : Expression
{
    public ControlStatement() : base(ValType.Void) { }

    public abstract string ToString(int depth);
}

class IfStatement : ControlStatement
{
    public Expression Cond;
    public List<(Destination?, Expression)> StmtsThen;
    public List<(Destination?, Expression)> StmtsElse;

    public override Type BuildMirror()
    {
        throw new Exception("if construction nyi");
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
