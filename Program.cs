using System.Diagnostics;
using System.Text.Json;

Console.WriteLine("HELLO");
for (int i=0;i<100;i++) {
    Foo.Bar();
}

if (false) {
    string module_name = "farter/target/wasm32-unknown-unknown/release/farter.wasm";
    string func_name = "test2";

    //string module_name = "tests/f32.0.wasm";
    //string func_name = "min";

    var module = new WasmModule(new MemoryStream(File.ReadAllBytes(module_name)));
    if (module.Exports.TryGetValue(func_name, out object item)) {
        var func = item as WasmFunction;
        if (func != null) {
            var callable = func.GetBody().Compile();
            var reg = new Registers();
            reg.R0 = 123;
            reg.R1 = 456;
            var instance = new WasmInstance();
            var start = Stopwatch.StartNew();
            var res = callable.Run(reg, instance);
            Console.WriteLine("> "+res);
            Console.WriteLine("> "+start.Elapsed);
            Environment.Exit(0);
        }
    }
    throw new Exception("failed to find function");
}

//TestCommands.RunFile("address");
TestCommands.RunFile("global");

return;
TestCommands.RunFile("i32");
TestCommands.RunFile("i64");
TestCommands.RunFile("int_literals");
TestCommands.RunFile("int_exprs");

TestCommands.RunFile("f32");
TestCommands.RunFile("f32_cmp");
TestCommands.RunFile("f64");
TestCommands.RunFile("f64_cmp");
TestCommands.RunFile("float_literals");
//TestCommands.RunFile("float_exprs");
TestCommands.RunFile("float_misc");

TestCommands.RunFile("conversions");

Console.WriteLine();

//var cmds = JsonSerializer.Deserialize<TestCommands>(File.ReadAllText("tests/int_exprs.json"));
//cmds.Run();
