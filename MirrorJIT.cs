class MirrorJIT
{
    public static void Compile(BlockInfo[] blocks, int start_block)
    {
        List<int> jit_blocks = [start_block];
        int current_block = start_block;
        int loop_head = -1;

        for (; ; )
        {
            //Console.WriteLine("block " + current_block);
            int next = blocks[current_block].GetNextBlock();
            //Console.WriteLine("-> " + next);

            if (next > 0 && !blocks[next].JitCompiled)
            {
                if (jit_blocks.Contains(next))
                {
                    loop_head = next;
                    break;
                }
                jit_blocks.Add(next);
                current_block = next;
            }
            else
            {
                break;
            }
        }

        foreach (int block_id in jit_blocks)
        {
            if (block_id == loop_head)
            {
                Console.WriteLine("block " + block_id + " [loop head]");
            }
            else
            {
                Console.WriteLine("block " + block_id);
            }
        }

        int next_block_index = -1;
        JitBlock next_block = null;

        jit_blocks.Reverse();
        foreach (int block_index in jit_blocks)
        {
            var block = blocks[block_index].OriginalBlock;
            if (next_block == null)
            {
                // clone the block in case the next step modifies it
                next_block = JitBlock.FromBlock(block);
            }
            else
            {
                var next_blocks = block.Terminator.GetNextBlocks();
                if (block.Terminator is JumpIf jump)
                {
                    int true_index = next_blocks[0].Index;
                    int false_index = next_blocks[1].Index;

                    if (true_index == next_block_index)
                    {
                        var term = new JitIf(jump.Cond, next_block, false_index);
                        next_block = JitBlock.FromBlock(block);
                        next_block.Terminator = term;
                    }
                    else if (false_index == next_block_index)
                    {
                        var term = new JitIf(jump.Cond, true_index, next_block);
                        next_block = JitBlock.FromBlock(block);
                        next_block.Terminator = term;
                    }
                    else
                    {
                        throw new Exception("inconsistent trace");
                    }
                }
                else if (block.Terminator is Jump)
                {
                    next_block.Statements = [.. block.Statements.Concat(next_block.Statements)];
                }
                else if (block.Terminator is JumpTable)
                {
                    // todo
                    Console.WriteLine("skipping block with unsupported terminator");
                    blocks[start_block].JitCompiled = true;
                    ResetBlocks(blocks);
                    return;
                }
                else
                {
                    throw new Exception("unsupported terminator " + block.Terminator);
                }
            }

            if (loop_head == block_index)
            {
                var term = new JitLoop(block_index, next_block);
                next_block = new JitBlock();
                next_block.Terminator = term;
            }

            next_block_index = block_index;
        }

        var ty = next_block.Compile();
        var new_instance = (Terminator)Activator.CreateInstance(ty);
        blocks[start_block].Block = new_instance;
        blocks[start_block].JitCompiled = true;

        ResetBlocks(blocks);
    }

    private static void ResetBlocks(BlockInfo[] blocks)
    {
        for (int i = 0; i < blocks.Length; i++)
        {
            blocks[i].EntryCount = 0;
        }
    }
}

class JitBlock {
    public List<(Destination?, Expression)> Statements = new List<(Destination?, Expression)>();
    public JitTerminator Terminator;
    public BlockTerminator SimpleTerminator;

    public static JitBlock FromBlock(Block b)
    {
        var result = new JitBlock();

        result.SimpleTerminator = b.Terminator;
        result.Statements = [.. b.Statements];

        return result;
    }

    public Type Compile()
    {
        Type stmts_ty = MirrorBuilder.CompileStatements(Statements);

        if (Terminator != null)
        {
            return Terminator.BuildMirror(stmts_ty);
        }
        else
        {
            return SimpleTerminator.BuildMirror(stmts_ty);
        }
    }
}

abstract class JitTerminator {
    public abstract Type BuildMirror(Type stmts_ty);
}

class JitIf : JitTerminator
{
    Expression Cond;
    JitBlock PredictedBlock;
    int BailBlockIndex;
    bool ExpectedCond;

    public JitIf(Expression cond, JitBlock t_block, int f_index)
    {
        Cond = cond;
        ExpectedCond = true;
        PredictedBlock = t_block;
        BailBlockIndex = f_index;
    }
    
    public JitIf(Expression cond, int t_index, JitBlock f_block)
    {
        Cond = cond;
        ExpectedCond = false;
        PredictedBlock = f_block;
        BailBlockIndex = t_index;
    }

    public override Type BuildMirror(Type stmts_ty)
    {
        if (ExpectedCond)
        {
            return MirrorBuilder.MakeGeneric(typeof(TermJitExpectTrue<,,,>), [
                stmts_ty,
                Cond.BuildMirror(),
                PredictedBlock.Compile(),
                MirrorBuilder.MakeConstant(BailBlockIndex),
            ]);
        }
        else
        {
            return MirrorBuilder.MakeGeneric(typeof(TermJitExpectFalse<,,,>), [
                stmts_ty,
                Cond.BuildMirror(),
                MirrorBuilder.MakeConstant(BailBlockIndex),
                PredictedBlock.Compile(),
            ]);
        }
    }
}

class JitLoop : JitTerminator
{
    int BlockIndex;
    JitBlock Inner;

    public JitLoop(int block_index, JitBlock inner)
    {
        BlockIndex = block_index;
        Inner = inner;
    }

    public override Type BuildMirror(Type stmts_ty)
    {
        return MirrorBuilder.MakeGeneric(typeof(TermJitLoop<,,>), [
            stmts_ty,
            MirrorBuilder.MakeConstant(BlockIndex),
            Inner.Compile()
        ]);
    }
}
