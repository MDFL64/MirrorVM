public struct DynCallable {
    public int SigId;
    public ICallable Callable;
}

public class WasmInstance {
    public byte[] Memory;
    public ICallable[] Functions;
    public DynCallable[] DynamicCallTable;

    public WasmInstance(WasmModule module) {
        Memory = module.GetInitialMemory();
        Functions = new ICallable[module.Functions.Count];
        for (int i=0;i<Functions.Length;i++) {
            Functions[i] = new JitStub(module.Functions[i], i);
        }
        var source_table = module.Tables[0];
        DynamicCallTable = new DynCallable[source_table.GetLength()];
        for (int i=0;i<DynamicCallTable.Length;i++) {
            int index = (int)source_table.Get(i);
            DynamicCallTable[i] = new DynCallable{
                Callable = Functions[index],
                SigId = module.FindSigId(module.Functions[index].Sig)
            };
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
        // write the compiled function into our table
        var compiled = Function.GetBody().Compile();
        inst.Functions[Index] = compiled;

        return compiled.Call(args, inst);
    }
}
