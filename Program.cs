using System.ComponentModel.Design.Serialization;
using System.Diagnostics;

// BrainFlood: Safe runtime .NET codegen by abusing Generics, Reflection, and the JIT

// simple interpreter               15.665
// optimized interpreter            9.728
// static hell                      2.172
// dynamic hell                     2.095
// generated c-sharp                1.541
// nayuki compiler + clang -O3      0.498

//var code = File.ReadAllBytes("farter/target/wasm32-unknown-unknown/release/farter.wasm");
var code = File.ReadAllBytes("pooper/test.wasm");

var module = new WasmModule(new MemoryStream(code));

if (module.Exports.TryGetValue("test_i32_compare", out object item)) {
    var func = item as WasmFunction;
    if (func != null) {
        var body = func.GetBody();
        Console.WriteLine(">>> "+body);
    }
} else {
    throw new Exception("function not found");
}

/*var t0 = Stopwatch.StartNew();
var hell = new DynamicFlood(code);
Console.WriteLine("compile: "+t0.Elapsed);

Console.WriteLine();
var timer = Stopwatch.StartNew();
//InterpreterOpt.Run(code);
//Generated.Run();
//GeneratedStatic.Run();
var output = new Output();

hell.Run(output);
Console.WriteLine(timer.Elapsed);
Console.WriteLine();
*/
