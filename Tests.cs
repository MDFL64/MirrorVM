using System.Text.Json;

class DummyCallable : ICallable
{
    public void Call(Span<long> args, WasmInstance inst)
    {
    }

    public void SetBody(object body)
    {
        throw new NotImplementedException();
    }
}

class TestImports : ImportProvider {
    public override ICallable ImportFunction(string module, string name, FunctionType sig)
    {
        if (module == "spectest" && name == "print_i32") {
            return new DummyCallable();
        }

        return base.ImportFunction(module, name, sig);
    }

    public override long ImportGlobal(string module, string name, ValType ty)
    {
        return (module,name,ty) switch {
            ("spectest","global_i32",ValType.I32) => 666,
            ("spectest","global_i64",ValType.I64) => 666,
            _ => base.ImportGlobal(module,name,ty)
        };
    }

    public override void ImportMemory(string module, string name, WasmMemory memory)
    {
        if (module == "spectest" && name == "memory") {
            Console.WriteLine("TODO MEMORY IMPORT");
        } else {
            base.ImportMemory(module, name, memory);
        }
    }

    public override void ImportTable(string module, string name, WasmTable table)
    {
        if (module == "spectest" && name == "table") {
            table.Reserve(100); // ???
        } else {
            base.ImportTable(module, name, table);
        }
    }
}

class TestCommands {
    public static void RunFile(string name, int[] skip_lines = null) {
        Console.WriteLine(">>> "+name);
        var cmds = JsonSerializer.Deserialize<TestCommands>(File.ReadAllText("tests/"+name+".json"));
        cmds.Run(skip_lines);
    }

    public TestCommand[] commands {get;set;}
    public WasmModule Module;
    public WasmInstance Instance;

    public void Run(int[] skip_lines) {
        var imports = new TestImports();

        int total = 0;
        int passed = 0;
        foreach (var cmd in commands) {
            //Console.WriteLine("? "+cmd.line);
            if (skip_lines != null && skip_lines.Contains(cmd.line)) {
                continue;
            }
            if (total - passed >= 10) {
                Console.WriteLine("--- TOO MANY FAILED TESTS");
                return;
            }
            switch (cmd.type) {
                case "module": {
                    //Console.WriteLine("module "+cmd.line);
                    var code = File.ReadAllBytes("tests/"+cmd.filename);
                    Module = new WasmModule(new MemoryStream(code), imports);
                    Instance = new WasmInstance(Module);
                    break;
                }
                case "action": {
                    total++;
                    var res = cmd.action.Run(Module, Instance, out _);
                    bool this_passed = res == ActionResult.Okay;
                    cmd.action.PrintStatus(this_passed,res.ToString(),cmd.line);
                    if (this_passed) {
                        passed++;
                    }
                    break;
                }
                case "assert_return": {
                    total++;
                    var res = cmd.action.Run(Module, Instance, out Frame returned);
                    if (res != ActionResult.Okay) {
                        cmd.action.PrintStatus(false,res.ToString(),cmd.line);
                    } else {
                        for (int i=0;i<cmd.expected.Length;i++) {
                            if (!cmd.expected[i].Check(returned.GetReturnLong(i))) {
                                cmd.action.PrintStatus(false,TestValue.StringifyResults(returned, cmd.expected, "!="),cmd.line);
                                goto end;
                            }
                        }

                        passed++;
                        cmd.action.PrintStatus(true,TestValue.StringifyResults(returned, cmd.expected, "=="),cmd.line);
                    }
                    end:
                    break;
                }
                case "assert_trap": {
                    total++;
                    var res = cmd.action.Run(Module, Instance, out Frame returned);
                    if (res != ActionResult.Trap) {
                        cmd.action.PrintStatus(false,"trap expected",cmd.line);
                    } else {
                        passed++;
                        cmd.action.PrintStatus(true,"trap",cmd.line);
                    }
                    break;
                }
                case "assert_invalid":
                case "assert_malformed":
                case "assert_exhaustion":
                case "assert_uninstantiable":
                    // ignored
                    break;
                default:
                    throw new Exception("todo command: "+cmd.type);
            }
        }
        if (passed == total) {
            Console.BackgroundColor = ConsoleColor.Green;
        } else {
            Console.BackgroundColor = ConsoleColor.Red;
        }
        Console.ForegroundColor = ConsoleColor.Black;
        Console.Write("[ "+passed+" / "+total+" TESTS PASSED ]");
        Console.ResetColor();
        Console.WriteLine();
    }
}

class TestCommand {
    public string type {get;set;}
    public string filename {get;set;}
    public int line {get;set;}
    public TestAction action {get;set;}
    public TestValue[] expected {get;set;}
}

class TestAction {
    public string type {get;set;}
    public string field {get;set;}
    public TestValue[] args {get;set;}

    public ActionResult Run(WasmModule module, WasmInstance instance, out Frame frame) {
        if (type != "invoke") {
            throw new Exception("unknown action: "+type);
        }

        if (module.Exports.TryGetValue(field, out object item)) {
            var func = item as WasmFunction;
            if (func != null) {
                frame = new Frame(func.Sig.Outputs.Count);
                ICallable callable;
                try {
                    callable = func.GetBody().Compile();
                } catch (Exception e) {
                    Console.WriteLine(e);
                    return ActionResult.CompileFailed;
                }

                for (int i=0;i<args.Length;i++) {
                    var val = args[i].Parse();
                    frame.SetArg(i, val);
                }

                try {
                    callable.Call(frame, instance);
                    return ActionResult.Okay;
                } catch (Exception) {
                    //Console.WriteLine(e);
                    return ActionResult.Trap;
                }
            }
        }

        frame = null;
        return ActionResult.FunctionNotFound;
    }

    public void PrintStatus(bool pass, string reason, int line) {
        // don't print passes
        if (pass) {
            return;
        }

        Console.ForegroundColor = ConsoleColor.Black;
        if (pass) {
            Console.BackgroundColor = ConsoleColor.Green;
            Console.Write("[PASS]");
        } else {
            Console.BackgroundColor = ConsoleColor.Red;
            Console.Write("[FAIL]");
        }
        Console.ResetColor();
        Console.Write(" #"+line+" "+field+"(");
        for (int i=0;i<args.Length;i++) {
            if (i != 0) {
                Console.Write(", ");
            }
            Console.Write(args[i].PrettyParse());
        }
        Console.WriteLine(") -> "+reason);
    }
}

enum ActionResult {
    FunctionNotFound,
    Trap,
    Okay,
    CompileFailed
}

class TestValue {
    public string type {get;set;}
    public string value {get;set;}

    public static string StringifyResults(Frame results, TestValue[] expected, string sep) {
        string str_results = "[ ";
        string str_expected = "[ ";

        for (int i = 0; i < expected.Length; i++) {
            if (i != 0) {
                str_results += ", ";
                str_expected += ", ";
            }
            str_results += results.GetReturnLong(i).ToString("x");
            str_expected += expected[i].Parse().ToString("x");
        }

        return str_results+" ] "+sep+" "+str_expected+" ]";
    }

    public string PrettyParse() {
        switch (type) {
            case "i32":
                return ((int)UInt32.Parse(value)).ToString();
            case "i64":
            case "externref":
                return ((long)UInt64.Parse(value)).ToString();
            case "f32": {
                var m = (int)UInt32.Parse(value);
                var f = BitConverter.Int32BitsToSingle(m);
                return f.ToString();
            }
            case "f64": {
                var m = (long)UInt64.Parse(value);
                var f = BitConverter.Int64BitsToDouble(m);
                return f.ToString();
            }
            default:
                throw new Exception("todo parse: "+type+" "+value);
        }
    }

    public NanKind GetNanKind() {
        if (type == "f32" || type == "f64") {
            if (value == "nan:canonical") {
                return NanKind.Canonical;
            } else if (value == "nan:arithmetic") {
                return NanKind.Arithmetic;
            }
        }
        return NanKind.Number;
    }

    public long Parse() {
        if (value == "null") {
            return 0;
        }
        switch (type) {
            case "i32":
            case "f32":
                if (value.StartsWith("nan:")) {
                    return 0x7f000000;
                }
                return UInt32.Parse(value);
            case "i64":
            case "f64":
            case "externref":
                if (value.StartsWith("nan:")) {
                    return 0x7ff0000000000000;
                }
                return (long)UInt64.Parse(value);
            default:
                throw new Exception("todo parse: "+type+" "+value);
        }
    }

    public bool Check(long val) {
        NanKind expected_nan = GetNanKind();
        long expected_val = Parse();

        // don't bother with robust canonicalization testing
        // this will wrongly accept +/- inf as well
        if (expected_nan != NanKind.Number) {
            return (expected_val & val) == expected_val;
        } else {
            return val == expected_val;
        }
    }
}

enum NanKind {
    Canonical,
    Arithmetic,
    Number
}
