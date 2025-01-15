using System.Diagnostics;

// simple interpreter               15.665
// optimized interpreter            9.728
// generated hell                   2.172
// generated c-sharp                1.541
// nayuki compiler + clang -O3      0.498

var code = File.ReadAllText("mandelbrot.txt");

if (false) {
    new CodeGen().GenerateStatic(code);

    throw new Exception("done");
}

if (false) {
    //var n = default(Num<D8,D8>).Run();
    //Console.WriteLine(">>> "+n);
    Test.Run();
    throw new Exception("done");
}

Console.WriteLine();
var timer = Stopwatch.StartNew();
//InterpreterOpt.Run(code);
//Generated.Run();
GeneratedStatic.Run();
Console.WriteLine(timer.Elapsed);
Console.WriteLine();

class Interpreter {
    public static void Run(string code) {
        int pc = 0;
        int ptr = 1000;
        var data = new byte[1_000_000];
        int[] bracket_map = Enumerable.Repeat(-1, code.Length).ToArray();

        {
            var brackets = new Stack<int>();
            for (int i=0;i<code.Length;i++) {
                var c = code[i];
                if (c=='[') {
                    brackets.Push(i);
                } else if (c==']') {
                    int start = brackets.Pop();
                    bracket_map[start] = i;
                    bracket_map[i] = start;
                }
            }
        }

        while (pc<code.Length) {
            var c = code[pc];
            switch (c) {
                case '+':
                    data[ptr]++;
                    break;
                case '-':
                    data[ptr]--;
                    break;
                case '>':
                    ptr++;
                    break;
                case '<':
                    ptr--;
                    break;
                case '[':
                    if (data[ptr] == 0) {
                        pc = bracket_map[pc];
                        continue;
                    }
                    break;
                case ']':
                    if (data[ptr] != 0) {
                        pc = bracket_map[pc];
                        continue;
                    }
                    break;
                case '.':
                    Console.Write((char)data[ptr]);
                    break;
                case ',':
                    throw new Exception("input not supported");
            }
            pc++;
        }
    }
}

struct Instr {
    public OpCode Op;
    public byte Inc;
    public int Offset;
}

enum OpCode : byte {
    UpdateCell,
    UpdatePointer,
    LoopStart,
    LoopEnd,
    Output,
    Zero
}

class InterpreterOpt {
    public static List<Instr> Compile(string code) {
        var bytecode = new List<Instr>();
        var brackets = new Stack<int>();
        int offset = 0;

        for (int i=0;i<code.Length;i++) {
            var c = code[i];
            int count = 1;
            switch (c) {
                case '+':
                    while (i+1 < code.Length && code[i+1] == '+') {
                        i++;
                        count++;
                    }
                    bytecode.Add(new Instr{
                        Op = OpCode.UpdateCell,
                        Offset = offset,
                        Inc = (byte)count,
                    });
                    break;
                case '-':
                    while (i+1 < code.Length && code[i+1] == '-') {
                        i++;
                        count++;
                    }
                    bytecode.Add(new Instr{
                        Op = OpCode.UpdateCell,
                        Offset = offset,
                        Inc = (byte)-count,
                    });
                    break;
                case '>':
                    while (i+1 < code.Length && code[i+1] == '>') {
                        i++;
                        count++;
                    }
                    offset += count;
                    break;
                case '<':
                    while (i+1 < code.Length && code[i+1] == '<') {
                        i++;
                        count++;
                    }
                    offset -= count;
                    break;
                case '[':
                    if (i+2 < code.Length && code[i+1] == '-' && code[i+2] == ']') {
                        bytecode.Add(new Instr{
                            Op = OpCode.Zero,
                            Offset = offset
                        });
                        i+=2;
                        continue;
                    }

                    if (offset != 0) {
                        bytecode.Add(new Instr{
                            Op = OpCode.UpdatePointer,
                            Offset = offset
                        });
                        offset = 0;
                    }
                    brackets.Push(bytecode.Count);
                    bytecode.Add(new Instr{
                        Op = OpCode.LoopStart,
                        Offset = -1
                    });
                    break;
                case ']':
                    if (offset != 0) {
                        bytecode.Add(new Instr{
                            Op = OpCode.UpdatePointer,
                            Offset = offset
                        });
                        offset = 0;
                    }
                    int start_pc = brackets.Pop();
                    if (bytecode[start_pc].Op != OpCode.LoopStart) {
                        throw new Exception("bad loop");
                    }
                    var start_bc = bytecode[start_pc];
                    start_bc.Offset = bytecode.Count;
                    bytecode[start_pc] = start_bc;

                    bytecode.Add(new Instr{
                        Op = OpCode.LoopEnd,
                        Offset = start_pc
                    });
                    break;
                case '.':
                    bytecode.Add(new Instr{
                        Op = OpCode.Output,
                        Offset = offset
                    });
                    break;
                case ',':
                    throw new Exception("input not supported");
            }
        }

        return bytecode;
    }

    public static void Run(string code) {
        var bytecode = Compile(code);

        int pc = 0;
        int ptr = 1000;
        var data = new byte[1_000_000];

        while (pc<bytecode.Count) {
            var c = bytecode[pc];
            //Console.WriteLine("? "+c.Op+" "+c.Inc+" "+c.Offset);
            switch (c.Op) {
                case OpCode.UpdateCell:
                    //Console.WriteLine("pre "+data[ptr - c.Offset]);
                    data[ptr + c.Offset] += c.Inc;
                    //Console.WriteLine("post "+data[ptr + c.Offset]);
                    break;
                case OpCode.UpdatePointer:
                    ptr += c.Offset;
                    break;
                case OpCode.LoopStart:
                    //ptr += (sbyte)c.Inc;
                    if (data[ptr] == 0) {
                        pc = c.Offset;
                        continue;
                    }
                    break;
                case OpCode.LoopEnd:
                    //ptr += (sbyte)c.Inc;
                    if (data[ptr] != 0) {
                        pc = c.Offset;
                        continue;
                    }
                    break;
                case OpCode.Output:
                    Console.Write((char)data[ptr + c.Offset]);
                    break;
                case OpCode.Zero:
                    data[ptr + c.Offset] = 0;
                    break;
                default:
                    throw new Exception("todo "+c.Op);
            }
            pc++;
        }
    }
}

class CodeGen {
    int Tabs = 0;
    StreamWriter? WriteStream;

    public void WriteLine(string line) {
        if (WriteStream == null) {
            throw new Exception();
        }
        for (int i=0;i<Tabs;i++) {
            WriteStream.Write("    ");
        }
        WriteStream.WriteLine(line);
    }

    private static string PtrOffset(int offset) {
        if (offset == 0) {
            return "ptr";
        } else if (offset > 0) {
            return "ptr+"+offset;
        } else {
            return "ptr-"+(-offset);
        }
    }

    public void GenerateStatic(string code) {
        WriteStream = new StreamWriter("GeneratedStatic.cs");

        WriteLine("public class GeneratedStatic {");
        Tabs = 1;
        WriteLine("public static void Run() {");
        Tabs = 2;
        WriteLine("int ptr = 1000;");
        WriteLine("var data = new byte[1_000_000];");
        WriteLine("");

        var bytecode = InterpreterOpt.Compile(code);
        int index = bytecode.Count-1;

        var parts = GenerateStaticInner(bytecode,ref index).Split(",");
        string current_line = "var machine = default(";
        for (int i=0;i<parts.Length;i++) {
            current_line += parts[i];
            if (i != parts.Length-1) {
                current_line += ",";
            }
            if (current_line.Length > 100) {
                WriteLine(current_line);
                current_line = "";
            }
        }
        WriteLine(current_line+");");
        WriteLine("");
        WriteLine("machine.Run(ptr,data);");

        Tabs = 1;
        WriteLine("}\n}");
        WriteStream.Close();
    }

    private string GenerateConst(int n) {
        if (n < 0) {
            return "Neg<"+GenerateConst(-n)+">";
        }
        string ns = n.ToString("X");
        if (ns.Length == 1) {
            return "D"+ns;
        } else if (ns.Length == 2) {
            return "Num<D"+ns[0]+",D"+ns[1]+">";
        } else {
            throw new Exception("const too large "+ns);
        }
    }

    private string GenerateStaticInner(List<Instr> bytecode, ref int index) {
        string source = "Stop";

        while (index >= 0) {
            var c = bytecode[index];
            switch (c.Op) {
                case OpCode.UpdateCell:
                    source = "UpdateCell<"+GenerateConst(c.Offset)+","+GenerateConst(c.Inc)+","+source+">";
                    break;
                case OpCode.UpdatePointer:
                    source = "UpdatePointer<"+GenerateConst(c.Offset)+","+source+">";
                    break;
                case OpCode.Output:
                    source = "OutputCell<"+GenerateConst(c.Offset)+","+source+">";
                    break;
                case OpCode.Zero:
                    source = "ZeroCell<"+GenerateConst(c.Offset)+","+source+">";
                    break;
                case OpCode.LoopEnd:
                    index--;
                    string body = GenerateStaticInner(bytecode,ref index);
                    if (bytecode[index].Op != OpCode.LoopStart) {
                        throw new Exception("BAD LOOP");
                    }
                    source = "Loop<"+body+","+source+">";
                    break;
                case OpCode.LoopStart:
                    return source;
                default:
                    Console.WriteLine("--- "+source);
                    throw new Exception("todo "+c.Op);
            }
            index--;
        }

        return source;
    }

    public void Generate(string code) {
        WriteStream = new StreamWriter("Generated.cs");

        WriteLine("public class Generated {");
        Tabs = 1;
        WriteLine("public static void Run() {");
        Tabs = 2;
        WriteLine("int ptr = 1000;");
        WriteLine("var data = new byte[1_000_000];");
        WriteLine("");
        WriteLine("");

        var bytecode = InterpreterOpt.Compile(code);

        for (int i=0;i<bytecode.Count;i++) {
            var bc = bytecode[i];
            switch (bc.Op) {
                case OpCode.UpdateCell:
                    WriteLine("data["+PtrOffset(bc.Offset)+"] += "+bc.Inc+";");
                    break;
                case OpCode.Zero:
                    WriteLine("data["+PtrOffset(bc.Offset)+"] = 0;");
                    break;
                case OpCode.UpdatePointer:
                    WriteLine("ptr += "+bc.Offset+";");
                    break;
                case OpCode.LoopStart:
                    WriteLine("while (data[ptr] != 0) {");
                    Tabs++;
                    break;
                case OpCode.LoopEnd:
                    Tabs--;
                    WriteLine("}");
                    break;
                case OpCode.Output:
                    WriteLine("Console.Write((char)data["+PtrOffset(bc.Offset)+"]);");
                    break;
            }
        }

        Tabs = 1;
        WriteLine("}\n}");
        WriteStream.Close();
    }
}
