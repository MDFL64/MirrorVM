/*
public class Output {
    public string Text = "";
    public string Info = "";
    public bool Kill = false;

    public void WriteChar(char c) {
        /*#if SANDBOX
        Text += c;
        #else
        Console.Write(c);
        #endif* /
        Text += c;
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

    private static Type BuildInner(List<Instr> bytecode, ref int index) {
        Type result = typeof(Stop);

        while (index >= 0) {
            var c = bytecode[index];
            switch (c.Op) {
                case OpCode.UpdateCell:
					result = MakeGeneric(typeof(UpdateCell<,,>),[ConstBuilder.BuildInt(c.Offset),ConstBuilder.BuildInt(c.Inc),result]);
                    break;
                case OpCode.UpdatePointer:
                    result = MakeGeneric(typeof(UpdatePointer<,>),[ConstBuilder.BuildInt(c.Offset), result]);
                    break;
                case OpCode.Zero:
                    result = MakeGeneric(typeof(ZeroCell<,>),[ConstBuilder.BuildInt(c.Offset), result]);
                    break;
                case OpCode.Output:
                    result = MakeGeneric(typeof(OutputCell<,>),[ConstBuilder.BuildInt(c.Offset), result]);
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
*/
