class HellBuilder {
    public static ICallable Compile(Block initial_block, int arg_count, int ret_count) {
        HashSet<Block> Closed = new HashSet<Block>();
        Queue<Block> Open = new Queue<Block>();
        List<Block> Blocks = new List<Block>();
        Open.Enqueue(initial_block);
        Closed.Add(initial_block);

        while (Open.Count > 0) {
            var block = Open.Dequeue();
            if (block.Index != -1) {
                throw new Exception("attempted to number block twice");
            }
            block.Index = Blocks.Count;
            Blocks.Add(block);

            foreach (var next in block.Terminator.GetNextBlocks()) {
                // enqueue block
                if (!Closed.Contains(next)) {
                    Open.Enqueue(next);
                    Closed.Add(next);
                }
            }
        }

        List<Type> CompiledBlocks = new List<Type>();
        foreach (var block in Blocks) {
            //Console.WriteLine(block.Name+" "+block.Index);
            Type block_ty = typeof(End);

            for (int i=block.Statements.Count-1;i>=0;i--) {
                (var dest,var source) = block.Statements[i];

                var source_ty = source?.BuildHell();
                if (dest != null) {
                    block_ty = dest.BuildDestination(source_ty, block_ty);
                } else {
                    var val_ty = ConvertValType(source.Type);
                    block_ty = MakeGeneric(typeof(ExprStmt<,,>), [source_ty, val_ty, block_ty]);
                }
                //Console.WriteLine("> stmt "+DebugType(block_ty));
            }
            
            var final_ty = block.Terminator.BuildHell(block_ty);
            //Console.WriteLine("> term "+DebugType(final_ty));
            CompiledBlocks.Add(final_ty);
        }
        while (CompiledBlocks.Count < 10) {
            CompiledBlocks.Add(typeof(TermVoid));
        }
        // arg setup
        {
            var ty = arg_count switch {
                0 => typeof(ArgRead0),
                1 => typeof(ArgRead1),
                2 => typeof(ArgRead2),
                3 => typeof(ArgRead3),
                4 => typeof(ArgRead4),
                5 => typeof(ArgRead5),
                _ => throw new Exception("too many arguments "+arg_count)
            };
            CompiledBlocks.Add(ty);
        }
        // result setup
        {
            int extra_rets = ret_count > 1 ? ret_count - 1 : 0;
            CompiledBlocks.Add(MakeConstant(extra_rets));
        }

        var body = MakeGeneric(typeof(Body<,,,,,,,,,,,>),CompiledBlocks.ToArray());
        //Console.WriteLine("~> "+DebugType(body));
        return (ICallable)Activator.CreateInstance(body);
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
