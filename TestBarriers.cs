class TestBarriers {
    public static void Run(string name) {
        Console.WriteLine("??? "+name);
        var module = new WasmModule(new MemoryStream(File.ReadAllBytes("barrier_tests/"+name+".wasm")),null);
        var instance = new WasmInstance(module);
        var func = (WasmFunction)module.Exports["addTwo"];
        var callable = func.GetBody().Compile();
        var frame = new Frame(1);

        for (int i = 0; i < 10; i++)
        {
            frame.SetArg(0, i);
            frame.SetArg(1, i);

            callable.Call(frame, instance);
            //frame.Dump();
            Console.WriteLine(frame.GetReturnInt());
        }
    }
}
