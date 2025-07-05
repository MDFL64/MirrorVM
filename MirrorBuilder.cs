using System.Drawing.Interop;

class MirrorBuilder {
    public static ICallable Compile(IRBody ir_body, string dump_name = null)
    {
        ControlFlowOptimizer.Optimize(ir_body.Entry, dump_name);

        var body = CompileBody(ir_body.Entry, dump_name);

        List<Type> func_args = [body.GetType()];

        var func_ty = MakeGeneric(typeof(Function<>), func_args.ToArray());
        var func = (ICallable)Activator.CreateInstance(func_ty);
        func.SetBody(body);
        return func;
    }

    private static object CompileBody(Block initial_block, string dump_name)
    {
        Type result_ty;
        if (initial_block.Terminator is Return)
        {
            DebugIR.Dump(initial_block, dump_name, false);

            result_ty = CompileStatements(initial_block.Statements);
            return Activator.CreateInstance(result_ty);
        }

        var blocks = initial_block.GatherBlocks();
        var ordered_blocks = new List<Block>();

        // clear indices
        foreach (var block in blocks)
        {
            block.Index = -1;
        }

        // entry block is zero
        blocks[0].Index = 0;
        ordered_blocks.Add(blocks[0]);

        // number jump tables
        foreach (var block in blocks)
        {
            if (block.Terminator is JumpTable jt)
            {
                var next_blocks = jt.GetNextBlocks();
                for (int i = 0; i < next_blocks.Count; i++)
                {
                    var next = next_blocks[i];
                    if (next.Index != -1)
                    {
                        next = jt.AddIntermediateBlock(i);
                    }
                    next.Index = ordered_blocks.Count;
                    ordered_blocks.Add(next);
                }
            }
        }
        // number remaining blocks
        foreach (var block in blocks)
        {
            if (block.Index == -1)
            {
                block.Index = ordered_blocks.Count;
                ordered_blocks.Add(block);
            }
        }

        if (dump_name != null && ordered_blocks.Count <= 100)
        {
            DebugIR.Dump(initial_block, dump_name, false);
        }

        List<Type> CompiledBlocks = new List<Type>();
        foreach (var block in ordered_blocks)
        {
            Type block_ty = CompileStatements(block.Statements);

            var final_ty = block.Terminator.BuildMirror(block_ty);
            //Console.WriteLine("> term "+DebugType(final_ty));
            CompiledBlocks.Add(final_ty);
        }

        int block_limit;
        Type dispatch_loop_type;
        if (CompiledBlocks.Count <= 10)
        {
            block_limit = 10;
            dispatch_loop_type = typeof(DispatchLoop10<,,,,,,,,,>);
        }
        else if (CompiledBlocks.Count <= 25)
        {
            block_limit = 25;
            dispatch_loop_type = typeof(DispatchLoop25<,,,,,,,,,,,,,,,,,,,,,,,,>);
        }
        else if (CompiledBlocks.Count <= 50)
        {
            block_limit = 50;
            dispatch_loop_type = typeof(DispatchLoop50<,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,>);
        }
        else if (CompiledBlocks.Count <= 100)
        {
            block_limit = 100;
            dispatch_loop_type = typeof(DispatchLoop100<,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,>);
        }
        else
        {
            block_limit = 200;
            dispatch_loop_type = typeof(DispatchLoop200<,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,>);
        }
        if (CompiledBlocks.Count > block_limit /*|| true*/)
        {
            //Console.WriteLine("block count = " + CompiledBlocks.Count);
            var result = new DispatchLoopArray();
            result.Blocks = new Terminator[CompiledBlocks.Count];
            for (int i = 0; i < CompiledBlocks.Count; i++)
            {
                result.Blocks[i] = (Terminator)Activator.CreateInstance(CompiledBlocks[i]);
            }

            return result;
        }
        while (CompiledBlocks.Count < block_limit)
        {
            CompiledBlocks.Add(typeof(TermVoid));
        }

        result_ty = MakeGeneric(dispatch_loop_type, CompiledBlocks.ToArray());
        return Activator.CreateInstance(result_ty);
    }

    static int BASE_TIER = 0;

    public static Type CompileStatements(List<(Destination, Expression)> stmts)
    {
        if (stmts.Count == 0)
        {
            return typeof(End);
        }

        if (SPLIT_ADVANCED)
        {
            List<(Destination, Expression)> current_batch = [];
            int current_cost = 0;
            List<List<(Destination, Expression)>> batches = [];
            foreach (var stmt in stmts)
            {
                int cost = GetCost(ConvertStatement(0, stmt));
                if (current_cost + cost > 800)
                {
                    batches.Add(current_batch);
                    current_batch = [stmt];
                    current_cost = cost;
                }
                else
                {
                    current_batch.Add(stmt);
                    current_cost += cost;
                }
            }
            batches.Add(current_batch);

            if (batches.Count > 1)
            {
                object prev_tree = null;
                for (int i = batches.Count - 1; i >= 0; i--)
                {
                    List<object> nodes = [..batches[i]];
                    if (prev_tree != null)
                    {
                        if (prev_tree is StatementNode node)
                        {
                            node.NoInline = true;
                        }
                        nodes.Add(prev_tree);
                    }
                    prev_tree = CreateTree(nodes);
                }

                return ConvertStatementTree(BASE_TIER, prev_tree);
            }
        }

        // Constructing a temporary tree helps to number tiers

        var tree = CreateTree([..stmts]);
        return ConvertStatementTree(BASE_TIER, tree);
    }

    private static Type ConvertStatement(int tier, (Destination, Expression) stmt_pair)
    {
        var (dest, source) = stmt_pair;

        if (dest == null)
        {
            int old_base;
            if (source is StatementExpression stmt)
            {
                old_base = BASE_TIER;
                BASE_TIER = tier + 1;
                var res = stmt.BuildStatement();
                BASE_TIER = old_base;
                return res;
            }

            old_base = BASE_TIER;
            BASE_TIER = tier + 1;
            var source_ty = source.BuildMirror();
            BASE_TIER = old_base;

            var val_ty = ConvertValType(source.Type);
            return MakeGeneric(typeof(ExprStmt<,>), [source_ty, val_ty]);
        }
        else
        {
            int old_base = BASE_TIER;
            BASE_TIER = tier + 1;
            var source_ty = source.BuildMirror();
            BASE_TIER = old_base;

            return dest.BuildDestination(source_ty);
        }
    }

    private static object CreateTree(List<object> nodes)
    {
        if (nodes.Count == 0)
        {
            throw new ArgumentException("can't create tree from zero nodes");
        }

        while (nodes.Count > 1)
        {
            int index = 0;
            List<object> new_nodes = [];

            while (index < nodes.Count)
            {
                var node = new StatementNode();
                for (int i = 0; i < 4; i++)
                {
                    if (index < nodes.Count)
                    {
                        node.Children[i] = nodes[index];
                        index++;
                    }
                    else
                    {
                        break;
                    }
                }
                new_nodes.Add(node);
            }

            nodes = new_nodes;
        }
        return nodes[0];
    }

    private static Type ConvertStatementTree(int tier, object target)
    {
        var end = typeof(End);
        if (target is StatementNode node)
        {
            Type bundle_ty = (tier % 8) switch
            {
                0 => typeof(Stmts1<,,,>),
                1 => typeof(Stmts2<,,,>),
                2 => typeof(Stmts3<,,,>),
                3 => typeof(Stmts4<,,,>),
                4 => typeof(Stmts5<,,,>),
                5 => typeof(Stmts6<,,,>),
                6 => typeof(Stmts7<,,,>),
                7 => typeof(Stmts8<,,,>),
                _ => null
            };

            var bundle_args = new Type[4];
            var costs = new int[4];
            int sum = 0;
            for (int i = 0; i < 4; i++)
            {
                if (node.Children[i] != null)
                {
                    bundle_args[i] = ConvertStatementTree(tier + 1, node.Children[i]);
                }
                else
                {
                    bundle_args[i] = end;
                }
                costs[i] = GetCost(bundle_args[i]);
                sum += costs[i];
            }

            /*if (ALT_INLINE_SPLIT)
            {
                while (sum > 900)
                {
                    Console.WriteLine("cost too high: " + sum);
                    int max_value = 0;
                    int max_index = 0;

                    for (int i = 0; i < 4; i++)
                    {
                        Console.WriteLine(i + ": " + costs[i]);
                        if (costs[i] > max_value)
                        {
                            max_value = costs[i];
                            max_index = i;
                        }
                    }

                    Console.WriteLine("max = " + max_index);

                    sum -= costs[max_index];
                    costs[max_index] = 0;
                    bundle_args[max_index] = MakeGeneric(typeof(NoInline<>), [bundle_args[max_index]]);
                }
            }*/

            var result = MakeGeneric(bundle_ty, bundle_args);
            if (SPLIT_SIMPLE)
            {
                var cost = GetCost(result);
                if (cost > MAX_COST)
                {
                    result = MakeGeneric(typeof(NoInline<>), [result]);
                }
            }

            if (node.NoInline)
            {
                Console.WriteLine("NO inline");
                result = MakeGeneric(typeof(NoInline<>), [result]);
            }

            return result;
        }
        else
        {
            return ConvertStatement(tier, ((Destination, Expression))target);
        }
        throw new ArgumentException("bad target " + target);
    }

    const bool SPLIT_ADVANCED = false;
    const bool SPLIT_SIMPLE = true;
    const int MAX_COST = 250;
    //const int MAX_COST = 800;

    private static int GetCost(Type base_ty)
    {
        Stack<Type> types = new Stack<Type>();
        types.Push(base_ty);

        int sum = 0;
        while (types.Count > 0)
        {
            var ty = types.Pop();
            sum += 3; // TODO
            if (ty.IsConstructedGenericType && ty.GetGenericTypeDefinition() == typeof(NoInline<>))
            {
                // skip
                continue;
            }
            foreach (var arg in ty.GetGenericArguments())
            {
                types.Push(arg);
            }
        }

        return sum;
    }

    public static Type ConvertValType(ValType ty) {
        switch (ty) {
            case ValType.I32: return typeof(int);
            case ValType.I64: return typeof(long);
            case ValType.F32: return typeof(float);
            case ValType.F64: return typeof(double);
        }
        throw new Exception("todo convert type "+ty);
    }

    public static Type MakeGeneric(Type base_ty, Type[] args) {
        #if SANDBOX
        for (int i=0;i<100;i++) {
            var bty = TypeLibrary.GetType(base_ty);
            if (bty == null) {
                Log.Info("retry "+base_ty);
                continue;
            }
            return bty.MakeGenericType(args);
        }
        throw new Exception("bad basetype "+base_ty);
        #else
        return base_ty.MakeGenericType(args);
        #endif
	}

    public static Type MakeConstant(long x) {
        if (x < 0 && x != long.MinValue) {
            return MakeGeneric(typeof(Neg<>),[MakeConstant(-x)]);
        }
        ulong n = (ulong)x;
        if (n < 16) {
            return GetDigit(n);
        } else if (n < 256) {
            return MakeGeneric(typeof(Num<,>),[GetDigit(n>>4),GetDigit(n)]);
        } else if (n < 65536) {
            return MakeGeneric(typeof(Num<,,,>),[
                GetDigit(n>>12),GetDigit(n>>8),GetDigit(n>>4),GetDigit(n)
            ]);
        } else if (n < 4294967296) {
            return MakeGeneric(typeof(Num<,,,,,,,>),[
                GetDigit(n>>28),GetDigit(n>>24),GetDigit(n>>20),GetDigit(n>>16),
                GetDigit(n>>12),GetDigit(n>>8),GetDigit(n>>4),GetDigit(n)
            ]);
        } else {
            return MakeGeneric(typeof(Num<,,,,,,,,,,,,,,,>),[
                GetDigit(n>>60),GetDigit(n>>56),GetDigit(n>>52),GetDigit(n>>48),
                GetDigit(n>>44),GetDigit(n>>40),GetDigit(n>>36),GetDigit(n>>32),
                GetDigit(n>>28),GetDigit(n>>24),GetDigit(n>>20),GetDigit(n>>16),
                GetDigit(n>>12),GetDigit(n>>8),GetDigit(n>>4),GetDigit(n)
            ]);
        }
    }

    private static Type GetDigit(ulong n) {
        switch (n & 0xF) {
            case 0: return typeof(D0);
            case 1: return typeof(D1);
            case 2: return typeof(D2);
            case 3: return typeof(D3);
            case 4: return typeof(D4);
            case 5: return typeof(D5);
            case 6: return typeof(D6);
            case 7: return typeof(D7);
            case 8: return typeof(D8);
            case 9: return typeof(D9);
            case 0xA: return typeof(DA);
            case 0xB: return typeof(DB);
            case 0xC: return typeof(DC);
            case 0xD: return typeof(DD);
            case 0xE: return typeof(DE);
            case 0xF: return typeof(DF);
        }
        throw new Exception("die");
    }

    public static string DebugType(Type ty) {
        var args = ty.GetGenericArguments();
        string res = ty.Name.Split('`')[0];
        if (args.Length > 0) {
            res += "<";
            for (int i=0;i<args.Length;i++) {
                if (i != 0) {
                    res += ',';
                }
                res += DebugType(args[i]);
            }
            res += ">";
        }

        return res;
    }
}

class StatementNode
{
    public int Tier;
    public bool NoInline;
    public object[] Children = new object[4];
}
