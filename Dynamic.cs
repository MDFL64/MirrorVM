public class Output {
    public string Text = "";
    public string Info = "";
    public bool Kill = false;

    public void WriteChar(char c) {
        #if SANDBOX
        Text += c;
        #else
        Console.Write(c);
        #endif
    }

    public void WriteInfo(string s) {
        Info += s;
        Info += '\n';
    }

    public void CheckKill() {
        if (Kill) {
            throw new OperationCanceledException();
        }
    }
}

public class DynamicFlood {
    private Op Root;

    public DynamicFlood(string code) {
        var bytecode = FastInterpreter.Compile(code);
        int index = bytecode.Count-1;

        var op_ty = BuildInner(bytecode,ref index);

        #if SANDBOX
        var op = TypeLibrary.Create<Op>(op_ty);
        if (op == null) {
            throw new Exception("failed to create op");
        }
        #else
        var new_op = Activator.CreateInstance(op_ty);
        if (new_op == null) {
            throw new Exception("failed to create op");
        }
        var op = (Op)new_op;
        #endif

        Root = op;
    }

    public void Run(Output output) {
        int ptr = 1000;
        var data = new byte[1_000_000];
        Root.Run(ptr,data,output);
    }

    private static Type MakeGeneric(Type base_ty, Type[] args) {
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

    private static Type GetDigit(int n) {
        switch (n) {
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

    private static Type GenerateConst(int n) {
        if (n < 0) {
            return MakeGeneric(typeof(Neg<>),[GenerateConst(-n)]);
        }
        if (n < 16) {
            return GetDigit(n);
        } else if (n < 256) {
            return MakeGeneric(typeof(Num<,>),[GetDigit(n>>4),GetDigit(n&0xF)]);
        } else {
            throw new Exception("const too large "+n);
        }
    }

    private static Type BuildInner(List<Instr> bytecode, ref int index) {
        Type result = typeof(Stop);

        while (index >= 0) {
            var c = bytecode[index];
            switch (c.Op) {
                case OpCode.UpdateCell:
					result = MakeGeneric(typeof(UpdateCell<,,>),[GenerateConst(c.Offset),GenerateConst(c.Inc),result]);
                    break;
                case OpCode.UpdatePointer:
                    result = MakeGeneric(typeof(UpdatePointer<,>),[GenerateConst(c.Offset), result]);
                    break;
                case OpCode.Zero:
                    result = MakeGeneric(typeof(ZeroCell<,>),[GenerateConst(c.Offset), result]);
                    break;
                case OpCode.Output:
                    result = MakeGeneric(typeof(OutputCell<,>),[GenerateConst(c.Offset), result]);
                    break;
                case OpCode.LoopEnd:
                    index--;
                    var body = BuildInner(bytecode,ref index);
                    if (bytecode[index].Op != OpCode.LoopStart) {
                        throw new Exception("BAD LOOP");
                    }
                    result = MakeGeneric(typeof(Loop<,>),[body, result]);
                    break;
                case OpCode.LoopStart:
                    return result;
                default:
                    //Console.WriteLine("--- "+result);
                    throw new Exception("todo "+c.Op);
            }
            index--;
        }

        return result;
    }
}
