class TestBarriers {
    public static void Run(string name) {
        Console.WriteLine("??? "+name);
        var module = new WasmModule(new MemoryStream(File.ReadAllBytes("barrier_tests/"+name+".wasm")),null);
        var instance = new WasmInstance(module);
        var func = (WasmFunction)module.Exports["addTwo"];
        var callable = func.GetBody().Compile();

        for (int i=0;i<10;i++) {
            long res = callable.Call([i,i],instance);
            Console.WriteLine(res);
        }
    }
}
