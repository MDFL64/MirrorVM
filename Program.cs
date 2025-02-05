using System.Diagnostics;
using System.Text.Json;

if (false) {
    //string module_name = "farter/target/wasm32-unknown-unknown/release/farter.wasm";
    //string func_name = "test3";

    string module_name = "tests/f32.0.wasm";
    string func_name = "min";

    var module = new WasmModule(new MemoryStream(File.ReadAllBytes(module_name)));
    if (module.Exports.TryGetValue(func_name, out object item)) {
        var func = item as WasmFunction;
        if (func != null) {
            var callable = func.GetBody().Compile();
            var reg = new Registers();
            reg.R0 = 123;
            reg.R1 = 456;
            var start = Stopwatch.StartNew();
            var res = callable.Run(reg);
            Console.WriteLine("> "+res);
            Console.WriteLine("> "+start.Elapsed);
            Environment.Exit(0);
        }
    }
    throw new Exception("failed to find function");
}

// i32 i64
TestCommands.RunFile("i32");
TestCommands.RunFile("i64");
TestCommands.RunFile("f32");


//TestCommands.RunFile("int_exprs");

Console.WriteLine();

//var cmds = JsonSerializer.Deserialize<TestCommands>(File.ReadAllText("tests/int_exprs.json"));
//cmds.Run();
