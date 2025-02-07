using System.Text.Json;

class TestCommands {
    public static void RunFile(string name) {
        Console.WriteLine(">>> "+name);
        var cmds = JsonSerializer.Deserialize<TestCommands>(File.ReadAllText("tests/"+name+".json"));
        cmds.Run();
    }

    public TestCommand[] commands {get;set;}
    public WasmModule Module;

    public void Run() {
        int total = 0;
        int passed = 0;
        foreach (var cmd in commands) {
            if (total - passed >= 5) {
                Console.WriteLine("--- TOO MANY FAILED TESTS");
                return;
            }
            switch (cmd.type) {
                case "module": {
                    var code = File.ReadAllBytes("tests/"+cmd.filename);
                    Module = new WasmModule(new MemoryStream(code));
                    break;
                }
                case "action": {
                    total++;
                    var (res,val) = cmd.action.Run(Module);
                    bool this_passed = res == ActionResult.Okay;
                    cmd.action.PrintStatus(this_passed,res.ToString(),cmd.line);
                    if (this_passed) {
                        passed++;
                    }
                    break;
                }
                case "assert_return": {
                    total++;
                    if (cmd.expected.Length != 1) {
                        throw new Exception("expected length = "+cmd.expected.Length);
                    }
                    var expected_nan = cmd.expected[0].GetNanKind();
                    long expected_val = cmd.expected[0].Parse();
                    var (res,val) = cmd.action.Run(Module);
                    if (res != ActionResult.Okay) {
                        cmd.action.PrintStatus(false,res.ToString(),cmd.line);
                    } else {
                        if (expected_nan == NanKind.Canonical) {
                            long expected_alt = expected_val | (expected_val << 1);
                            if (val == expected_val || val == expected_alt) {
                                cmd.action.PrintStatus(true,val.ToString("x")+" == canonical nan",cmd.line);
                                passed++;
                            } else {
                                cmd.action.PrintStatus(false,val.ToString("x")+" != canonical nan",cmd.line);
                            }
                        } else if (expected_nan == NanKind.Arithmetic) {
                            if ((expected_val & val) == expected_val) {
                                cmd.action.PrintStatus(true,val.ToString("x")+" == arithmetic nan",cmd.line);
                                passed++;
                            } else {
                                cmd.action.PrintStatus(false,val.ToString("x")+" != arithmetic nan",cmd.line);
                            }
                        } else {
                            if (val == expected_val) {
                                cmd.action.PrintStatus(true,val.ToString("x")+" == "+expected_val.ToString("x"),cmd.line);
                                passed++;
                            } else {
                                cmd.action.PrintStatus(false,val.ToString("x")+" != "+expected_val.ToString("x"),cmd.line);
                            }
                        }

                    }
                    break;
                }
                case "assert_trap": {
                    total++;
                    var (res,val) = cmd.action.Run(Module);
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

    public (ActionResult,long) Run(WasmModule module) {
        if (type != "invoke") {
            throw new Exception("unknown action: "+type);
        }

        if (module.Exports.TryGetValue(field, out object item)) {
            var func = item as WasmFunction;
            if (func != null) {
                IBody callable;
                try {
                    callable = func.GetBody().Compile();
                } catch (Exception e) {
                    if (e.Message != "attempted to number block twice") {
                        Console.WriteLine(e);
                    }
                    return (ActionResult.CompileFailed,0);
                }

                Registers reg = new Registers();
                for (int i=0;i<args.Length;i++) {
                    var val = args[i].Parse();
                    reg.Set(i,val);
                }
                var instance = new WasmInstance();
                try {

                    long res_val = callable.Run(reg, instance);
                    return (ActionResult.Okay,res_val);
                } catch (Exception) {
                    //Console.WriteLine(e);
                    return (ActionResult.Trap,0);
                }
            }
        }
        return (ActionResult.FunctionNotFound,0);
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

    public string PrettyParse() {
        switch (type) {
            case "i32":
                return ((int)UInt32.Parse(value)).ToString();
            case "i64":
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
        switch (type) {
            case "i32":
            case "f32":
                if (value.StartsWith("nan:")) {
                    return 0x7fc00000;
                }
                return UInt32.Parse(value);
            case "i64":
            case "f64":
                if (value.StartsWith("nan:")) {
                    return 0x7ff8000000000000;
                }
                return (long)UInt64.Parse(value);
            default:
                throw new Exception("todo parse: "+type+" "+value);
        }
    }
}

enum NanKind {
    Canonical,
    Arithmetic,
    Number
}
