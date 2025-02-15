using System.Reflection.Metadata;

enum BlockKind {
    Block,
    Loop,
    If,
    Else
}

public abstract class BlockTerminator {
    public abstract void SetFallThrough(Block b);

    public abstract void TraverseExpressions(Action<Expression> f);

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

    public abstract Type BuildHell(Type body);

    public Block AddIntermediateBlock(int index) {
        var new_block = new Block();
        var next = NextBlocks[index];

        // link this to new
        NextBlocks[index] = new_block;
        new_block.Predecessors.Add(OwningBlock);

        // link new to next
        next.Predecessors.Remove(OwningBlock);
        new_block.Terminator = new Jump(new_block, next);
        return new_block;
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

    public override void TraverseExpressions(Action<Expression> f) {}
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

    public override void TraverseExpressions(Action<Expression> f) {
        Cond.Traverse(f);
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

    public override void TraverseExpressions(Action<Expression> f) {
        Selector.Traverse(f);
    }

    public override Type BuildHell(Type body)
    {
        var blocks = GetNextBlocks();
        var sel = Selector.BuildHell();
        var jump_base = HellBuilder.MakeConstant(blocks[0].Index);
        var jump_count = HellBuilder.MakeConstant(blocks.Count);

        return HellBuilder.MakeGeneric(typeof(TermJumpTable<,,,>),[sel,jump_base,jump_count,body]);
    }
}

class Return : BlockTerminator {
    public Return(Block owner) : base(owner, 0) {}

    public override void SetFallThrough(Block b)
    {
        // do nothing
    }

    public override Type BuildHell(Type body) {
        return HellBuilder.MakeGeneric(typeof(TermReturn<>),[body]);
    }

    public override string ToString()
    {
        return "Return";
    }

    public override void TraverseExpressions(Action<Expression> f) {}
}

class Trap : BlockTerminator {
    public Trap(Block owner) : base(owner, 0) {}

    public override void SetFallThrough(Block b)
    {
        // do nothing
    }

    public override Type BuildHell(Type body) {
        return typeof(TermTrap);
    }

    public override void TraverseExpressions(Action<Expression> f) {}
}

public class Block {
    public string Name;
    public int Index = -1;
    public bool IsEntry = false;

    static long NextBlockId = 0;

    public Block() {
        Name = "Block"+NextBlockId;
        NextBlockId++;
    }

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

    public List<Block> GatherBlocks() {
        List<Block> res = [];

        HashSet<Block> Closed = new HashSet<Block>();
        Queue<Block> Open = new Queue<Block>();
        Open.Enqueue(this);
        Closed.Add(this);

        while (Open.Count > 0) {
            var block = Open.Dequeue();
            var next_blocks = block.Terminator.GetNextBlocks();
            res.Add(block);
            
            foreach (var next in next_blocks) {
                if (!Closed.Contains(next)) {
                    Open.Enqueue(next);
                    Closed.Add(next);
                }
            }
        }

        return res;
    }
}

class BlockStackEntry {
    public BlockKind Kind;
    public Block Block; // exit block
    public Block ElseBlock;
    public Local[] SpillLocals;
}

class IRBuilder {
    private int VariableCount = 0;
    private int SpillCount = 0;
    private int ReturnSlotCount = 0;
    private int CallSlotBase = 0;
    private int CallSlotTotalCount = 0;

    public Block InitialBlock = new Block();
    public Block CurrentBlock;

    public IRBuilder(List<ValType> local_types, List<ValType> ret_types) {
        VariableCount = local_types.Count;
        if (ret_types.Count > 1) {
            ReturnSlotCount = ret_types.Count - 1;
        }
        CurrentBlock = InitialBlock;
        CurrentBlock.IsEntry = true;
    }

    private Stack<Expression> ExpressionStack = new Stack<Expression>();

    // I hate this stupid damn language. JUST LET ME INDEX INTO A STACK, FUCK YOU!
    private List<BlockStackEntry> BlockStack = new List<BlockStackEntry>();

    public Local CreateSpillLocal(ValType ty) {
        var result = new Local(SpillCount, ty, LocalKind.Spill);
        SpillCount++;
        return result;
    }

    public void AddCall(FunctionType sig, string debug_name, int? func_index) {
        var args = new List<Expression>();
        for (int i=0;i<sig.Inputs.Count;i++) {
            args.Add(PopExpression());
        }
        
        if (func_index == null) {
            throw new Exception("todo dynamic calls");
        }
        Expression call_expr = new Call(func_index.Value, CallSlotBase, args, debug_name);

        if (sig.Outputs.Count == 0) {
            AddStatement(null, call_expr);
        } else {
            var out_ty = sig.Outputs[0];
            var output = CreateSpillLocal(out_ty);
            AddStatement(output.WithType(ValType.I64), call_expr);
            PushExpression(output);
            // multi-returns
            for (int i=1;i<sig.Outputs.Count;i++) {
                PushExpression(new Local(CallSlotBase + i - 1, sig.Outputs[i], LocalKind.Call));
            }
        }

        int slots_used = int.Max(sig.Inputs.Count,sig.Outputs.Count - 1);
        CallSlotTotalCount = int.Max(CallSlotTotalCount, CallSlotBase + slots_used);

        if (sig.Outputs.Count > 1) {
            CallSlotBase += sig.Outputs.Count - 1;
        }
    }

    public void AddReturn(int ret_count) {
        var values = new Expression[ret_count];
        for (int i=0;i<ret_count;i++) {
            values[i] = PopExpression();
        }
        Array.Reverse(values);
        for (int i=1;i<values.Length;i++) {
            var ty = values[i].Type;
            AddStatement(new Local(i-1,ty,LocalKind.Frame),values[i]);
        }
        // clobbers a register, must be last
        if (ret_count > 0) {
            var ty = values[0].Type;
            AddStatement(new Local(0,ty,LocalKind.Register),values[0]);
        }

        TerminateBlock(new Return(CurrentBlock));
    }

    public void StartBlock(ValType[] tys) {
        Local[] spill_locals = new Local[tys.Length];
        for (int i=0;i<spill_locals.Length;i++) {
            spill_locals[i] = CreateSpillLocal(tys[i]);
        }
        BlockStack.Add(new BlockStackEntry{
            Kind = BlockKind.Block,
            Block = new Block(),
            SpillLocals = spill_locals
        });
    }

    public void StartIf(ValType[] tys) {
        var exit_block = new Block();
        var else_block = new Block();

        var cond = PopExpression();
        var if_term = new JumpIf(CurrentBlock, cond, else_block);
        TerminateBlock(if_term);
        if_term.Invert();

        Local[] spill_locals = new Local[tys.Length];
        for (int i=0;i<spill_locals.Length;i++) {
            spill_locals[i] = CreateSpillLocal(tys[i]);
        }
        BlockStack.Add(new BlockStackEntry{
            Kind = BlockKind.If,
            Block = exit_block,
            ElseBlock = else_block,
            SpillLocals = spill_locals
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

    public void StartLoop(ValType[] tys) {
        var loop_block = new Block();
        SwitchBlock(loop_block);
        BlockStack.Add(new BlockStackEntry{
            Kind = BlockKind.Loop,
            Block = loop_block
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
            foreach (var local in block_info.SpillLocals) {
                PushExpression(local);
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
        foreach (var local in target.SpillLocals.Reverse()) {
            var res = PopExpression();
            AddStatement(local,res);
        }
    }

    public void TeeBlockResult(BlockStackEntry target) {
        foreach (var local in target.SpillLocals.Reverse()) {
            var res = PopExpression();
            AddStatement(local,res);
        }
        foreach (var local in target.SpillLocals) {
            PushExpression(local);
        }
    }

    public BlockStackEntry GetBlock(int i) {
        return BlockStack[BlockStack.Count-1-i];
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

    public void PushMemoryRead(ValType res, MemSize size, int offset) {
        var addr = PopExpression();
        PushExpression(new MemoryOp(res,size,addr,offset));
    }

    public void AddMemoryWrite(ValType arg_ty, MemSize size, int offset) {
        var value = PopExpression();
        var addr = PopExpression();
        AddStatement(new MemoryOp(arg_ty,size,addr,offset),value);
    }

    public void AddStatement(Destination? dest, Expression expr) {
        CurrentBlock.Statements.Add((dest,expr));
    }

    public void TerminateBlock(BlockTerminator term) {
        SpillStack();

        CurrentBlock.Terminator = term;
        CurrentBlock = new Block();
        term.SetFallThrough(CurrentBlock);
    }

    private void SpillStack() {
        var stack = ExpressionStack.ToArray();
        bool mutated = false;

        for (int i=0;i<stack.Length;i++) {
            if (stack[i].IsAnyRead()) {
                var spill = CreateSpillLocal(stack[i].Type);
                AddStatement(spill, stack[i]);
                stack[i] = spill;

                mutated = true;
            }
        }

        if (mutated) {
            ExpressionStack = new Stack<Expression>(stack);
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

    public void LowerLocals() {
        var blocks = InitialBlock.GatherBlocks();
        foreach (var block in blocks) {
            foreach (var (dest,expr) in block.Statements) {
                if (dest != null) {
                    dest.Traverse(LowerLocal);
                }
                expr.Traverse(LowerLocal);
            }
            block.Terminator.TraverseExpressions(LowerLocal);
        }
    }

    private void LowerLocal(Expression e) {
        if (e is Local local) {
            if (local.Kind == LocalKind.Variable) {
                local.Kind = LocalKind.Register;
            } else if (local.Kind == LocalKind.Spill) {
                local.Kind = LocalKind.Register;
                local.Index += VariableCount;
            } else if (local.Kind == LocalKind.Call) {
                local.Kind = LocalKind.Frame;
                local.Index += ReturnSlotCount;
            } else if (local.Kind == LocalKind.Register || local.Kind == LocalKind.Frame) {
                // generated by returns
                return;
            } else {
                Console.WriteLine("TODO FIX "+local.Kind);
            }
            // convert high registers to frame accesses
            if (local.Kind == LocalKind.Register && local.Index >= 7) {
                int index = ReturnSlotCount + CallSlotTotalCount + (local.Index - 7);
                local.Kind = LocalKind.Frame;
                local.Index = index;
            }
        }
        if (e is Call call) {
            call.FrameIndex += ReturnSlotCount;
        }
    }
}
