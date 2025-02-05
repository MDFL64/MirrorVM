using System.Diagnostics;
using System.Text.Json;

if (false) {
    //string module_name = "farter/target/wasm32-unknown-unknown/release/farter.wasm";
    //string func_name = "test3";

    string module_name = "tests/i32.0.wasm";
    string func_name = "popcnt";

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

//var cmds = JsonSerializer.Deserialize<TestCommands>(File.ReadAllText("tests/i32.json"));
//cmds.Run();
var cmds = JsonSerializer.Deserialize<TestCommands>(File.ReadAllText("tests/i64.json"));
cmds.Run();
