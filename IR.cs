using System.Runtime.CompilerServices;

public abstract class Expression {
    public virtual Type BuildHell() {
        throw new Exception("todo build hell: "+this.GetType());
    }
}

class GetLocal : Expression {
    int LocalIndex;
    ValType Type;

    public GetLocal(int index, ValType ty) {
        Type = ty;
        LocalIndex = index;
    }

    public override string ToString()
    {
        return "L"+LocalIndex;
    }

    public override Type BuildHell() {
        if (Type != ValType.I32) {
            throw new Exception("todo non-i32 locals");
        }
        switch (LocalIndex) {
            case 0: return typeof(GetR0_I32);
            case 1: return typeof(GetR1_I32);
            case 2: return typeof(GetR2_I32);
            case 3: return typeof(GetR3_I32);
            default: throw new Exception("register too big :(");
        }
    }
}

class Constant : Expression {
    long Value;
    ValType Type;

    public static Constant I32(long x) {
        return new Constant(x, ValType.I32);
    }

    private Constant(long value, ValType ty) {
        Type = ty;
        Value = value;
    }

    public override string ToString()
    {
        return Value.ToString();
    }

    public override Type BuildHell() {
        if (Type != ValType.I32) {
            throw new Exception("todo non-i32 constants");
        }
        return HellBuilder.MakeGeneric(typeof(Const_I32<>),[HellBuilder.MakeConstant(Value)]);
    }
}

class BinaryOp : Expression {
    public BinaryOpKind Kind;
    public Expression A;
    public Expression B;

    public BinaryOp(BinaryOpKind kind, Expression a, Expression b) {
        Kind = kind;
        A = a;
        B = b;
    }

    public override string ToString()
    {
        return Kind+"("+A+","+B+")";
    }

    public override Type BuildHell() {
        Type ty;
        switch (Kind) {
            case BinaryOpKind.I32_GreaterEqual_S: ty = typeof(Op_I32_GreaterEqual_S<,>); break;
            case BinaryOpKind.I32_Add: ty = typeof(Op_I32_Add<,>); break;
            case BinaryOpKind.I32_Sub: ty = typeof(Op_I32_Sub<,>); break;

            case BinaryOpKind.I32_Div_S: ty = typeof(Op_I32_Div_S<,>); break;

            case BinaryOpKind.I32_ShiftLeft: ty = typeof(Op_I32_ShiftLeft<,>); break;
            default:
                throw new Exception("todo build hell: "+Kind);
        }
        var lhs = A.BuildHell();
        var rhs = B.BuildHell();

        return HellBuilder.MakeGeneric(ty,[lhs,rhs]);
    }
}

class UnaryOp : Expression {
    public UnaryOpKind Kind;
    public Expression A;

    public UnaryOp(UnaryOpKind kind, Expression a) {
        Kind = kind;
        A = a;
    }

    public override string ToString()
    {
        return Kind+"("+A+")";
    }
}

enum BinaryOpKind {
    I32_Add,
    I32_Sub,
    I32_Mul,
    I32_Div_S,
    I32_Div_U,

    I32_ShiftLeft,
    I32_Equal,

    I32_GreaterEqual_S
}

enum UnaryOpKind {
    I32_EqualZero
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

    public virtual Type BuildHell(Type body) {
        throw new Exception("todo build hell: "+this.GetType());
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

    public override Type BuildHell(Type body) {
        var blocks = GetNextBlocks();
        var next = HellBuilder.MakeConstant(blocks[0].Index);

        return HellBuilder.MakeGeneric(typeof(TermJump<,>),[next,body]);
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

    public override Type BuildHell(Type body) {
        var blocks = GetNextBlocks();
        var cond = Cond.BuildHell();
        var t = HellBuilder.MakeConstant(blocks[0].Index);
        var f = HellBuilder.MakeConstant(blocks[1].Index);

        return HellBuilder.MakeGeneric(typeof(TermJumpIf<,,,>),[cond,t,f,body]);
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

    public override Type BuildHell(Type body) {
        var value = Value.BuildHell();
        return HellBuilder.MakeGeneric(typeof(TermReturn_I32<,>),[value,body]);
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
    public int Index = -1;
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
    public virtual Type BuildHell(Type input, Type next) {
        throw new Exception("todo build hell: "+this.GetType());
    }
}

class Local : Destination {
    int Index;
    ValType Type;

    public Local(int index, ValType ty) {
        Type = ty;
        Index = index;
    }

    public override string ToString()
    {
        return "L"+Index;
    }

    public override Type BuildHell(Type input, Type next) {
        if (Type != ValType.I32) {
            throw new Exception("todo non-i32 locals");
        }
        Type base_ty;
        switch (Index) {
            case 0: base_ty = typeof(SetR0_I32<,>); break;
            case 1: base_ty = typeof(SetR1_I32<,>); break;
            case 2: base_ty = typeof(SetR2_I32<,>); break;
            case 3: base_ty = typeof(SetR3_I32<,>); break;
            default: throw new Exception("register too big :(");
        }
        return HellBuilder.MakeGeneric(base_ty,[input,next]);
    }
}

class IRBuilder {
    private int NextBlock = 2;

    public Block InitialBlock = new Block(1);
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

    public void PushBinaryOp(BinaryOpKind kind) {
        var b = PopExpression();
        var a = PopExpression();
        PushExpression(new BinaryOp(kind, a, b));
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

    public void Dump(bool draw_backlinks) {
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
            if (draw_backlinks) {
                foreach (var pred in block.Predecessors) {
                    result += "\t"+block.Name+" -> "+pred.Name+" [color=blue constraint=false]\n";
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
