using System.Diagnostics;

// simple interpreter               15.665
// generated c-sharp                3.209
// nayuki compiler + clang -O3      0.498

var code = File.ReadAllText("mandelbrot.txt");

if (false) {
    new CodeGen().Generate(code);

    throw new Exception("done");
}

var timer = Stopwatch.StartNew();
//Interpreter.Run(code);
Generated.Run();
Console.WriteLine();
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

class CodeGen {
    int Tabs = 0;
    StreamWriter WriteStream;

    public void WriteLine(string line) {
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
                    WriteLine("data["+PtrOffset(offset)+"] += "+count+";");
                    break;
                case '-':
                    while (i+1 < code.Length && code[i+1] == '-') {
                        i++;
                        count++;
                    }
                    WriteLine("data["+PtrOffset(offset)+"] -= "+count+";");
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
                    if (offset != 0) {
                        WriteLine("ptr += "+offset+";");
                        offset = 0;
                    }
                    WriteLine("while (data[ptr] != 0) {");
                    Tabs++;
                    break;
                case ']':
                    if (offset != 0) {
                        WriteLine("ptr += "+offset+";");
                        offset = 0;
                    }
                    Tabs--;
                    WriteLine("}");
                    break;
                case '.':
                    WriteLine("Console.Write((char)data["+PtrOffset(offset)+"]);");
                    break;
                case ',':
                    throw new Exception("input not supported");
            }
        }

        Tabs = 1;
        WriteLine("}\n}");
        WriteStream.Close();
    }
} 
