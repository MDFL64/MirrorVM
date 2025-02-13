
public class WasmInstance {
    public byte[] Memory;
    public ICallable[] Functions;

    public WasmInstance(WasmModule module) {
        Memory = module.GetInitialMemory();
        Functions = new ICallable[module.Functions.Count];
        for (int i=0;i<Functions.Length;i++) {
            Functions[i] = new JitStub(module.Functions[i], i);
        }
    }
}

class JitStub : ICallable {
    WasmFunction Function;
    int Index;

    public JitStub(WasmFunction function, int index) {
        Function = function;
        Index = index;
    }

    public long Call(Span<long> args, WasmInstance inst)
    {
        Console.WriteLine("GET "+Index);
        var compiled = Function.GetBody().Compile();
        compiled.Call(args, inst);
        inst.Functions[Index] = compiled;

        return compiled.Call(args, inst);
    }
}
