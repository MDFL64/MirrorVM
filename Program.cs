using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;

using Wacs.Core;
using Wacs.Core.Runtime;

string module_name = "X:/MirrorVM/rust_bench/target/wasm32-unknown-unknown/release/rust_bench.wasm";
string[] benchmarks = ["hashes", "image", "json", "prospero_compile", "prospero_eval", "rand_sort", "rapier", /*"regex",*/ "zip"];
var module = new WasmModule(new MemoryStream(File.ReadAllBytes(module_name)), null);
var instance = new WasmInstance(module);

// MirrorVM benchmark
if (true)
{
    List<string> result_table = [];

    foreach (var name in benchmarks)
    {
        string func_name = "bench_" + name;
        Console.WriteLine("function " + func_name);

        if (module.Exports.TryGetValue(func_name, out object item))
        {
            var func = item as WasmFunction;
            if (func != null)
            {
                var callable = func.GetBody().Compile();
                //long[] func_args = [100_000_000];

                List<TimeSpan> times = [];

                for (int i = 0; i < 10; i++)
                {
                    var start = Stopwatch.StartNew();
                    var res = callable.Call([], instance);
                    times.Add(start.Elapsed);
                    Console.WriteLine("> " + res);
                }
                times.Sort();
                Console.WriteLine("min = " + times[0]);
                Console.WriteLine("max = " + times[times.Count - 1]);

                result_table.Add(name + "," + times[0].TotalSeconds);
            }
        }
        else
        {
            throw new Exception("failed to find function: " + func_name);
        }
    }

    Console.WriteLine("count reg = " + IRBuilder.TOTAL_REG);
    Console.WriteLine("count frame = " + IRBuilder.TOTAL_FRAME);
    for (int i = 0; i < IRBuilder.FRAME_INDICES.Length; i++)
    {
        int n = IRBuilder.FRAME_INDICES[i];
        if (n > 0)
        {
            Console.WriteLine("count frame[" + i + "] = " + n);
        }
    }

    Console.WriteLine("==================");
    foreach (var line in result_table)
    {
        Console.WriteLine(line);
    }
    Console.WriteLine("==================");
}

// WACS benchmark
if (false)
{
    var runtime = new WasmRuntime();
    runtime.TranspileModules = true;

    var stream = new FileStream(module_name, FileMode.Open);
    var wacs_module = BinaryModuleParser.ParseWasm(stream);

    var modInst = runtime.InstantiateModule(wacs_module);
    runtime.RegisterModule("bench", modInst);

    foreach (var name in benchmarks)
    {
        if (runtime.TryGetExportedFunction(("bench", "bench_" + name), out var func_addr))
        {
            var func_invoker = runtime.CreateInvokerFunc<Value>(func_addr);
            //Console.WriteLine("calling...");

            var start = Stopwatch.StartNew();
            int n = func_invoker();
            var elapsed = start.Elapsed.TotalSeconds;

            Console.WriteLine(name + "," + elapsed);
        }
    }
}

// prospero eval
if (false)
{
    int SIZE = 256;
    Bitmap image = new Bitmap(SIZE, SIZE);
    //Graphics gfx = Graphics.FromImage(image);

    {
        if (module.Exports.TryGetValue("bench_rapier", out object item))
        {
            var func = item as WasmFunction;
            if (func != null)
            {
                var callable = func.GetBody().Compile();
                callable.Call([], instance);
            }
        }
    }

    {
        if (module.Exports.TryGetValue("physics_test", out object item))
        {
            var func = item as WasmFunction;
            if (func != null)
            {
                var callable = func.GetBody().Compile();

                for (int y = 0; y < SIZE; y++)
                {
                    for (int x = 0; x < SIZE; x++)
                    {
                        float scale = 20.0f;
                        float x_f = (float)x / SIZE * 2 - 1;
                        float y_f = -(float)y / SIZE * 2 + 1;

                        var res = callable.Call([
                            BitConverter.SingleToInt32Bits(x_f * scale),
                            BitConverter.SingleToInt32Bits(y_f * scale),
                        ], instance);

                        if (res > 0)
                        {
                            image.SetPixel(x, y, Color.Red);
                        }

                        //Console.WriteLine("-> " + x_f + " " + y_f + " " + res_f);
                    }
                }
            }
        }
    }

    image.Save("physics.png", ImageFormat.Png);
}


//TestBarriers.Run("funky");
return;

TestBarriers.Run("local_set");
TestBarriers.Run("local_tee");
TestBarriers.Run("global");
TestBarriers.Run("memory");
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
