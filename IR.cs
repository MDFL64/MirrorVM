using System.Security.Cryptography.X509Certificates;

enum BlockKind {
    Block,
    Loop,
    If,
    Else,
    FunctionBody
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

    public abstract Type BuildMirror(Type body);

    public Block AddIntermediateBlock(int index) {
        var new_block = new Block(OwningBlock.LoopWeight); // weight shouldn't matter here
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

    public override Type BuildMirror(Type body) {
        var blocks = GetNextBlocks();
        var next = MirrorBuilder.MakeConstant(blocks[0].Index);

        return MirrorBuilder.MakeGeneric(typeof(TermJump<,>),[next,body]);
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

    public override Type BuildMirror(Type body) {
        var blocks = GetNextBlocks();
        var cond = Cond.BuildMirror();
        var t = MirrorBuilder.MakeConstant(blocks[0].Index);
        var f = MirrorBuilder.MakeConstant(blocks[1].Index);

        return MirrorBuilder.MakeGeneric(typeof(TermJumpIf<,,,>),[cond,t,f,body]);
    }

    public override void TraverseExpressions(Action<Expression> f) {
        Cond.Traverse(f);
    }
}

class JumpTable : BlockTerminator {
    public Expression Selector;

    public JumpTable(Block owner, Expression sel, List<Block> opts) : base(owner, opts.Count) {
        Selector = sel;
        for (int i=0;i<opts.Count;i++) {
            SetNextBlock(i, opts[i]);
        }
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

    public override Type BuildMirror(Type body)
    {
        var blocks = GetNextBlocks();
        var sel = Selector.BuildMirror();
        var jump_base = MirrorBuilder.MakeConstant(blocks[0].Index);
        var jump_count = MirrorBuilder.MakeConstant(blocks.Count);

        return MirrorBuilder.MakeGeneric(typeof(TermJumpTable<,,,>),[sel,jump_base,jump_count,body]);
    }
}

class Return : BlockTerminator {
    public Return(Block owner) : base(owner, 0) {}

    public override void SetFallThrough(Block b)
    {
        // do nothing
    }

    public override Type BuildMirror(Type body) {
        return MirrorBuilder.MakeGeneric(typeof(TermReturn<>),[body]);
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

    public override Type BuildMirror(Type body) {
        return typeof(TermTrap);
    }

    public override void TraverseExpressions(Action<Expression> f) {}
}

public class IRBody
{
    public Block Entry;
    public int ArgCount;
    public int RetCount;
    public int FrameSize;
    public int VarBase;
    public (int, LocalKind)[] RegisterMap;
}

public class Block {
    public string Name;
    public int Index = -1;
    public bool IsEntry = false;

    public long LoopWeight;

    static long NextBlockId = 0;

    public Block(long loop_weight) {
        Name = "Block"+NextBlockId;
        LoopWeight = loop_weight;
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
    public required BlockKind Kind;
    public required Block Block; // exit block
    public required Local[] SpillLocals;
    public required int ResultCount;
    public required int ExpressionStackBase;
    public Block ElseBlock;
    public Local[] IfParams;
}

enum CallKind {
    Static,
    Dynamic
}

class IRBuilder
{
    private int VariableCount = 0;
    private int SpillCount = 0;
    private int ReturnSlotCount = 0;
    private int CallSlotBase = 0;
    private int CallSlotTotalCount = 0;

    public Block InitialBlock = new Block(1);
    public Block CurrentBlock;

    public IRBuilder(List<ValType> local_types, List<ValType> ret_types)
    {
        VariableCount = local_types.Count;
        if (ret_types.Count > 1)
        {
            ReturnSlotCount = ret_types.Count - 1;
        }
        CurrentBlock = InitialBlock;
        CurrentBlock.IsEntry = true;

        var ret_spill_locals = new Local[ret_types.Count];
        var ret_block = new Block(1);
        ret_block.Terminator = new Return(ret_block);
        for (int i = 0; i < ret_spill_locals.Length; i++)
        {
            ret_spill_locals[i] = CreateSpillLocal(ret_types[i]);
            if (i >= 1)
            {
                ret_block.Statements.Add((new Local(i - 1, ret_types[i], LocalKind.Frame), ret_spill_locals[i]));
            }
        }
        if (ret_spill_locals.Length > 0)
        {
            ret_block.Statements.Add((new Local(0, ret_types[0], LocalKind.Register), ret_spill_locals[0]));
        }
        ReturnBlock = new BlockStackEntry
        {
            Block = ret_block,
            ExpressionStackBase = 0,
            Kind = BlockKind.FunctionBody,
            ResultCount = ret_types.Count,
            SpillLocals = ret_spill_locals
        };
    }

    private Stack<Expression> ExpressionStack = new Stack<Expression>();
    private int ExpressionStackBase = 0;

    // I hate this stupid damn language. JUST LET ME INDEX INTO A STACK, FUCK YOU!
    private List<BlockStackEntry> BlockStack = new List<BlockStackEntry>();
    BlockStackEntry ReturnBlock;

    public Local CreateSpillLocal(ValType ty)
    {
        var result = new Local(SpillCount, ty, LocalKind.Spill);
        SpillCount++;
        return result;
    }

    public void AddCall(FunctionType sig, string debug_name, CallKind kind, int func_or_sig_index, int table_index = 0)
    {
        var args = new List<Expression>();
        Expression index_expr = null;
        if (kind == CallKind.Dynamic)
        {
            index_expr = PopExpression();
        }

        for (int i = 0; i < sig.Inputs.Count; i++)
        {
            args.Add(PopExpression());
        }

        Expression call_expr;
        if (kind == CallKind.Dynamic)
        {
            call_expr = new CallIndirect(index_expr, CallSlotBase, args, func_or_sig_index, table_index);
        }
        else
        {
            call_expr = new Call(func_or_sig_index, CallSlotBase, args, debug_name);
        }

        SpillStack();
        if (sig.Outputs.Count == 0)
        {
            AddStatement(null, call_expr);
        }
        else
        {
            var out_ty = sig.Outputs[0];
            var output = CreateSpillLocal(out_ty);
            AddStatement(output.WithType(ValType.I64), call_expr);
            PushExpression(output);
            // multi-returns
            for (int i = 1; i < sig.Outputs.Count; i++)
            {
                PushExpression(new Local(CallSlotBase + i - 1, sig.Outputs[i], LocalKind.Call));
            }
        }

        int slots_used = int.Max(sig.Inputs.Count, sig.Outputs.Count - 1);
        CallSlotTotalCount = int.Max(CallSlotTotalCount, CallSlotBase + slots_used);

        if (sig.Outputs.Count > 1)
        {
            CallSlotBase += sig.Outputs.Count - 1;
        }
    }

    public void AddReturn(int ret_count)
    {
        var values = new Expression[ret_count];
        for (int i = 0; i < ret_count; i++)
        {
            values[i] = PopExpression();
        }
        Array.Reverse(values);
        for (int i = 1; i < values.Length; i++)
        {
            var ty = values[i].Type;
            AddStatement(new Local(i - 1, ty, LocalKind.Frame), values[i]);
        }
        // clobbers a register, must be last
        if (ret_count > 0)
        {
            var ty = values[0].Type;
            AddStatement(new Local(0, ty, LocalKind.Register), values[0]);
        }

        TerminateBlock(new Return(CurrentBlock));
    }

    public long GetTopBlockWeight()
    {
        if (BlockStack.Count == 0)
        {
            return 1;
        }
        return BlockStack.Last().Block.LoopWeight;
    }

    public void StartBlock(FunctionType block_ty)
    {
        Local[] spill_locals = new Local[block_ty.Outputs.Count];
        for (int i = 0; i < spill_locals.Length; i++)
        {
            spill_locals[i] = CreateSpillLocal(block_ty.Outputs[i]);
        }
        ExpressionStackBase = int.Max(ExpressionStack.Count - block_ty.Inputs.Count, ExpressionStackBase);
        BlockStack.Add(new BlockStackEntry
        {
            Kind = BlockKind.Block,
            Block = new Block( GetTopBlockWeight() ),
            SpillLocals = spill_locals,
            ResultCount = spill_locals.Length,
            ExpressionStackBase = ExpressionStackBase
        });
    }

    public void StartIf(FunctionType block_ty)
    {
        long weight = GetTopBlockWeight();
        var exit_block = new Block(weight);
        var else_block = new Block(weight);

        var cond = PopExpression();

        var if_params = new Local[block_ty.Inputs.Count];
        for (int i = 0; i < block_ty.Inputs.Count; i++)
        {
            if_params[i] = CreateSpillLocal(block_ty.Inputs[i]);
            AddStatement(if_params[i], PopExpression());
        }
        Array.Reverse(if_params);

        var if_term = new JumpIf(CurrentBlock, cond, else_block);
        TerminateBlock(if_term);
        if_term.Invert();

        Local[] spill_locals = new Local[block_ty.Outputs.Count];
        for (int i = 0; i < spill_locals.Length; i++)
        {
            spill_locals[i] = CreateSpillLocal(block_ty.Outputs[i]);
        }
        ExpressionStackBase = ExpressionStack.Count;
        foreach (var param in if_params)
        {
            PushExpression(param);
        }
        BlockStack.Add(new BlockStackEntry
        {
            Kind = BlockKind.If,
            Block = exit_block,
            ElseBlock = else_block,
            IfParams = if_params,
            SpillLocals = spill_locals,
            ResultCount = spill_locals.Length,
            ExpressionStackBase = ExpressionStackBase
        });
    }

    public void StartElse()
    {
        if (BlockStack.Count == 0)
        {
            throw new Exception("else is missing if");
        }
        var block_info = BlockStack[BlockStack.Count - 1];
        if (block_info.Kind != BlockKind.If)
        {
            throw new Exception("else is missing if");
        }
        SpillBlockResult(block_info);
        ResetExpressionStack();

        TerminateBlock(new Jump(CurrentBlock, block_info.Block));
        SwitchBlock(block_info.ElseBlock);
        block_info.Kind = BlockKind.Else;
        block_info.ElseBlock = null;

        foreach (var param in block_info.IfParams)
        {
            PushExpression(param);
        }
    }

    public void StartLoop(FunctionType block_ty)
    {
        Local[] spill_locals = new Local[block_ty.Inputs.Count];
        for (int i = 0; i < spill_locals.Length; i++)
        {
            spill_locals[i] = CreateSpillLocal(block_ty.Inputs[i]);
        }
        SpillBlockResult(spill_locals);

        var loop_block = new Block( GetTopBlockWeight() * Config.REG_ALLOC_LOOP_WEIGHT );
        SwitchBlock(loop_block);
        ExpressionStackBase = ExpressionStack.Count;
        foreach (var local in spill_locals)
        {
            PushExpression(local);
        }
        BlockStack.Add(new BlockStackEntry
        {
            Kind = BlockKind.Loop,
            Block = loop_block,
            SpillLocals = spill_locals,
            ResultCount = block_ty.Outputs.Count,
            ExpressionStackBase = ExpressionStackBase,
        });
    }

    public bool EndBlock()
    {
        if (BlockStack.Count == 0)
        {
            return true;
        }
        var block_info = BlockStack[BlockStack.Count - 1];
        BlockStack.RemoveAt(BlockStack.Count - 1);

        if (block_info.Kind == BlockKind.Block || block_info.Kind == BlockKind.If || block_info.Kind == BlockKind.Else)
        {
            SpillBlockResult(block_info);
            ResetExpressionStack();
            if (block_info.Kind == BlockKind.If)
            {
                if (block_info.ResultCount != 0)
                {
                    TerminateBlock(new Jump(CurrentBlock, block_info.Block));

                    // create a block which attempts to fix results
                    SwitchBlock(block_info.ElseBlock);
                    foreach (var param in block_info.IfParams)
                    {
                        PushExpression(param);
                    }
                    SpillBlockResult(block_info);
                }
                else
                {
                    // fix empty else block
                    SwitchBlock(block_info.ElseBlock);
                }
            }
            SwitchBlock(block_info.Block);
            foreach (var local in block_info.SpillLocals)
            {
                PushExpression(local);
            }
        }
        else if (block_info.Kind == BlockKind.Loop)
        {
            // ending a loop is a no-op
            // we shouldn't even need to spill the result to a temporary, since this is the only exit!
            ResetExpressionStack(block_info.ResultCount);
        }
        else
        {
            throw new Exception("todo end block " + block_info.Kind);
        }

        return false;
    }

    public void SpillBlockResult(BlockStackEntry target)
    {
        SpillBlockResult(target.SpillLocals);
    }

    public void SpillBlockResult(Local[] locals)
    {
        foreach (var local in locals.Reverse())
        {
            var res = PopExpression();
            AddStatement(local, res);
        }
    }

    public void TeeBlockResult(BlockStackEntry target)
    {
        foreach (var local in target.SpillLocals.Reverse())
        {
            var res = PopExpression();
            AddStatement(local, res);
        }
        foreach (var local in target.SpillLocals)
        {
            PushExpression(local);
        }
    }

    public BlockStackEntry GetBlock(int i)
    {
        int index = BlockStack.Count - 1 - i;
        if (index == -1)
        {
            return ReturnBlock;
        }
        return BlockStack[index];
    }

    public void PushExpression(Expression e)
    {
        ExpressionStack.Push(e);
    }

    public Expression PopExpression()
    {
        if (ExpressionStack.Count <= ExpressionStackBase)
        {
            return new ErrorExpression();
        }
        return ExpressionStack.Pop();
    }

    private void ResetExpressionStack(int extra = 0)
    {
        int target_size = ExpressionStackBase + extra;
        while (ExpressionStack.Count > target_size)
        {
            ExpressionStack.Pop();
        }
        if (ExpressionStack.Count < ExpressionStackBase)
        {
            throw new Exception("stack underflow");
        }
        //
        if (BlockStack.Count > 0)
        {
            ExpressionStackBase = BlockStack[BlockStack.Count - 1].ExpressionStackBase;
        }
        else
        {
            ExpressionStackBase = 0;
        }
    }

    public int GetExpressionStackSize()
    {
        return ExpressionStack.Count;
    }

    public void PushBinaryOp(BinaryOpKind kind)
    {
        var b = PopExpression();
        var a = PopExpression();
        PushExpression(new BinaryOp(kind, a, b));
    }

    public void PushMemoryRead(ValType res, MemSize size, int offset)
    {
        var addr = PopExpression();
        PushExpression(new MemoryOp(res, size, addr, offset));
    }

    public void AddMemoryWrite(ValType arg_ty, MemSize size, int offset)
    {
        var value = PopExpression();
        var addr = PopExpression();
        SpillMemoryReads();
        AddStatement(new MemoryOp(arg_ty, size, addr, offset), value);
    }

    public void AddStatement(Destination? dest, Expression expr)
    {
        CurrentBlock.Statements.Add((dest, expr));
    }

    public void AddDebug(string s)
    {
        //CurrentBlock.Statements.Add((null, new DebugExpression(s)));
    }

    public string DumpStack()
    {
        string res = "depth=" + ExpressionStack.Count + " [ ";
        var stack = ExpressionStack.ToArray();
        for (int i = 0; i < stack.Length; i++)
        {
            res += stack[i] + " ";
        }
        return res + "]";
    }

    public void TerminateBlock(BlockTerminator term)
    {
        SpillStack();

        CurrentBlock.Terminator = term;
        CurrentBlock = new Block( GetTopBlockWeight() );
        term.SetFallThrough(CurrentBlock);
    }

    private void SpillWhere(Func<Expression, bool> test)
    {
        var stack = ExpressionStack.ToArray();
        bool mutated = false;

        for (int i = 0; i < stack.Length; i++)
        {
            if (test(stack[i]))
            {
                var spill = CreateSpillLocal(stack[i].Type);
                AddStatement(spill, stack[i]);
                stack[i] = spill;

                mutated = true;
            }
        }

        if (mutated)
        {
            Array.Reverse(stack);
            ExpressionStack = new Stack<Expression>(stack);
        }
    }

    private void SpillStack()
    {
        SpillWhere(x => x.IsAnyRead());
    }

    public void SpillLocalVar(int index)
    {
        SpillWhere(x => x.IsLocalRead(index));
    }

    public void SpillGlobalVar(int index)
    {
        SpillWhere(x => x.IsGlobalRead(index));
    }

    public void SpillMemoryReads()
    {
        SpillWhere(x => x.IsMemoryRead());
    }

    private void SwitchBlock(Block block)
    {
        if (block == CurrentBlock)
        {
            throw new Exception("attempt to switch to current block");
        }
        if (block.Terminator != null || CurrentBlock.Terminator != null)
        {
            throw new Exception("neither block involved in switch should have a terminator");
        }
        if (block.Statements.Count > 0)
        {
            throw new Exception("attempt to switch to filled block");
        }
        SpillStack();

        CurrentBlock.Terminator = new Jump(CurrentBlock, block);
        CurrentBlock = block;
    }

    public void PruneBlocks()
    {
        HashSet<Block> Closed = new HashSet<Block>();
        Queue<Block> Open = new Queue<Block>();
        Open.Enqueue(InitialBlock);
        Closed.Add(InitialBlock);

        while (Open.Count > 0)
        {
            var block = Open.Dequeue();
            var next_blocks = block.Terminator.GetNextBlocks();

            bool force_enqueue_links = false;
            if (!block.IsEntry && block.Predecessors.Count == 0)
            {
                // KILL
                block.Delete();
                force_enqueue_links = true;
            }
            else if (block.IsTriviallyRedundant())
            {
                block.Delete();
            }

            foreach (var next in next_blocks)
            {
                if (force_enqueue_links || !Closed.Contains(next))
                {
                    Open.Enqueue(next);
                    Closed.Add(next);
                }
            }
            foreach (var prev in block.Predecessors)
            {
                if (force_enqueue_links || !Closed.Contains(prev))
                {
                    Open.Enqueue(prev);
                    Closed.Add(prev);
                }
            }
        }
    }

    public int GetFrameSize()
    {
        return ReturnSlotCount + CallSlotTotalCount + int.Max(VariableCount + SpillCount - Config.GetRegisterCount(), 0);
    }

    public int GetVarBase()
    {
        return ReturnSlotCount + CallSlotTotalCount;
    }

    private long[] RegisterWeights;
    private long CurrentLoopWeight = 1;
    public (int, LocalKind)[] RegisterMap;

    public void LowerLocals()
    {
        if (Config.REG_ALLOC_MODE == RegAllocMode.Enhanced)
        {
            RegisterWeights = new long[VariableCount + SpillCount];
        }

        var blocks = InitialBlock.GatherBlocks();
        foreach (var block in blocks)
        {
            //Console.WriteLine("weight = " + block.LoopWeight);
            CurrentLoopWeight = block.LoopWeight;
            foreach (var (dest, expr) in block.Statements)
            {
                if (dest != null)
                {
                    dest.Traverse(LowerLocal);
                }
                expr.Traverse(LowerLocal);
            }
            block.Terminator.TraverseExpressions(LowerLocal);
        }

        if (Config.REG_ALLOC_MODE == RegAllocMode.Enhanced)
        {
            var reg_priority = new int[RegisterWeights.Length];
            RegisterMap = new (int, LocalKind)[RegisterWeights.Length];

            for (int i = 0; i < RegisterWeights.Length; i++)
            {
                reg_priority[i] = i;
                //Console.WriteLine("reg" + i + ": " + RegisterWeights[i]);
            }

            Array.Sort(RegisterWeights, reg_priority);
            int next_index = 0;
            LocalKind next_kind = LocalKind.Register;
            for (int i = reg_priority.Length - 1; i >= 0; i--)
            {
                int reg_index = reg_priority[i];
                //Console.WriteLine("priority " + reg_index);
                RegisterMap[reg_index] = (next_index, next_kind);
                next_index++;
                if (next_kind == LocalKind.Register && next_index >= Config.GetRegisterCount())
                {
                    next_kind = LocalKind.Frame;
                    next_index = GetVarBase();
                }
            }

            /*for (int i = 0; i < RegisterMap.Length; i++)
            {
                Console.WriteLine("map" + i + ": " + RegisterMap[i]);
            }*/

            // pass 2, assign correct register numbers
            foreach (var block in blocks)
            {
                foreach (var (dest, expr) in block.Statements)
                {
                    if (dest != null)
                    {
                        dest.Traverse(LowerLocalPass2);
                    }
                    expr.Traverse(LowerLocalPass2);
                }
                block.Terminator.TraverseExpressions(LowerLocalPass2);
            }
        }
    }

    public static int TOTAL_REG = 0;
    public static int TOTAL_FRAME = 0;
    public static int[] FRAME_INDICES = new int[10_000];

    private void LowerLocal(Expression e)
    {
        if (e is Local local)
        {
            if (local.Kind == LocalKind.Variable)
            {
                local.Kind = LocalKind.Unallocated;
            }
            else if (local.Kind == LocalKind.Spill)
            {
                local.Kind = LocalKind.Unallocated;
                local.Index += VariableCount;
            }
            else if (local.Kind == LocalKind.Call)
            {
                local.Kind = LocalKind.Frame;
                local.Index += ReturnSlotCount;
            }
            else if (local.Kind == LocalKind.Register || local.Kind == LocalKind.Frame)
            {
                // generated by returns
                return;
            }
            else if (local.Kind == LocalKind.Unallocated)
            {
                // do nothing
            }
            else
            {
                Console.WriteLine("TODO FIX " + local.Kind);
            }
            // convert high registers to frame accesses
            if (Config.REG_ALLOC_MODE == RegAllocMode.Basic || Config.REG_ALLOC_MODE == RegAllocMode.None)
            {
                int reg_count = Config.GetRegisterCount();
                if (local.Kind == LocalKind.Unallocated && local.Index >= reg_count)
                {
                    int index = ReturnSlotCount + CallSlotTotalCount + (local.Index - reg_count);
                    local.Kind = LocalKind.Frame;
                    local.Index = index;
                    TOTAL_FRAME++;
                    FRAME_INDICES[index]++;
                }
                else
                {
                    local.Kind = LocalKind.Register;
                    TOTAL_REG++;
                }
            }
            else
            {
                // when using non-trivial reg alloc, we'll need to run another pass
                // this pass just records weights for variables
                if (local.Kind == LocalKind.Unallocated)
                {
                    RegisterWeights[local.Index] += CurrentLoopWeight;
                }
            }
        }
        if (e is Call call)
        {
            call.FrameIndex += ReturnSlotCount;
        }
        if (e is CallIndirect call2)
        {
            call2.FrameIndex += ReturnSlotCount;
        }
    }
    
    // writes the correct local kind into unallocated locals
    private void LowerLocalPass2(Expression e)
    {
        if (e is Local local)
        {
            if (local.Kind == LocalKind.Unallocated)
            {
                var (new_index, new_kind) = RegisterMap[local.Index];
                local.Kind = new_kind;
                local.Index = new_index;
            }
        }
    }
}
