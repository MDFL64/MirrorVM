using System.Drawing.Interop;

class MirrorBuilder {
    public static ICallable Compile(IRBody ir_body, string dump_name = null)
    {
        ControlFlowOptimizer.Optimize(ir_body.Entry, dump_name);

        var body = CompileBody(ir_body.Entry, dump_name);

        List<Type> func_args = [body];

        // arg setup
        {
            Type arg_read = typeof(ArgReadNone);
            if (Config.REG_ALLOC_MODE == RegAllocMode.Basic)
            {
                for (int i = 0; i < ir_body.ArgCount; i++)
                {
                    arg_read = i switch
                    {
                        0 => MakeGeneric(typeof(ArgReadR0<,>), [MakeConstant(i), arg_read]),
                        1 => MakeGeneric(typeof(ArgReadR1<,>), [MakeConstant(i), arg_read]),
                        2 => MakeGeneric(typeof(ArgReadR2<,>), [MakeConstant(i), arg_read]),
                        3 => MakeGeneric(typeof(ArgReadR3<,>), [MakeConstant(i), arg_read]),
                        4 => MakeGeneric(typeof(ArgReadR4<,>), [MakeConstant(i), arg_read]),
                        5 => MakeGeneric(typeof(ArgReadR5<,>), [MakeConstant(i), arg_read]),
                        6 => MakeGeneric(typeof(ArgReadR6<,>), [MakeConstant(i), arg_read]),
                        _ => MakeGeneric(typeof(ArgReadFrame<,,>), [
                            MakeConstant(i),
                            MakeConstant(ir_body.VarBase + i - Config.GetRegisterCount()),
                            arg_read
                        ])
                    };
                }
            }
            else if (Config.REG_ALLOC_MODE == RegAllocMode.Enhanced)
            {
                for (int i = 0; i < ir_body.ArgCount; i++)
                {
                    var (arg_index, arg_kind) = ir_body.RegisterMap[i];
                    if (arg_kind == LocalKind.Register)
                    {
                        //Console.WriteLine("read arg " + i + " to reg " + arg_index);
                        arg_read = arg_index switch
                        {
                            0 => MakeGeneric(typeof(ArgReadR0<,>), [MakeConstant(i), arg_read]),
                            1 => MakeGeneric(typeof(ArgReadR1<,>), [MakeConstant(i), arg_read]),
                            2 => MakeGeneric(typeof(ArgReadR2<,>), [MakeConstant(i), arg_read]),
                            3 => MakeGeneric(typeof(ArgReadR3<,>), [MakeConstant(i), arg_read]),
                            4 => MakeGeneric(typeof(ArgReadR4<,>), [MakeConstant(i), arg_read]),
                            5 => MakeGeneric(typeof(ArgReadR5<,>), [MakeConstant(i), arg_read]),
                            6 => MakeGeneric(typeof(ArgReadR6<,>), [MakeConstant(i), arg_read]),
                            _ => throw new Exception()
                        };
                    }
                    else
                    {
                        //Console.WriteLine("read arg " + i + " to frame " + arg_index);
                        arg_read = MakeGeneric(typeof(ArgReadFrame<,,>), [
                            MakeConstant(i),
                            MakeConstant(arg_index),
                            arg_read
                        ]);
                    }
                }
            }
            else
            {
                for (int i = 0; i < ir_body.ArgCount; i++)
                {
                    arg_read = MakeGeneric(typeof(ArgReadFrame<,,>), [
                        MakeConstant(i),
                        MakeConstant(ir_body.VarBase + i - Config.GetRegisterCount()),
                        arg_read
                    ]);
                }
            }
            func_args.Add(arg_read);
        }
        // result setup
        {
            int extra_rets = ir_body.RetCount > 1 ? ir_body.RetCount - 1 : 0;
            func_args.Add(MakeConstant(extra_rets));
        }
        // frame size
        {
            func_args.Add(MakeConstant(ir_body.FrameSize));
        }

        var func = MakeGeneric(typeof(Function<,,,>), func_args.ToArray());
        return (ICallable)Activator.CreateInstance(func);
    }

    private static Type CompileBody(Block initial_block, string dump_name)
    {
        if (initial_block.Terminator is Return)
        {
            return CompileStatements(initial_block.Statements);
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
        int block_limit = 200;
        while (CompiledBlocks.Count < block_limit)
        {
            CompiledBlocks.Add(typeof(TermVoid));
        }
        if (CompiledBlocks.Count > block_limit)
        {
            Console.WriteLine("block count = " + CompiledBlocks.Count);
        }

        return MakeGeneric(typeof(DispatchLoop200<
            ,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,
            ,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,
        >), CompiledBlocks.ToArray());
    }

    static int BASE_TIER = 0;

    public static Type CompileStatements(List<(Destination, Expression)> stmts)
    {
        // Constructing a temporary tree helps to number tiers
        List<object> nodes = [.. stmts];
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

        if (nodes.Count == 0)
        {
            return typeof(End);
        }

        return ConvertStatementNodes(BASE_TIER, nodes[0]);
    }

    private static Type ConvertStatementNodes(int tier, object target)
    {
        var end = typeof(End);
        if (target is (null, Expression expr))
        {
            int old_base = BASE_TIER;
            BASE_TIER = tier + 1;
            var source_ty = expr.BuildMirror();
            BASE_TIER = old_base;

            var val_ty = ConvertValType(expr.Type);
            return MakeGeneric(typeof(ExprStmt<,,>), [source_ty, val_ty, end]);
        }
        else if (target is (Destination dest, Expression source))
        {
            int old_base = BASE_TIER;
            BASE_TIER = tier + 1;
            var source_ty = source.BuildMirror();
            BASE_TIER = old_base;

            return dest.BuildDestination(source_ty, end);
        }
        else if (target is StatementNode node)
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
            for (int i = 0; i < 4; i++)
            {
                if (node.Children[i] != null)
                {
                    bundle_args[i] = ConvertStatementNodes(tier + 1, node.Children[i]);
                }
                else
                {
                    bundle_args[i] = end;
                }
            }

            return MakeGeneric(bundle_ty, bundle_args);
        }
        throw new ArgumentException("bad target "+target);
    }

    const int MAX_COST = 800;

    public static Type CompileStatementsOld(List<(Destination, Expression)> stmts) {
        List<List<Type>> bundles = new List<List<Type>>();
        var end = typeof(End);

        List<Type> current_bundle = new List<Type>();
        int current_cost = 0;

        foreach (var (dest,source) in stmts) {
            if (source is DebugExpression) {
                continue;
            }

            var source_ty = source?.BuildMirror();
            Type stmt_ty;
            if (dest != null) {
                stmt_ty = dest.BuildDestination(source_ty, end);
            } else {
                var val_ty = ConvertValType(source.Type);
                stmt_ty = MakeGeneric(typeof(ExprStmt<,,>), [source_ty, val_ty, end]);
            }

            int c = GetCost(stmt_ty);
            if (c + current_cost > MAX_COST) {
                bundles.Add(current_bundle);
                current_bundle = new List<Type>();
                current_cost = 0;
            }
            current_bundle.Add(stmt_ty);
            current_cost += c;
        }
        // add last bundle
        if (current_bundle.Count > 0 || bundles.Count == 0) {
            bundles.Add(current_bundle);
        }

        bundles.Reverse();
        Type next = null;
        int tier = 0;
        foreach (var bundle_types in bundles) {
            if (next != null) {
                bundle_types.Add(MakeGeneric(typeof(Anchor<>),[next]));
            }
            next = MakeBundle(bundle_types, ref tier);
        }

        return next;

        /*if (stmts.Count > 50) {
            int half = stmts.Count / 2;
            var first = new List<(Destination, Expression)>();
            var second = new List<(Destination, Expression)>();
            foreach (var stmt in stmts) {
                if (first.Count < half) {
                    first.Add(stmt);
                } else {
                    second.Add(stmt);
                }
            }
            Console.WriteLine("split "+first.Count+" "+second.Count);
            var a = CompileStatements(first);
            var b = CompileStatements(second);
            return MakeGeneric(typeof(PairGlue<,>),[a,b]);
        }*/

        /*Type block_ty = typeof(End);

        foreach (var stmt in stmts.AsEnumerable().Reverse()) {
            (var dest,var source) = stmt;

            if (source is DebugExpression) {
                continue;
            }

            var source_ty = source?.BuildMirror();
            if (dest != null) {
                block_ty = dest.BuildDestination(source_ty, block_ty);
            } else {
                var val_ty = ConvertValType(source.Type);
                block_ty = MakeGeneric(typeof(ExprStmt<,,>), [source_ty, val_ty, block_ty]);
            }
        }
        return block_ty;*/
    }

    private static Type MakeBundle(List<Type> stmt_types, ref int tier) {
        var end = typeof(End);
        while (stmt_types.Count > 1) {
            var next_types = new List<Type>();
            int index = 0;
            Type bundle_ty = (tier % 5) switch {
                0 => typeof(Stmts1<,,,>),
                1 => typeof(Stmts2<,,,>),
                2 => typeof(Stmts3<,,,>),
                3 => typeof(Stmts4<,,,>),
                4 => typeof(Stmts5<,,,>),
                _ => null
            };
            tier++;

            Type[] bundle_args = new Type[4];
            while (index < stmt_types.Count) {
                for (int i=0;i<4;i++) {
                    if (index < stmt_types.Count) {
                        bundle_args[i] = stmt_types[index];
                        index++;
                    } else {
                        bundle_args[i] = end;
                    }
                }
                //Console.WriteLine("? "+bundle_ty+" "+bundle_args);
                next_types.Add(MakeGeneric(bundle_ty,bundle_args));
            }

            //Console.WriteLine("reduced "+stmt_types.Count+" -> "+next_types.Count+" "+bundle_ty);
            stmt_types = next_types;
        }
        // result tier should match our last tier exactly
        // this should forcibly disable inlining
        if (tier > 0) {
            tier--;
        }

        if (stmt_types.Count == 0) {
            return end;
        }

        return stmt_types[0];
    }

    private static int GetCost(Type base_ty) {
        Stack<Type> types = new Stack<Type>();
        types.Push(base_ty);

        int sum = 0;
        while (types.Count > 0) {
            var ty = types.Pop();
            sum += 3; // TODO
            foreach (var arg in ty.GetGenericArguments()) {
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
    public object[] Children = new object[4];
}
