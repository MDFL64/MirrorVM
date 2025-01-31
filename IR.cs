using System.Runtime.CompilerServices;

public abstract class Expression {
    public ValType Type;

    public Expression(ValType ty) {
        Type = ty;
    }
}

class GetLocal : Expression {
    int LocalIndex;
    public GetLocal(int index, ValType ty) : base(ty) {
        LocalIndex = index;
    }

    public override string ToString()
    {
        return "L"+LocalIndex;
    }
}

class Constant : Expression {
    long Value;

    public static Constant I32(long x) {
        return new Constant(x, ValType.I32);
    }

    private Constant(long value, ValType ty) : base(ty) {
        Value = value;
    }

    public override string ToString()
    {
        return Value.ToString();
    }
}

class BinaryOp : Expression {
    public BinaryOpKind Kind;
    public Expression A;
    public Expression B;

    public BinaryOp(ValType ty, BinaryOpKind kind, Expression a, Expression b) : base(ty) {
        Kind = kind;
        A = a;
        B = b;
    }

    public override string ToString()
    {
        return Kind+"("+A+","+B+")";
    }
}

class UnaryOp : Expression {
    public UnaryOpKind Kind;
    public Expression A;

    public UnaryOp(ValType ty, UnaryOpKind kind, Expression a) : base(ty) {
        Kind = kind;
        A = a;
    }

    public override string ToString()
    {
        return Kind+"("+A+")";
    }
}

enum BinaryOpKind {
    Add,
    Sub,
    Mul,
    DivSigned,
    DivUnsigned,

    ShiftLeft,
    Equal
}

enum UnaryOpKind {
    EqualZero
}

enum BlockKind {
    Block,
    Loop
}

abstract class BlockTerminator {
    public abstract void SetNextBlock(Block b);

    public abstract IEnumerable<Block> GetNextBlocks();

    public virtual string GetLinks(string this_block) {
        return this_block+" -> TODO; "+this_block+" -> TODO;";
    }
}

class Jump : BlockTerminator {
    public Block Next;

    public Jump(Block next) {
        Next = next;
    }

    public override void SetNextBlock(Block b)
    {
        // do nothing
    }

    public override IEnumerable<Block> GetNextBlocks()
    {
        yield return Next;
    }

    public override string GetLinks(string this_block) {
        return this_block+" -> "+Next.Name;
    }
}

class JumpIf : BlockTerminator {
    public Expression Cond;

    public Block True;
    public Block False;

    public JumpIf(Expression cond, Block t) {
        Cond = cond;
        True = t;
    }

    public override void SetNextBlock(Block b)
    {
        False = b;
    }

    public override IEnumerable<Block> GetNextBlocks()
    {
        yield return True;
        yield return False;
    }

    public override string GetLinks(string this_block) {
        return 
            this_block+" -> "+True.Name+" [ label = true ]; " +
            this_block+" -> "+False.Name+" [ label = false ];";
    }

    public override string ToString()
    {
        return "JumpIf("+Cond+")";
    }
}

class JumpTable : BlockTerminator {
    public Expression Selector;
    public List<Block> Options;
    public Block Default;

    public JumpTable(Expression sel, List<Block> opts, Block def) {
        Selector = sel;
        Options = opts;
        Default = def;
    }

    public override void SetNextBlock(Block b)
    {
        // do nothing
    }

    public override IEnumerable<Block> GetNextBlocks()
    {
        foreach (var opt in Options) {
            yield return opt;
        }
        yield return Default;
    }

    public override string ToString()
    {
        return "JumpTable("+Selector+")";
    }

    public override string GetLinks(string this_block) {
        string result = "";
        for (int i=0;i<Options.Count;i++) {
            result += this_block+" -> "+Options[i].Name+" [ label = "+i+" ];";
        }
        return result + this_block+" -> "+Default.Name+" [ label = default ];";
        /*return 
            this_block+" -> "+True.Name+" [ label = true ]; " +
            this_block+" -> "+False.Name+" [ label = false ];";*/
    }
}

class Return : BlockTerminator {
    public Expression Value;

    public Return(Expression value) {
        Value = value;
    }

    public override void SetNextBlock(Block b)
    {
        // do nothing
    }

    public override IEnumerable<Block> GetNextBlocks()
    {
        // do nothing
        return [];
    }

    public override string ToString()
    {
        return "Return("+Value+")";
    }

    public override string GetLinks(string this_block) {
        return "";
    }
}

class Trap : BlockTerminator {
    public override void SetNextBlock(Block b)
    {
        // do nothing
    }

    public override IEnumerable<Block> GetNextBlocks()
    {
        // do nothing
        return [];
    }

    public override string GetLinks(string this_block) {
        return "";
    }
}

class Block {
    public string Name;

    public Block(int n) {
        Name = "Block"+n;
    }

    private List<int> LocalWrites = new List<int>();
    private List<int> LocalReads = new List<int>();

    public List<(Destination?,Expression)> Statements = new List<(Destination?,Expression)>();

    public BlockTerminator Terminator;
}

public abstract class Destination {

}

class Local : Destination {
    int Index;
    public Local(int index) {
        Index = index;
    }

    public override string ToString()
    {
        return "L"+Index;
    }
}

class IRBuilder {
    private int NextBlock = 2;

    private Block InitialBlock = new Block(1);
    private Block CurrentBlock;

    public IRBuilder() {
        CurrentBlock = InitialBlock;
    }

    private Stack<Expression> ExpressionStack = new Stack<Expression>();

    // null indicates a loop
    // if / else unsupported
    // I hate this stupid damn language. JUST LET ME INDEX INTO A STACK, FUCK YOU!
    private List<(Block,BlockKind)> BlockStack = new List<(Block,BlockKind)>();

    public void StartBlock(ValType ty) {
        if (ty != ValType.Void) {
            throw new Exception("blocks with values not supported");
        }
        BlockStack.Add((new Block(NextBlock),BlockKind.Block));
        NextBlock++;
    }

    public void StartLoop(ValType ty) {
        if (ty != ValType.Void) {
            throw new Exception("blocks with values not supported");
        }
        var loop_block = new Block(NextBlock);
        NextBlock++;
        SwitchBlock(loop_block);
        BlockStack.Add((loop_block,BlockKind.Loop));
    }

    public bool EndBlock() {
        if (BlockStack.Count == 0) {
            return true;
        }
        (var b,var kind) = BlockStack[BlockStack.Count-1];
        BlockStack.RemoveAt(BlockStack.Count-1);

        if (kind == BlockKind.Block) {
            SwitchBlock(b);
        } else if (kind == BlockKind.Loop) {
            // ending a loop is a no-op
        } else {
            throw new Exception("todo end block "+kind);
        }

        return false;
    }

    public Block GetBlock(int i) {
        return BlockStack[BlockStack.Count-1-i].Item1;
    }

    public void PushExpression(Expression e) {
        ExpressionStack.Push(e);
    }

    public Expression PopExpression() {
        return ExpressionStack.Pop();
    }

    public int GetExpressionStackSize() {
        return ExpressionStack.Count;
    }

    public void PushBinaryOp(ValType ty, BinaryOpKind kind) {
        var b = PopExpression();
        var a = PopExpression();
        PushExpression(new BinaryOp(ty, kind, a, b));
    }

    public void AddStatement(Destination? dest, Expression expr) {
        CurrentBlock.Statements.Add((dest,expr));
    }

    public void TerminateBlock(BlockTerminator term) {
        SpillStack();

        CurrentBlock.Terminator = term;
        CurrentBlock = new Block(NextBlock);
        NextBlock++;
        term.SetNextBlock(CurrentBlock);
    }

    private void SpillStack() {
        if (ExpressionStack.Count > 0) {
            throw new Exception("todo spill stack");
        }
    }

    private void SwitchBlock(Block block) {
        if (block == CurrentBlock) {
            throw new Exception("attempt to switch to current block");
        }
        if (block.Terminator != null || CurrentBlock.Terminator != null) {
            throw new Exception("neither block involved in switch should have a terminator");
        }
        if (block.Statements.Count > 0) {
            throw new Exception("attempt to switch to filled block");
        }
        SpillStack();

        Console.WriteLine("current "+CurrentBlock.Statements.Count);
        Console.WriteLine("next "+block.Statements.Count);

        CurrentBlock.Terminator = new Jump(block);
        CurrentBlock = block;
    }

    public void Dump() {
        HashSet<Block> Closed = new HashSet<Block>();
        Queue<Block> Open = new Queue<Block>();
        Open.Enqueue(InitialBlock);
        Closed.Add(InitialBlock);

        string result = "digraph {\n";

        while (Open.Count > 0) {
            var block = Open.Dequeue();
            string block_str = DumpBlock(block).Replace("\n","\\l");
            //Console.WriteLine("==> "+block_str);
            result += "\t"+block.Name+" [ shape=box label =\""+block_str+"\" ]\n";
            result += "\t"+block.Terminator.GetLinks(block.Name)+"\n";

            foreach (var next in block.Terminator.GetNextBlocks()) {
                if (!Closed.Contains(next)) {
                    Open.Enqueue(next);
                    Closed.Add(next);
                }
            }
        }
        result += "}";

        File.WriteAllText("graph.dot",result);
    }

    private string DumpBlock(Block b) {
        string res = "";
        foreach (var stmt in b.Statements) {
            (var dst,var src) = stmt;
            if (dst != null) {
                res += dst + " = " + src + "\n";
            } else {
                res += src + "\n";
            }
        }
        if (b.Terminator == null) {
            throw new Exception("no terminator");
        }
        res += b.Terminator;
        return res+"\n";
    }
}
