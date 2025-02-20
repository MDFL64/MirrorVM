using System.Diagnostics;
using System.Text.Json;

if (false) {
    string module_name = "farter/target/wasm32-unknown-unknown/release/farter.wasm";
    string func_name = "test_memory";

    //string module_name = "tests/address.0.wasm";
    //string func_name = "16u_good1";

    var module = new WasmModule(new MemoryStream(File.ReadAllBytes(module_name)));
    if (module.Exports.TryGetValue(func_name, out object item)) {
        var func = item as WasmFunction;
        if (func != null) {
            var callable = func.GetBody().Compile();
            long[] func_args = [100_000_000];
            var instance = new WasmInstance(module);

            List<TimeSpan> times = [];

            for (int i=0;i<20;i++) {
                var start = Stopwatch.StartNew();
                var res = callable.Call(func_args, instance);
                times.Add(start.Elapsed);
                Console.WriteLine("> "+res);
            }
            times.Sort();
            Console.WriteLine("min = "+times[0]);
            Console.WriteLine("max = "+times[times.Count-1]);

            Environment.Exit(0);
        }
    }
    throw new Exception("failed to find function");
}


TestCommands.RunFile("address");            // good
TestCommands.RunFile("align");              // good
//TestCommands.RunFile("binary");           no exec tests
//TestCommands.RunFile("binary-leb128");    no exec tests
TestCommands.RunFile("block");
TestCommands.RunFile("br");
TestCommands.RunFile("br_if");
TestCommands.RunFile("br_table",[1067,1068,1069,1070,1071,1072,1073,1074]); // skip br_table with thousands of options
// todo bulk
TestCommands.RunFile("call");

return;

TestCommands.RunFile("memory");

//return;
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
