class TestCommands {
    public TestCommand[] commands {get;set;}
    public WasmModule Module;

    public void Run() {
        int total = 0;
        int passed = 0;
        foreach (var cmd in commands) {
            switch (cmd.type) {
                case "module": {
                    var code = File.ReadAllBytes("tests/"+cmd.filename);
                    Module = new WasmModule(new MemoryStream(code));
                    break;
                }
                case "assert_return": {
                    total++;
                    if (cmd.expected.Length != 1) {
                        throw new Exception("expected length = "+cmd.expected.Length);
                    }
                    var expected_val = cmd.expected[0].Parse();
                    var (res,val) = cmd.action.Run(Module);
                    if (res != ActionResult.Okay) {
                        cmd.action.PrintStatus(false,res.ToString(),cmd.line);
                    } else {
                        if (val == expected_val) {
                            cmd.action.PrintStatus(true,val+" == "+expected_val,cmd.line);
                            passed++;
                        } else {
                            cmd.action.PrintStatus(false,val+" != "+expected_val,cmd.line);
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
        Console.BackgroundColor = ConsoleColor.Cyan;
        Console.ForegroundColor = ConsoleColor.Black;
        Console.WriteLine("[ "+passed+" / "+total+" TESTS PASSED ]");
        Console.ResetColor();
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
                try {
                    long res_val = callable.Run(reg);
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
        Console.ForegroundColor = ConsoleColor.Black;
        if (pass) {
            Console.BackgroundColor = ConsoleColor.Green;
            Console.Write("[PASS]");
        } else {
            Console.BackgroundColor = ConsoleColor.Red;
            Console.Write("[FAIL]");
        }
        Console.ResetColor();
        Console.Write("#"+line+" "+field+"(");
        for (int i=0;i<args.Length;i++) {
            if (i != 0) {
                Console.Write(", ");
            }
            Console.Write(args[i].value);
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

    public long Parse() {
        switch (type) {
            case "i32":
                return (int)UInt32.Parse(value);
            case "i64":
                return (long)UInt64.Parse(value);
            default:
                throw new Exception("todo parse: "+type);
        }
    }
}
