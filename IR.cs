using System.Reflection.Metadata;

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
}

class Return : BlockTerminator {
    public Expression[] Values;

    public Return(Block owner, Expression[] values) : base(owner, 0) {
        Array.Reverse(values);
        Values = values;
    }

    public override void SetFallThrough(Block b)
    {
        // do nothing
    }

    public override Type BuildHell(Type body) {
        if (Values.Length == 0) {
            return HellBuilder.MakeGeneric(typeof(TermReturn_Void<>),[body]);
        }

        var value = Values[0];
        var value_hell = value.BuildHell();
        switch (value.Type) {
            case ValType.I32: return HellBuilder.MakeGeneric(typeof(TermReturn_I32<,>),[value_hell,body]);
            case ValType.I64: return HellBuilder.MakeGeneric(typeof(TermReturn_I64<,>),[value_hell,body]);
            case ValType.F32: return HellBuilder.MakeGeneric(typeof(TermReturn_F32<,>),[value_hell,body]);
            case ValType.F64: return HellBuilder.MakeGeneric(typeof(TermReturn_F64<,>),[value_hell,body]);

            default: throw new Exception("todo return: "+value.Type);
        }
    }

    public override string ToString()
    {
        string str = "Return(";
        for (int i=0;i<Values.Length;i++) {
            if (i != 0) {
                str += ",";
            }
            str += Values[i];
        }
        return str+")";
    }
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
    public abstract Type BuildHell(Type input, Type next);
}

enum LocalKind {
    // front-end kinds
    Variable, // front-end wasm variables
    Spill,    // spills from the virtual stack, inserted before barriers
    Return,   // additional return values, placed at the start of frame
    Call,     // call arguments and extra return values, placed at end of frame
    // back-end kinds
    Register, // locals that live in registers
    Frame,    // locals that live in the frame
}

class Local : Destination {
    int Index;
    ValType Type;
    LocalKind Kind;

    public Local(int index, ValType ty, LocalKind kind) {
        Type = ty;
        Index = index;
        Kind = kind;
    }

    public GetLocal CreateGet() {
        return new GetLocal(Index, Type, Kind);
    }

    public Local WithType(ValType ty) {
        return new Local(Index, ty, Kind);
    }

    public static string LocalToString(LocalKind kind, int index) {
        switch (kind) {
            case LocalKind.Variable: return "V"+index;
            case LocalKind.Spill:    return "S"+index;
            case LocalKind.Call:     return "C"+index;
            default: return kind+""+index;
        }
    }

    public override string ToString()
    {
        return Local.LocalToString(Kind,Index);
    }

    public override Type BuildHell(Type input, Type next) {
        Type base_ty;
        if (Type == ValType.I32) {
            switch (Index) {
                case 0: base_ty = typeof(SetR0_I32<,>); break;
                case 1: base_ty = typeof(SetR1_I32<,>); break;
                case 2: base_ty = typeof(SetR2_I32<,>); break;
                case 3: base_ty = typeof(SetR3_I32<,>); break;
                case 4: base_ty = typeof(SetR4_I32<,>); break;
                case 5: base_ty = typeof(SetR5_I32<,>); break;
                case 6: base_ty = typeof(SetR6_I32<,>); break;

                default: throw new Exception("register-set out of bounds");
            }
        } else if (Type == ValType.I64) {
            switch (Index) {
                case 0: base_ty = typeof(SetR0_I64<,>); break;
                case 1: base_ty = typeof(SetR1_I64<,>); break;
                case 2: base_ty = typeof(SetR2_I64<,>); break;
                case 3: base_ty = typeof(SetR3_I64<,>); break;
                case 4: base_ty = typeof(SetR4_I64<,>); break;
                case 5: base_ty = typeof(SetR5_I64<,>); break;
                case 6: base_ty = typeof(SetR6_I64<,>); break;

                default: throw new Exception("register-set out of bounds");
            }
        } else if (Type == ValType.F32) {
            switch (Index) {
                case 0: base_ty = typeof(SetR0_F32<,>); break;
                case 1: base_ty = typeof(SetR1_F32<,>); break;
                case 2: base_ty = typeof(SetR2_F32<,>); break;
                case 3: base_ty = typeof(SetR3_F32<,>); break;
                case 4: base_ty = typeof(SetR4_F32<,>); break;
                case 5: base_ty = typeof(SetR5_F32<,>); break;
                case 6: base_ty = typeof(SetR6_F32<,>); break;

                default: throw new Exception("register-set out of bounds");
            }
        } else {
            throw new Exception("todo locals-set "+Type);
        }
        return HellBuilder.MakeGeneric(base_ty,[input,next]);
    }
}

class MemoryWrite : Destination {
    private ValType ArgType;
    private MemSize Size;
    private int Offset;
    public Expression Addr;

    public MemoryWrite(ValType arg_ty, MemSize size, Expression addr, int offset) {
        ArgType = arg_ty;
        Addr = addr;
        Size = size;
        Offset = offset;
    }

    public override Type BuildHell(Type input, Type next)
    {
        Type base_ty = (ArgType,Size) switch {
            (ValType.I64,MemSize.SAME) => typeof(Memory_I64_Store<,,,>),
            
            (ValType.I32,MemSize.I8_S) => typeof(Memory_I32_Store8<,,,>),
            (ValType.I32,MemSize.I16_S) => typeof(Memory_I32_Store16<,,,>),
            
            (ValType.I64,MemSize.I8_S) => typeof(Memory_I64_Store8<,,,>),
            (ValType.I64,MemSize.I16_S) => typeof(Memory_I64_Store16<,,,>),
            (ValType.I64,MemSize.I32_S) => typeof(Memory_I64_Store32<,,,>),

            _ => throw new Exception("WRITE "+ArgType+" "+Size)
        };
        return HellBuilder.MakeGeneric(base_ty,[
            input,
            Addr.BuildHell(),
            HellBuilder.MakeConstant(Offset),
            next
        ]);
    }
}

class BlockStackEntry {
    public BlockKind Kind;
    public Block Block; // exit block
    public Block ElseBlock;
    public ValType Type;
    public Local? SpillLocal;
}

class IRBuilder {
    private int NextBlock = 2;

    private int SpillCount = 0;
    private int CallSlotBase = 0;
    private int CallSlotTotalCount = 0;

    public Block InitialBlock = new Block(1);
    public Block CurrentBlock;

    public IRBuilder(List<ValType> local_types) {
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
            PushExpression(output.CreateGet());
            // multi-returns
            for (int i=1;i<sig.Outputs.Count;i++) {
                PushExpression(new GetLocal(CallSlotBase + i - 1, sig.Outputs[i], LocalKind.Call));
            }
        }

        int slots_used = int.Max(sig.Inputs.Count,sig.Outputs.Count - 1);
        CallSlotTotalCount = int.Max(CallSlotTotalCount, CallSlotBase + slots_used);

        if (sig.Outputs.Count > 1) {
            CallSlotBase += sig.Outputs.Count - 1;
        }
    }

    public void StartBlock(ValType ty) {
        Local? spill_local = null;
        if (ty != ValType.Void) {
            spill_local = CreateSpillLocal(ty);
        }
        BlockStack.Add(new BlockStackEntry{
            Kind = BlockKind.Block,
            Block = new Block(NextBlock),
            Type = ty,
            SpillLocal = spill_local
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
            if (block_info.SpillLocal != null) {
                PushExpression(block_info.SpillLocal.CreateGet());
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
        if (target.SpillLocal != null) {
            var res = PopExpression();
            AddStatement(target.SpillLocal,res);
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

    public void PushMemoryRead(ValType res, MemSize size, int offset) {
        var addr = PopExpression();
        PushExpression(new MemoryRead(res,size,addr,offset));
    }

    public void AddMemoryWrite(ValType arg_ty, MemSize size, int offset) {
        var value = PopExpression();
        var addr = PopExpression();
        AddStatement(new MemoryWrite(arg_ty,size,addr,offset),value);
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

    public void Dump(string name, bool draw_backlinks) {
        HashSet<Block> Closed = new HashSet<Block>();
        Queue<Block> Open = new Queue<Block>();
        Open.Enqueue(InitialBlock);
        Closed.Add(InitialBlock);

        // theme stolen from https://cprimozic.net/notes/posts/basic-graphviz-dark-theme-config/
        string result = """
        digraph {
            bgcolor="#181818";

            node [
                fontname = "Consolas";
                fontcolor = "#e6e6e6",
                style = filled,
                color = "#e6e6e6",
                fillcolor = "#333333"
            ]

            edge [
                fontname = "Arial";
                color = "#e6e6e6",
                fontcolor = "#e6e6e6"
            ]
        """;

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

        File.WriteAllText("graph/"+name+".dot",result);
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
