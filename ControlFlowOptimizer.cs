using Wacs.Core.Types;

public static class ControlFlowOptimizer
{
    public static void Optimize(Block initial_block, string dump_name)
    {
        //return;
        Console.WriteLine(">>> " + dump_name);
        int dump_index = 0;

        if (dump_name != null)
        {
            DebugIR.Dump(initial_block, dump_name+"_opt_"+dump_index, true);
        }
        dump_index++;

        var blocks = initial_block.GatherBlocks();

        for (int i = blocks.Count - 1; i >= 0; i--)
        {
            var block = blocks[i];
            if (!block.IsDeleted)
            {
                string result = null;
                do
                {
                    result = OptimizeBlock(block);
                    if (result != null)
                    {
                        Console.WriteLine(" - transform " + dump_index + ": " + result);
                        if (dump_name != null)
                        {
                            DebugIR.Dump(initial_block, dump_name + "_opt_" + dump_index, true);
                        }
                        dump_index++;
                    }
                } while (result != null);
                /*var res2 = OptimizeBlock(block);
                if (res2 != null)
                {
                    throw new Exception("a double");
                }*/
            }
        }
    }

    private static string OptimizeBlock(Block base_block)
    {
        if (base_block.Terminator is Jump base_jump)
        {
            var next_block = base_jump.GetNextBlocks()[0];

            if (next_block.HasUniquePredecessor(base_block))
            {
                base_block.Terminator.Destroy();
                next_block.Delete();

                foreach (var stmt in next_block.Statements)
                {
                    base_block.Statements.Add(stmt);   
                }
                next_block.Terminator.ChangeOwner(base_block);
                base_block.Terminator = next_block.Terminator;

                return "merge";
            }
        }

        if (base_block.Terminator is JumpIf base_if)
        {
            var next_blocks = base_if.GetNextBlocks();

            var term_0 = next_blocks[0].Terminator;
            var term_1 = next_blocks[1].Terminator;

            // case 1: both sides can be subsumed
            if (next_blocks[0].HasUniquePredecessor(base_block) && next_blocks[1].HasUniquePredecessor(base_block))
            {
                if (term_0 is Jump && term_1 is Jump)
                {
                    var final_block = term_0.GetNextBlocks()[0];
                    if (final_block == term_1.GetNextBlocks()[0])
                    {
                        var if_stmt = new IfStatement();
                        if_stmt.Cond = base_if.Cond;
                        if_stmt.StmtsThen = next_blocks[0].Statements;
                        if_stmt.StmtsElse = next_blocks[1].Statements;

                        base_block.Statements.Add((null, if_stmt));

                        // replace base_block.Terminator with unconditional jump to final block
                        base_block.Terminator = new Jump(base_block, final_block);

                        // final block predecessors replaced with base block
                        next_blocks[0].Delete();
                        next_blocks[1].Delete();

                        return "if-dual-block";
                    }
                }

            }
            // case 2A: "then" side can be subsumed
            if (next_blocks[0].HasUniquePredecessor(base_block) && term_0 is Jump)
            {
                var final_block = term_0.GetNextBlocks()[0];
                if (final_block == next_blocks[1])
                {
                    throw new Exception("then subsumed");
                }
            }
            // case 2B: "else" side can be subsumed
            if (next_blocks[1].HasUniquePredecessor(base_block) && term_1 is Jump)
            {
                var final_block = term_1.GetNextBlocks()[0];
                if (final_block == next_blocks[0])
                {
                    var if_stmt = new IfStatement();
                    if_stmt.Cond = base_if.Cond;
                    if_stmt.StmtsThen = [];
                    if_stmt.StmtsElse = next_blocks[1].Statements;

                    base_block.Statements.Add((null, if_stmt));

                    // replace base_block.Terminator with unconditional jump to final block
                    base_block.Terminator.Destroy();
                    base_block.Terminator = new Jump(base_block, final_block);

                    // final block predecessors replaced with base block
                    next_blocks[1].Delete();

                    return "if-else-block";
                }
            }
            // case 3A: "then" side traps
            if (next_blocks[0].HasUniquePredecessor(base_block) && term_0 is Trap)
            {
                var if_stmt = new IfStatement();
                if_stmt.Cond = base_if.Cond;
                if_stmt.StmtsThen = next_blocks[0].Statements;
                if_stmt.StmtsElse = [];

                if_stmt.StmtsThen.Add((null, new TrapExpression()));

                base_block.Statements.Add((null, if_stmt));

                base_block.Terminator.Destroy();
                base_block.Terminator = new Jump(base_block, next_blocks[1]);

                next_blocks[0].Delete();

                return "if-then-trap";
            }
            // case 3B: "else" side traps
            if (next_blocks[1].HasUniquePredecessor(base_block) && term_1 is Trap)
            {
                var if_stmt = new IfStatement();
                if_stmt.Cond = base_if.Cond;
                if_stmt.StmtsThen = [];
                if_stmt.StmtsElse = next_blocks[1].Statements;

                if_stmt.StmtsElse.Add((null, new TrapExpression()));

                base_block.Statements.Add((null, if_stmt));

                base_block.Terminator.Destroy();
                base_block.Terminator = new Jump(base_block, next_blocks[0]);

                next_blocks[1].Delete();

                return "if-else-trap";
            }

            // loop while true
            if (next_blocks[0] == base_block)
            {
                var loop_stmt = new LoopStatement();
                loop_stmt.Cond = base_if.Cond;
                loop_stmt.LoopValue = true;
                loop_stmt.Stmts = [.. base_block.Statements];

                base_block.Statements = [(null, loop_stmt)];
                base_block.Terminator.Destroy();
                base_block.Terminator = new Jump(base_block, next_blocks[1]);

                return "loop-true";
            }
            if (next_blocks[1] == base_block)
            {
                throw new Exception("loop-false");
            }
        }

        return null;
    }
}
