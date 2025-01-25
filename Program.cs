using System.ComponentModel.Design.Serialization;
using System.Diagnostics;

// BrainFlood: Safe runtime .NET codegen by abusing Generics, Reflection, and the JIT

// simple interpreter               15.665
// optimized interpreter            9.728
// static hell                      2.172
// dynamic hell                     2.095
// generated c-sharp                1.541
// nayuki compiler + clang -O3      0.498

var code = File.ReadAllText("mandelbrot.txt");

var t0 = Stopwatch.StartNew();
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
