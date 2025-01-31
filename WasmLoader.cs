using System.Text;
using System.Xml;

public class WasmModule : BaseReader {
    const uint MAGIC = 0x6d736100;
    const uint VERSION = 1;

    List<FunctionType> FunctionTypes = new List<FunctionType>();
    public List<WasmFunction> Functions = new List<WasmFunction>();
    List<WasmTable> Tables = new List<WasmTable>();
    List<WasmMemory> Memories = new List<WasmMemory>();

    public Dictionary<string,object> Exports = new Dictionary<string, object>();

    public WasmModule(Stream input) {
        Reader = new BinaryReader(input);

        var magic = Reader.ReadUInt32();
        if (magic != MAGIC) {
            throw new Exception("incorrect wasm magic");
        }
        var version = Reader.ReadUInt32();
        if (version != VERSION) {
            throw new Exception("incorrect wasm version");
        }

        for (;;) {
            byte section_id;
            try {
                section_id = Reader.ReadByte();
            } catch (Exception) {
                break;
            }
            int section_size = Reader.Read7BitEncodedInt();
            var next_section = Reader.BaseStream.Position + section_size;

            switch (section_id) {
                case 1: // type
                    ReadTypes();
                    break;
                case 3: // function
                    ReadFunctions();
                    break;
                case 4: // table
                    ReadTables();
                    break;
                case 5: // memory
                    ReadMemories();
                    break;
                case 6: // global
                    ReadGlobals();
                    break;
                case 7: // export
                    ReadExports();
                    break;
                //case 9: // element
                case 10: // code
                    ReadCode();
                    break;
                //case 11: // data
                default:
                    Console.WriteLine("? "+section_id+" "+section_size);
                    break;
            }

            Reader.BaseStream.Seek(next_section, SeekOrigin.Begin);
        }

    }

    private string ReadString() {
        int length = Reader.Read7BitEncodedInt();
        return Encoding.UTF8.GetString(Reader.ReadBytes(length));
    }

    private Limit ReadLimit() {
        bool has_max = Reader.ReadBoolean();
        int min = Reader.Read7BitEncodedInt();
        if (has_max) {
            return new Limit{
                Min = min,
                Max = Reader.Read7BitEncodedInt()
            };
        } else {
            return new Limit{
                Min = min
            };
        }
    }

    private void ReadTypes() {
        int count = Reader.Read7BitEncodedInt();
        for (int i=0;i<count;i++) {
            var func_ty = new FunctionType();
            byte kind = Reader.ReadByte();
            if (kind != 0x60) {
                throw new Exception("non-function type in function types section");
            }
            int in_count = Reader.Read7BitEncodedInt();
            for (int j=0;j<in_count;j++) {
                func_ty.Inputs.Add(ReadValType());
            }
            int out_count = Reader.Read7BitEncodedInt();
            for (int j=0;j<out_count;j++) {
                func_ty.Outputs.Add(ReadValType());
            }
            FunctionTypes.Add(func_ty);
        }
    }

    private void ReadFunctions() {
        int count = Reader.Read7BitEncodedInt();
        for (int i=0;i<count;i++) {
            int index = Reader.Read7BitEncodedInt();
            var func_ty = FunctionTypes[index];
            Console.WriteLine("func "+func_ty);
            Functions.Add(new WasmFunction(this,func_ty));
        }
    }

    private void ReadTables() {
        int count = Reader.Read7BitEncodedInt();
        for (int i=0;i<count;i++) {
            ValType tt = ReadValType();
            var limit = ReadLimit();
            Console.WriteLine("table "+tt+" "+limit);
            Tables.Add(new WasmTable(tt,limit));
        }
    }

    private void ReadMemories() {
        int count = Reader.Read7BitEncodedInt();
        for (int i=0;i<count;i++) {
            var limit = ReadLimit();
            Console.WriteLine("memory "+limit);
            Memories.Add(new WasmMemory(limit));
        }
    }

    private void ReadGlobals() {
        Console.WriteLine("TODO read globals");

        /*int count = Reader.Read7BitEncodedInt();
        for (int i=0;i<count;i++) {
            var ty = ReadValType();
            bool is_mut = Reader.ReadBoolean();
            Console.WriteLine("global "+is_mut+" "+ty);
        }*/
    }

    private void ReadExports() {
        int count = Reader.Read7BitEncodedInt();
        for (int i=0;i<count;i++) {
            string name = ReadString();
            Console.WriteLine("extern "+name);
            var kind = Reader.ReadByte();
            int index = Reader.Read7BitEncodedInt();
            switch (kind) {
                case 0: // function
                    Exports[name] = Functions[index];
                    break;
                case 1: // table
                    Exports[name] = Tables[index];
                    break;
                case 2: // memory
                    Exports[name] = Memories[index];
                    break;
                case 3: // global
                    Console.WriteLine("TODO export global");
                    break;
            }
        }
    }

    private void ReadCode() {
        int count = Reader.Read7BitEncodedInt();
        for (int i=0;i<count;i++) {
            var function = Functions[i];
            int size = Reader.Read7BitEncodedInt();

            function._SetCodeIndex(Reader.BaseStream.Position);

            Reader.BaseStream.Seek(size, SeekOrigin.Current);
        }
    }
}

public class FunctionType {
    public List<ValType> Inputs = new List<ValType>();
    public List<ValType> Outputs = new List<ValType>();

    public override string ToString()
    {
        string res = "(";
        for (int i=0;i<Inputs.Count;i++) {
            if (i != 0) {
                res += ',';
            }
            res += Inputs[i];
        }
        res += ") -> (";
        for (int i=0;i<Outputs.Count;i++) {
            if (i != 0) {
                res += ',';
            }
            res += Outputs[i];
        }
        res += ")";

        return res;
    }
}

public enum ValType: byte {
    I32 = 0x7F,
    I64 = 0x7E,
    F32 = 0x7D,
    F64 = 0x7C,

    FuncRef = 0x70,
    ExternRef = 0x6F,

    Void = 0x40
}

public class WasmFunction {
    WasmModule Module;
    public FunctionType Sig;
    long CodeIndex;
    FunctionBody Body;

    public WasmFunction(WasmModule module, FunctionType sig) {
        Module = module;
        Sig = sig;
    }

    public void _SetCodeIndex(long index) {
        if (CodeIndex != 0) {
            throw new Exception("code index already set");
        }
        CodeIndex = index;
    }

    public FunctionBody GetBody() {
        if (Body == null) {
            Body = new FunctionBody(Module, Sig, CodeIndex);
        }
        return Body;
    }
}

public class FunctionBody : BaseReader {
    List<ValType> Locals = new List<ValType>();

    public FunctionBody(WasmModule module, FunctionType sig, long index) {
        Reader = module.Reader;
        Reader.BaseStream.Seek(index, SeekOrigin.Begin);

        foreach (var input in sig.Inputs) {
            Locals.Add(input);
        }

        int local_count = Reader.Read7BitEncodedInt();
        for (int j=0;j<local_count;j++) {
            int ty_count = Reader.Read7BitEncodedInt();
            var ty = ReadValType();
            for (int i=0;i<ty_count;i++) {
                Locals.Add(ty);
            }
        }

        ValType ret_type;
        switch (sig.Outputs.Count) {
            case 0:
                ret_type = ValType.Void;
                break;
            case 1:
                ret_type = sig.Outputs[0];
                break;
            default:
                throw new Exception("bad function output count");
        }

        for (int i=0;i<Locals.Count;i++) {
            var m = Locals[i];
            Console.WriteLine("local "+i+" "+m);
        }

        ReadExpression(Locals,module.Functions,ret_type);
    }
}

class WasmTable {
    ValType Ty;
    Limit Limit;
    List<Object> Items;

    public WasmTable(ValType ty, Limit limit) {
        Ty = ty;
        Limit = limit;
        Items = new object[Limit.Min].ToList();
    }
}

class WasmMemory {
    Limit Limit;
    byte[] Data;

    public WasmMemory(Limit limit) {
        Limit = limit;
        Data = new byte[Limit.Min];
    }
}

struct Limit {
    public int Min;
    public int? Max;

    public override string ToString()
    {
        if (Max != null) {
            return Min+".."+Max;
        } else {
            return Min+"..";
        }
    }
}

public abstract class BaseReader {
    public BinaryReader Reader;

    protected ValType ReadValType() {
        byte b = Reader.ReadByte();
        return (ValType)b;
    }

    private long ReadSignedInt() {
        int bit_count = 0;
        ulong result = 0;
        byte b;
        do {
            b = Reader.ReadByte();
            result |= (ulong)(b & 0x7F) << bit_count;
            bit_count += 7;
        } while ((b & 0x80) != 0);

        int shift = 64 - bit_count;
        var final = ((long)result << shift) >> shift;

        return final;
    }

    protected void ReadExpression(List<ValType> local_types, List<WasmFunction> functions, ValType ret_type) {
        var builder = new IRBuilder();

        for (;;) {
            byte code = Reader.ReadByte();
            switch (code) {
                case 0x00: {
                    builder.TerminateBlock(new Trap(builder.CurrentBlock));
                    break;
                }
                case 0x02: {
                    var ty = ReadValType();
                    builder.StartBlock(ty);
                    break;
                }
                case 0x03: {
                    var ty = ReadValType();
                    builder.StartLoop(ty);
                    break;
                }
                case 0x0B:
                    if (builder.EndBlock()) {
                        goto finish;
                    }
                    break;
                case 0x0C: {
                    var br = builder.GetBlock(Reader.Read7BitEncodedInt());
                    builder.TerminateBlock(new Jump(builder.CurrentBlock, br));
                    break;
                }
                case 0x0D: {
                    var br = builder.GetBlock(Reader.Read7BitEncodedInt());
                    var cond = builder.PopExpression();
                    builder.TerminateBlock(new JumpIf(builder.CurrentBlock, cond, br));
                    break;
                }
                case 0x0E: {
                    List<Block> opts = new List<Block>();
                    var label_count = Reader.Read7BitEncodedInt();
                    for (int i=0;i<label_count;i++) {
                        int br = Reader.Read7BitEncodedInt();
                        opts.Add(builder.GetBlock(br));
                    }
                    int br_def = Reader.Read7BitEncodedInt();
                    var def = builder.GetBlock(br_def);
                    var selector = builder.PopExpression();
                    builder.TerminateBlock(new JumpTable(builder.CurrentBlock, selector, opts, def));
                    break;
                }
                case 0x0F: {
                    if (ret_type != ValType.Void) {
                        var val = builder.PopExpression();
                        builder.TerminateBlock(new Return(builder.CurrentBlock, val));
                    } else {
                        builder.TerminateBlock(new Return(builder.CurrentBlock, null));
                    }
                    break;
                }
                case 0x10: {
                    var func = functions[Reader.Read7BitEncodedInt()];
                    for (int i=0;i<func.Sig.Inputs.Count;i++) {
                        builder.PopExpression();
                    }
                    if (func.Sig.Outputs.Count != 0) {
                        throw new Exception("todo actual calls");
                    }
                    break;
                }
                case 0x20: {
                    var local_index = Reader.Read7BitEncodedInt();
                    var ty = local_types[local_index];
                    builder.PushExpression( new GetLocal(local_index, ty) );
                    break;
                }
                case 0x21: {
                    var local_index = Reader.Read7BitEncodedInt();
                    var expr = builder.PopExpression();
                    builder.AddStatement(new Local(local_index), expr);
                    break;
                }
                case 0x22: {
                    var local_index = Reader.Read7BitEncodedInt();
                    var expr = builder.PopExpression();
                    builder.AddStatement(new Local(local_index), expr);
                    var ty = local_types[local_index];
                    builder.PushExpression(new GetLocal(local_index, ty));
                    break;
                }
                case 0x41: {
                    var value = ReadSignedInt();
                    builder.PushExpression( Constant.I32(value) );
                    break;
                }
                case 0x45: {
                    var a = builder.PopExpression();
                    builder.PushExpression(new UnaryOp(ValType.I32, UnaryOpKind.EqualZero, a));
                    break;
                }
                case 0x46: {
                    builder.PushBinaryOp(ValType.I32, BinaryOpKind.Equal);
                    break;
                }
                case 0x4E:
                    builder.PushBinaryOp(ValType.I32, BinaryOpKind.GreaterEqualSigned);
                    break;
                case 0x6A: {
                    builder.PushBinaryOp(ValType.I32, BinaryOpKind.Add);
                    break;
                }
                case 0x6B: {
                    builder.PushBinaryOp(ValType.I32, BinaryOpKind.Sub);
                    break;
                }
                case 0x6C: {
                    builder.PushBinaryOp(ValType.I32, BinaryOpKind.Mul);
                    break;
                }
                case 0x6D: {
                    builder.PushBinaryOp(ValType.I32, BinaryOpKind.DivSigned);
                    break;
                }
                case 0x74: {
                    builder.PushBinaryOp(ValType.I32, BinaryOpKind.ShiftLeft);
                    break;
                }
                default:
                    throw new Exception("todo bytecode "+code.ToString("X"));
            }
        }
        finish:
        int expr_stack_size = builder.GetExpressionStackSize();
        /*if (expr_stack_size != 0) {
            throw new Exception("incorrect final stack size "+expr_stack_size);
        }*/
        
        if (ret_type != ValType.Void) {
            if (expr_stack_size != 1) {
                throw new Exception("incorrect final stack size "+expr_stack_size);
            }
            var val = builder.PopExpression();
            builder.TerminateBlock(new Return(builder.CurrentBlock, val));
        } else {
            if (expr_stack_size != 0) {
                throw new Exception("incorrect final stack size "+expr_stack_size);
            }
            builder.TerminateBlock(new Return(builder.CurrentBlock, null));
        }

        builder.PruneBlocks();
        builder.Dump();

        return;
    }
}
