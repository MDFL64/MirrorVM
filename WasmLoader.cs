using System.ComponentModel;
using System.Diagnostics;
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
    Block InitialBlock;
    IBody Compiled;

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

        InitialBlock = ReadExpression(Locals,module.Functions,ret_type);
    }

    public IBody Compile() {
        if (Compiled == null) {
            Compiled = HellBuilder.Compile(InitialBlock);
        }
        return Compiled;
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

        int shift = bit_count < 64 ? 64 - bit_count : 0;
        var final = ((long)result << shift) >> shift;

        return final;
    }

    protected Block ReadExpression(List<ValType> local_types, List<WasmFunction> functions, ValType ret_type) {
        var builder = new IRBuilder();

        for (;;) {
            byte code = Reader.ReadByte();
            //Console.WriteLine("instr "+code.ToString("x"));
            switch (code) {
                case 0x00: {
                    builder.TerminateBlock(new Trap(builder.CurrentBlock));
                    break;
                }
                case 0x01: break; // nop
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
                case 0x04: {
                    var ty = ReadValType();
                    builder.StartIf(ty);
                    break;
                }
                case 0x05: {
                    builder.StartElse();
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
                case 0x1B: {
                    var cond = builder.PopExpression();
                    var b = builder.PopExpression();
                    var a = builder.PopExpression();
                    builder.PushExpression(new SelectOp(a,b,cond));
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
                    var ty = local_types[local_index];
                    var expr = builder.PopExpression();
                    builder.AddStatement(new Local(local_index, ty), expr);
                    break;
                }
                case 0x22: {
                    var local_index = Reader.Read7BitEncodedInt();
                    var ty = local_types[local_index];
                    var expr = builder.PopExpression();
                    builder.AddStatement(new Local(local_index, ty), expr);
                    builder.PushExpression(new GetLocal(local_index, ty));
                    break;
                }
                case 0x41: {
                    var value = ReadSignedInt();
                    builder.PushExpression( Constant.I32(value) );
                    break;
                }
                case 0x42: {
                    var value = ReadSignedInt();
                    builder.PushExpression( Constant.I64(value) );
                    break;
                }
                // unary
                case 0x45:
                case 0x50:

                case 0x67:
                case 0x68:
                case 0x69:

                case 0x79:
                case 0x7A:
                case 0x7B:

                case 0x8B:
                case 0x8C:
                case 0x8D:
                case 0x8E:
                case 0x8F:
                case 0x90:
                case 0x91:

                case 0x99:
                case 0x9A:
                case 0x9B:
                case 0x9C:
                case 0x9D:
                case 0x9E:
                case 0x9F:

                case 0xA7:
                case 0xA8:
                case 0xA9:
                case 0xAA:
                case 0xAB:
                case 0xAC:
                case 0xAD:
                case 0xAE:
                case 0xAF:
                case 0xB0:
                case 0xB1:
                case 0xB2:
                case 0xB3:
                case 0xB4:
                case 0xB5:
                case 0xB6:
                case 0xB7:
                case 0xB8:
                case 0xB9:
                case 0xBA:
                case 0xBB:
                case 0xBC:
                case 0xBD:
                case 0xBE:
                case 0xBF:
                case 0xC0:
                case 0xC1:
                case 0xC2:
                case 0xC3:
                case 0xC4:
                {
                    var a = builder.PopExpression();
                    builder.PushExpression(new UnaryOp((UnaryOpKind)code, a));
                    break;
                }

                // binary
                case 0x46:
                case 0x47:
                case 0x48:
                case 0x49:
                case 0x4A:
                case 0x4B:
                case 0x4C:
                case 0x4D:
                case 0x4E:
                case 0x4F:
                // gap - eqz
                case 0x51:
                case 0x52:
                case 0x53:
                case 0x54:
                case 0x55:
                case 0x56:
                case 0x57:
                case 0x58:
                case 0x59:
                case 0x5A:
                case 0x5B:
                case 0x5C:
                case 0x5D:
                case 0x5E:
                case 0x5F:
                case 0x60:
                case 0x61:
                case 0x62:
                case 0x63:
                case 0x64:
                case 0x65:
                case 0x66:
                // gap for unary
                case 0x6A:
                case 0x6B:
                case 0x6C:
                case 0x6D:
                case 0x6E:
                case 0x6F:
                case 0x70:
                case 0x71:
                case 0x72:
                case 0x73:
                case 0x74:
                case 0x75:
                case 0x76:
                case 0x77:
                case 0x78:
                case 0x7C:
                case 0x7D:
                case 0x7E:
                case 0x7F:
                case 0x80:
                case 0x81:
                case 0x82:
                case 0x83:
                case 0x84:
                case 0x85:
                case 0x86:
                case 0x87:
                case 0x88:
                case 0x89:
                case 0x8A:

                case 0x92:
                case 0x93:
                case 0x94:
                case 0x95:
                case 0x96:
                case 0x97:
                case 0x98:

                case 0xA0:
                case 0xA1:
                case 0xA2:
                case 0xA3:
                case 0xA4:
                case 0xA5:
                case 0xA6:

                    builder.PushBinaryOp((BinaryOpKind)code);
                    break;
                default:
                    throw new Exception("todo bytecode "+code.ToString("X"));
            }
        }
        finish:

        int expr_stack_size = builder.GetExpressionStackSize();
        if (expr_stack_size == 0) {
            // just assume this is fine
            builder.TerminateBlock(new Return(builder.CurrentBlock, null));
        } else if (expr_stack_size == 1) {
            var val = builder.PopExpression();
            builder.TerminateBlock(new Return(builder.CurrentBlock, val));
        } else {
            throw new Exception("bad final stack size = "+expr_stack_size);
        }

        builder.PruneBlocks();
        builder.Dump(true);
        /*var f = HellBuilder.Compile(builder.InitialBlock);
        Registers r = default;
        r.R0 = 123;
        r.R1 = 456;
        var _ = f.Run(r);
        var sw = Stopwatch.StartNew();
        var res = f.Run(r);
        Console.WriteLine("DONE: "+res);
        Console.WriteLine("> "+sw.Elapsed);*/
        return builder.InitialBlock;
    }
}
