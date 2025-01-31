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
    Equal,

    GreaterEqualSigned
}

enum UnaryOpKind {
    EqualZero
}

enum BlockKind {
    Block,
    Loop
}

abstract class BlockTerminator {
    public abstract void SetFallThrough(Block b);

    private Block OwningBlock;
    private Block[] NextBlocks;
    
    public BlockTerminator(Block owning_block, int next_count) {
        OwningBlock = owning_block;
        NextBlocks = new Block[next_count];
    }

    protected void SetNextBlock(int index, Block b) {
        if (NextBlocks[index] != null) {
            var old = NextBlocks[index];
            old.Predecessors.Remove(OwningBlock);
        }
        NextBlocks[index] = b;
        b.Predecessors.Add(OwningBlock);
    }

    public void ReplaceNextBlock(Block old_block, Block new_block) {
        for (int i=0;i<NextBlocks.Length;i++) {
            if (NextBlocks[i] == old_block) {
                SetNextBlock(i, new_block);
                break;
            }
        }
    }

    public IReadOnlyList<Block> GetNextBlocks() {
        return NextBlocks;
    }

    public virtual string LabelLink(int i) {
        return "todo";
    }
}

class Jump : BlockTerminator {
    public Jump(Block owner, Block next) : base(owner,1) {
        SetNextBlock(0, next);
    }

    public override void SetFallThrough(Block b)
    {
        // do nothing
    }

    public override string LabelLink(int i)
    {
        return "";
    }
}

class JumpIf : BlockTerminator {
    public Expression Cond;

    // blocks = [true, false]

    public JumpIf(Block owner, Expression cond, Block b) : base(owner,2) {
        Cond = cond;
        SetNextBlock(0, b);
    }

    public override void SetFallThrough(Block b)
    {
        SetNextBlock(1, b);
    }

    public override string ToString()
    {
        return "JumpIf("+Cond+")";
    }

    public override string LabelLink(int i)
    {
        return i == 0 ? "true" : "false";
    }
}

class JumpTable : BlockTerminator {
    public Expression Selector;

    public JumpTable(Block owner, Expression sel, List<Block> opts, Block def) : base(owner, opts.Count + 1) {
        Selector = sel;
        for (int i=0;i<opts.Count;i++) {
            SetNextBlock(i, opts[i]);
        }
        SetNextBlock(opts.Count, def);
    }

    public override void SetFallThrough(Block b)
    {
        // do nothing
    }

    public override string ToString()
    {
        return "JumpTable("+Selector+")";
    }

    public override string LabelLink(int i)
    {
        var count = GetNextBlocks().Count;
        if (i < count-1) {
            return i.ToString();
        }
        return "default";
    }
    /*public override string GetLinks(string this_block) {
        string result = "";
        for (int i=0;i<Options.Count;i++) {
            result += this_block+" -> "+Options[i].Name+" [ label = "+i+" ];";
        }
        return result + this_block+" -> "+Default.Name+" [ label = default ];";
        /*return 
            this_block+" -> "+True.Name+" [ label = true ]; " +
            this_block+" -> "+False.Name+" [ label = false ];";* /
    }*/
}

class Return : BlockTerminator {
    public Expression Value;

    public Return(Block owner, Expression value) : base(owner, 0) {
        Value = value;
    }

    public override void SetFallThrough(Block b)
    {
        // do nothing
    }

    public override string ToString()
    {
        return "Return("+Value+")";
    }
}

class Trap : BlockTerminator {
    public Trap(Block owner) : base(owner, 0) {}

    public override void SetFallThrough(Block b)
    {
        // do nothing
    }
}

class Block {
    public string Name;
    public bool IsEntry = false;

    public Block(int n) {
        Name = "Block"+n;
    }

    private List<int> LocalWrites = new List<int>();
    private List<int> LocalReads = new List<int>();

    public List<Block> Predecessors = new List<Block>();
    public List<(Destination?,Expression)> Statements = new List<(Destination?,Expression)>();

    public BlockTerminator Terminator;

    public bool IsTriviallyRedundant() {
        return Statements.Count == 0 && Terminator is Jump;
    }

    public void Delete() {
        foreach (var next in Terminator.GetNextBlocks()) {
            next.Predecessors.Remove(this);
        }

        if (Predecessors.Count > 0) {
            var next_blocks = Terminator.GetNextBlocks();
            if (next_blocks.Count != 1) {
                throw new Exception("can't delete a block where pred_count > 0 and next_count != 1");
            }
            var next = next_blocks[0];
            foreach (var pred in Predecessors.ToArray()) {
                pred.Terminator.ReplaceNextBlock(this,next);
            }
        }
    }
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
    public Block CurrentBlock;

    public IRBuilder() {
        CurrentBlock = InitialBlock;
        CurrentBlock.IsEntry = true;
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
        term.SetFallThrough(CurrentBlock);
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

        CurrentBlock.Terminator = new Jump(CurrentBlock, block);
        CurrentBlock = block;
    }

    public void PruneBlocks() {
        HashSet<Block> Closed = new HashSet<Block>();
        Queue<Block> Open = new Queue<Block>();
        Open.Enqueue(InitialBlock);
        Closed.Add(InitialBlock);

        while (Open.Count > 0) {
            var block = Open.Dequeue();
            var next_blocks = block.Terminator.GetNextBlocks();
            
            bool force_enqueue_links = false;
            if (!block.IsEntry && block.Predecessors.Count == 0) {
                // KILL
                block.Delete();
                force_enqueue_links = true;
            } else if (block.IsTriviallyRedundant()) {
                block.Delete();
            }
            
            foreach (var next in next_blocks) {
                if (force_enqueue_links || !Closed.Contains(next)) {
                    Open.Enqueue(next);
                    Closed.Add(next);
                }
            }
            foreach (var prev in block.Predecessors) {
                if (force_enqueue_links || !Closed.Contains(prev)) {
                    Open.Enqueue(prev);
                    Closed.Add(prev);
                }
            }
        }
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

            var next_blocks = block.Terminator.GetNextBlocks();
            for (int i=0;i<next_blocks.Count;i++) {
                // add link
                var next = next_blocks[i];
                var label = block.Terminator.LabelLink(i);
                result += "\t"+block.Name+" -> "+next.Name+" [label = \""+label+"\"]\n";

                // enqueue block
                if (!Closed.Contains(next)) {
                    Open.Enqueue(next);
                    Closed.Add(next);
                }
            }
            foreach (var pred in block.Predecessors) {
                result += "\t"+block.Name+" -> "+pred.Name+" [color=blue constraint=false]\n";
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
