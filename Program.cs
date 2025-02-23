using System.Diagnostics;
using System.Text.Json;

if (false) {
    string module_name = "farter/target/wasm32-unknown-unknown/release/farter.wasm";
    string func_name = "test_memory";

    //string module_name = "tests/address.0.wasm";
    //string func_name = "16u_good1";

    var module = new WasmModule(new MemoryStream(File.ReadAllBytes(module_name)),null);
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

//return;

TestCommands.RunFile("address");
TestCommands.RunFile("align");
//TestCommands.RunFile("binary");           no exec tests
//TestCommands.RunFile("binary-leb128");    no exec tests
TestCommands.RunFile("block");
TestCommands.RunFile("br");
TestCommands.RunFile("br_if");
TestCommands.RunFile("br_table",[1067,1068,1069,1070,1071,1072,1073,1074]); // skip br_table with thousands of options
// todo bulk
TestCommands.RunFile("call");
TestCommands.RunFile("call_indirect");
//TestCommands.RunFile("comments");         no exec tests
TestCommands.RunFile("const");
TestCommands.RunFile("conversions");
//TestCommands.RunFile("custom");           no exec tests
//TestCommands.RunFile("data");             no exec tests
//TestCommands.RunFile("elem");             a bunch of junk I don't want to support, including multi-module tests
TestCommands.RunFile("endianness");
//TestCommands.RunFile("exports");          didn't bother
TestCommands.RunFile("f32");
TestCommands.RunFile("f32_bitwise");
TestCommands.RunFile("f32_cmp");
TestCommands.RunFile("f64");
TestCommands.RunFile("f64_bitwise");
TestCommands.RunFile("f64_cmp");
TestCommands.RunFile("fac");
TestCommands.RunFile("float_exprs",[2403,2405,2407,2409,2411,2413]); // canonicalization
TestCommands.RunFile("float_literals");
TestCommands.RunFile("float_memory",[21,22,71,73,74]); // canonicalization crap, possibly worth looking into?
TestCommands.RunFile("float_misc");
TestCommands.RunFile("forward");
TestCommands.RunFile("func");
TestCommands.RunFile("func_ptrs");
TestCommands.RunFile("global");
TestCommands.RunFile("i32");
TestCommands.RunFile("i64");
TestCommands.RunFile("if");
//TestCommands.RunFile("imports");          didn't bother
//TestCommands.RunFile("inline-module");    no exec tests
TestCommands.RunFile("int_exprs");
TestCommands.RunFile("int_literals");
TestCommands.RunFile("labels");
TestCommands.RunFile("left-to-right");
//TestCommands.RunFile("linking");          didn't bother
TestCommands.RunFile("load");
TestCommands.RunFile("local_get");
TestCommands.RunFile("local_set");
TestCommands.RunFile("local_tee");
TestCommands.RunFile("loop");
TestCommands.RunFile("memory");
//TestCommands.RunFile("memory_copy");      todo bulk memory
//TestCommands.RunFile("memory_grow");      multi-module tests
//TestCommands.RunFile("memory_init");      fairly certain something is wrong with data section handling
TestCommands.RunFile("memory_redundancy");
TestCommands.RunFile("memory_size");
TestCommands.RunFile("memory_trap");
//TestCommands.RunFile("names");            mostly just breaks my debug output
TestCommands.RunFile("nop");
//TestCommands.RunFile("obsolete-keywords");    no exec tests
//TestCommands.RunFile("ref_func");         didn't bother
//TestCommands.RunFile("ref_is_null");      didn't bother
//TestCommands.RunFile("ref_null");         didn't bother
TestCommands.RunFile("return");
//TestCommands.RunFile("select");           don't want to deal with annotated select
// skip simd
//TestCommands.RunFile("skip-stack-guard-page");    no exec tests / there's no way this is relevant
TestCommands.RunFile("stack");
//TestCommands.RunFile("start");            don't bother
TestCommands.RunFile("store");
TestCommands.RunFile("switch");
// skip table

Console.WriteLine();

//var cmds = JsonSerializer.Deserialize<TestCommands>(File.ReadAllText("tests/int_exprs.json"));
//cmds.Run();
