public struct DynCallable {
    public int SigId;
    public ICallable Callable;
}

public class WasmInstance {
    public byte[] Memory;
    public long[] Globals;
    public ICallable[] Functions;
    public DynCallable[][] DynamicCallTable;

    public WasmInstance(WasmModule module) {
        Memory = module.GetInitialMemory();
        Functions = new ICallable[module.Functions.Count];
        for (int i=0;i<Functions.Length;i++) {
            Functions[i] = new JitStub(module.Functions[i], i);
        }
        Globals = new long[module.Globals.Count];
        for (int i=0;i<Globals.Length;i++) {
            Globals[i] = module.Globals[i].Item2;
        }
        DynamicCallTable = new DynCallable[module.Tables.Count][];
        for (int table_i=0;table_i<module.Tables.Count;table_i++) {
            var source_table = module.Tables[table_i];
            DynamicCallTable[table_i] = new DynCallable[source_table.GetLength()];
            for (int i=0;i<source_table.GetLength();i++) {
                var entry = source_table.Get(i);
                if (entry != null) {
                    int index = (int)entry;
                    DynamicCallTable[table_i][i] = new DynCallable{
                        Callable = Functions[index],
                        SigId = module.FindSigId(module.Functions[index].Sig)
                    };
                }
            }
        }
    }

    public void GrowMemory(int page_count) {
        if (page_count < 0) {
            throw new Exception("attempt to grow memory by negative value");
        }
        var new_memory = new byte[Memory.Length + page_count * 65536];
        Memory.CopyTo(new_memory,0);
        Memory = new_memory;
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
