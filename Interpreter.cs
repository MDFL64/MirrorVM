using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;

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

class FastInterpreter {
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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Run(string code, Output output) {
        var sw = Stopwatch.StartNew();
        var bytecode = Compile(code);
        output.WriteInfo("Compiled to bytecode in "+sw.Elapsed);

        sw = Stopwatch.StartNew();
        int pc = 0;
        int ptr = 1000;
        var data = new byte[1_000_000];

        while (pc<bytecode.Count) {
            var c = bytecode[pc];
            output.CheckKill();
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
                    output.Text += (char)data[ptr + c.Offset];
                    break;
                case OpCode.Zero:
                    data[ptr + c.Offset] = 0;
                    break;
                default:
                    throw new Exception("todo "+c.Op);
            }
            pc++;
        }
        output.WriteInfo("Executed in "+sw.Elapsed);
    }
}
