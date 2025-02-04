enum BlockKind {
    Block,
    Loop,
    If,
    Else
}

public abstract class BlockTerminator {
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

    protected void SwapBlocks(int a, int b) {
        var ba = NextBlocks[a];
        var bb = NextBlocks[b];
        if (ba == null || bb == null) {
            throw new Exception("null block in swap");
        }
        NextBlocks[a] = bb;
        NextBlocks[b] = ba;
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

    public void Invert() {
        SwapBlocks(0,1);
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

public class Block {
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
            case 4: base_ty = typeof(SetR4_I32<,>); break;
            case 5: base_ty = typeof(SetR5_I32<,>); break;
            case 6: base_ty = typeof(SetR6_I32<,>); break;
            case 7: base_ty = typeof(SetR7_I32<,>); break;

            default: throw new Exception("register-set out of bounds");
        }
        return HellBuilder.MakeGeneric(base_ty,[input,next]);
    }
}

class BlockStackEntry {
    public BlockKind Kind;
    public Block Block; // exit block
    public Block ElseBlock;
    public ValType Type;
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

    // I hate this stupid damn language. JUST LET ME INDEX INTO A STACK, FUCK YOU!
    private List<BlockStackEntry> BlockStack = new List<BlockStackEntry>();

    public void StartBlock(ValType ty) {
        BlockStack.Add(new BlockStackEntry{
            Kind = BlockKind.Block,
            Block = new Block(NextBlock),
            Type = ty
        });
        NextBlock++;
    }

    public void StartIf(ValType ty) {
        var exit_block = new Block(NextBlock);
        NextBlock++;
        var else_block = new Block(NextBlock);
        NextBlock++;

        var cond = PopExpression();
        var if_term = new JumpIf(CurrentBlock, cond, else_block);
        TerminateBlock(if_term);
        if_term.Invert();

        BlockStack.Add(new BlockStackEntry{
            Kind = BlockKind.If,
            Block = exit_block,
            ElseBlock = else_block,
            Type = ty
        });
    }

    public void StartElse() {
        if (BlockStack.Count == 0) {
            throw new Exception("else is missing if");
        }
        var block_info = BlockStack[BlockStack.Count-1];
        if (block_info.Kind != BlockKind.If) {
            throw new Exception("else is missing if");
        }
        SpillBlockResult(block_info);
        TerminateBlock(new Jump(CurrentBlock, block_info.Block));
        SwitchBlock(block_info.ElseBlock);
        block_info.Kind = BlockKind.Else;
        block_info.ElseBlock = null;
    }

    public void StartLoop(ValType ty) {
        var loop_block = new Block(NextBlock);
        NextBlock++;
        SwitchBlock(loop_block);
        BlockStack.Add(new BlockStackEntry{
            Kind = BlockKind.Loop,
            Block = loop_block,
            Type = ty
        });
    }

    public bool EndBlock() {
        if (BlockStack.Count == 0) {
            return true;
        }
        var block_info = BlockStack[BlockStack.Count-1];
        BlockStack.RemoveAt(BlockStack.Count-1);

        if (block_info.Kind == BlockKind.Block || block_info.Kind == BlockKind.If || block_info.Kind == BlockKind.Else) {
            SpillBlockResult(block_info);
            if (block_info.Kind == BlockKind.If) {
                // fix empty else block
                SwitchBlock(block_info.ElseBlock);
            }
            SwitchBlock(block_info.Block);
            if (block_info.Type != ValType.Void) {
                PushExpression(new GetLocal(99, block_info.Type));
            }
        } else if (block_info.Kind == BlockKind.Loop) {
            // ending a loop is a no-op
            // we shouldn't even need to spill the result to a temporary, since this is the only exit!
        } else {
            throw new Exception("todo end block "+block_info.Kind);
        }

        return false;
    }

    public void SpillBlockResult(BlockStackEntry target) {
        if (target.Type != ValType.Void) {
            var res = PopExpression();
            AddStatement(new Local(99,target.Type),res);
        }
    }

    public Block GetBlock(int i) {
        return BlockStack[BlockStack.Count-1-i].Block;
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

            if (block.Terminator != null) {
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
            } else {
                result += "\t"+block.Name+" -> ERROR [color=red constraint=false]\n";
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
            res += "ERROR: NO TERMINATOR!";
        } else {
            res += b.Terminator;
        }
        return res+"\n";
    }
}
