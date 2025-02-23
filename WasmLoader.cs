using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Xml;

public class ImportProvider {
    public virtual long ImportGlobal(string module, string name, ValType ty) {
        throw new NotImplementedException("import global "+module+":"+name+" :: "+ty);
    }
}

public class WasmModule : BaseReader {
    const uint MAGIC = 0x6d736100;
    const uint VERSION = 1;

    public List<FunctionType> FunctionTypes = new List<FunctionType>();
    public List<WasmFunction> Functions = new List<WasmFunction>();
    public List<WasmTable> Tables = new List<WasmTable>();
    List<WasmMemory> Memories = new List<WasmMemory>();
    public List<(ValType,long)> Globals = new List<(ValType, long)>();

    public Dictionary<string,object> Exports = new Dictionary<string, object>();

    public WasmModule(Stream input, ImportProvider imports) {
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
                case 2: // imports
                    ReadImports(imports);
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
                case 9: // element
                    ReadElements();
                    break;
                case 10: // code
                    ReadCode();
                    break;
                case 11:
                    ReadData();
                    break;
                default:
                    //Console.WriteLine("? "+section_id+" "+section_size);
                    break;
            }

            Reader.BaseStream.Seek(next_section, SeekOrigin.Begin);
        }

    }

    public byte[] GetInitialMemory() {
        if (Memories.Count >= 1) {
            return (byte[])Memories[0].Data.Clone();
        }
        return null;
    }

    // nothing specifies that function types must be unique,
    // so we need to find canonical ids for dynamic calls
    public int FindSigId(FunctionType sig) {
        for (int i=0;i<FunctionTypes.Count;i++) {
            if (sig.Equals(FunctionTypes[i])) {
                return i;
            }
        }
        throw new Exception("failed to find sig id: "+sig);
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
            int sig_index = Reader.Read7BitEncodedInt();
            var func_ty = FunctionTypes[sig_index];
            Functions.Add(new WasmFunction(this,func_ty,i));
        }
    }

    private void ReadTables() {
        int count = Reader.Read7BitEncodedInt();
        for (int i=0;i<count;i++) {
            ValType tt = ReadValType();
            var limit = ReadLimit();
            Tables.Add(new WasmTable(tt,limit));
        }
    }

    private void ReadElements() {
        int count = Reader.Read7BitEncodedInt();
        for (int i=0;i<count;i++) {
            int b = Reader.ReadByte();
            switch (b) {
                case 0: {
                    var expr = HellBuilder.Compile(ReadExpression([],[ValType.I32],this),0,1);
                    int offset = (int)expr.Call([],null);
                    int entry_count = Reader.Read7BitEncodedInt();
                    for (int j=0;j<entry_count;j++) {
                        int func_index = Reader.Read7BitEncodedInt();
                        Tables[0].Set(offset + j, func_index);
                    }
                    break;
                }
                case 2: {
                    int table_index = Reader.Read7BitEncodedInt();
                    var expr = HellBuilder.Compile(ReadExpression([],[ValType.I32],this),0,1);
                    int offset = (int)expr.Call([],null);
                    Reader.ReadByte(); // elem kind (0)

                    int entry_count = Reader.Read7BitEncodedInt();
                    for (int j=0;j<entry_count;j++) {
                        int func_index = Reader.Read7BitEncodedInt();
                        Tables[table_index].Set(offset + j, func_index);
                    }

                    //Console.WriteLine("??? "+table_index+" "+offset);
                    //throw new Exception("-");
                    break;
                }
                default:
                    throw new Exception("element "+b);
            }
        }
    }

    private void ReadMemories() {
        int count = Reader.Read7BitEncodedInt();
        for (int i=0;i<count;i++) {
            var limit = ReadLimit();
            Memories.Add(new WasmMemory(limit));
        }
    }

    private void ReadGlobals() {
        int count = Reader.Read7BitEncodedInt();
        for (int i=0;i<count;i++) {
            var ty = ReadValType();
            bool _ = Reader.ReadBoolean(); // mutable

            var expr = HellBuilder.Compile(ReadExpression([],[ty],this),0,1);
            var inst = new WasmInstance(this);
            var value = expr.Call([],inst);
            Globals.Add((ty, value));
        }
    }

    private void ReadImports(ImportProvider imports) {
        int count = Reader.Read7BitEncodedInt();
        for (int i=0;i<count;i++) {
            string mod = ReadString();
            string name = ReadString();
            byte kind = Reader.ReadByte();
            switch (kind) {
                case 3:
                    var ty = ReadValType();
                    bool _ = Reader.ReadBoolean(); // mutable
                    long value = imports.ImportGlobal(mod,name,ty);
                    Globals.Add((ty,value));
                    break;
                default:
                    Console.WriteLine(mod+" "+name+" "+kind);
                    throw new Exception("stop");
            }
        }
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
                    Functions[index].DebugName = name;
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

    private void ReadData() {
        int count = Reader.Read7BitEncodedInt();
        for (int i=0;i<count;i++) {
            int b = Reader.Read7BitEncodedInt();
            int offset = 0;
            int memory_index = 0;
            if (b == 0 || b == 2) {
                if (b == 2) {
                    memory_index = Reader.Read7BitEncodedInt();
                }
                var expr = HellBuilder.Compile(ReadExpression([],[ValType.I32],this),0,1);
                offset = (int)expr.Call([],null);
            } else if (b == 1) {
                // okay, passive
            } else {
                throw new Exception("data? "+b);
            }

            int byte_count = Reader.Read7BitEncodedInt();
            for (int j=0;j<byte_count;j++) {
                Memories[memory_index].Data[offset + j] = Reader.ReadByte();
            }
        }
    }
}

public class FunctionType {
    public List<ValType> Inputs = new List<ValType>();
    public List<ValType> Outputs = new List<ValType>();

    public FunctionType() {}
    public FunctionType(List<ValType> inputs, List<ValType> outputs) {
        Inputs = inputs;
        Outputs = outputs;
    }

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

    public override bool Equals(object obj)
    {
        if (base.Equals(obj)) {
            return true;
        }
        if (obj is FunctionType other) {
            if (Inputs.Count != other.Inputs.Count) {
                return false;
            }
            if (Outputs.Count != other.Outputs.Count) {
                return false;
            }
            for (int i=0;i<Inputs.Count;i++) {
                if (Inputs[i] != other.Inputs[i]) {
                    return false;
                }
            }
            for (int i=0;i<Outputs.Count;i++) {
                if (Outputs[i] != other.Outputs[i]) {
                    return false;
                }
            }
            return true;
        }
        return false;
    }
}

public enum ValType: byte {
    Error,

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

    public string DebugName;

    public WasmFunction(WasmModule module, FunctionType sig, int index) {
        Module = module;
        Sig = sig;
        DebugName = "func"+index;
    }

    public void _SetCodeIndex(long index) {
        if (CodeIndex != 0) {
            throw new Exception("code index already set");
        }
        CodeIndex = index;
    }

    public FunctionBody GetBody() {
        if (Body == null) {
            Body = new FunctionBody(Module, Sig, CodeIndex, DebugName);
        }
        return Body;
    }
}

public class FunctionBody : BaseReader {
    FunctionType Sig;
    List<ValType> Locals = new List<ValType>();
    Block InitialBlock;
    ICallable Compiled;
    string DebugName;

    public FunctionBody(WasmModule module, FunctionType sig, long index, string debug_name) {
        Reader = module.Reader;
        Reader.BaseStream.Seek(index, SeekOrigin.Begin);

        Sig = sig;
        DebugName = debug_name;

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

        InitialBlock = ReadExpression(Locals,sig.Outputs,module);
    }

    public ICallable Compile() {
        if (Compiled == null) {
            Compiled = HellBuilder.Compile(InitialBlock, Sig.Inputs.Count, Sig.Outputs.Count, DebugName);
        }
        return Compiled;
    }
}

public class WasmTable {
    ValType Ty;
    Limit Limit;
    List<object> Items;

    public WasmTable(ValType ty, Limit limit) {
        Ty = ty;
        Limit = limit;
        Items = new object[Limit.Min].ToList();
    }

    public void Set(int index, object o) {
        Items[index] = o;
    }

    public int GetLength() {
        return Items.Count;
    }

    public object Get(int index) {
        return Items[index];
    }
}

class WasmMemory {
    Limit Limit;
    public byte[] Data;

    public WasmMemory(Limit limit) {
        Limit = limit;
        Data = new byte[Limit.Min*65536];
    }
}

public struct Limit {
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

    protected ValType[] ReadBlockType(List<FunctionType> table) {
        long x = ReadSignedInt();
        if (x < 0) {
            var res = (ValType)(x & 0x7F);
            if (res == ValType.Void) {
                return [];
            } else {
                return [res];
            }
        } else {
            var func_ty = table[(int)x];
            if (func_ty.Inputs.Count > 0) {
                throw new Exception("block inputs???");
            }
            return func_ty.Outputs.ToArray();
        }
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

    protected Block ReadExpression(List<ValType> local_types, List<ValType> ret_types, WasmModule module) {
        var builder = new IRBuilder(local_types, ret_types);

        for (;;) {
            byte code = Reader.ReadByte();
            //Console.WriteLine("instr "+code.ToString("x")+" "+builder.GetExpressionStackSize());
            switch (code) {
                case 0x00: {
                    builder.TerminateBlock(new Trap(builder.CurrentBlock));
                    break;
                }
                case 0x01: break; // nop
                case 0x02: {
                    var ty = ReadBlockType(module.FunctionTypes);
                    builder.StartBlock(ty);
                    break;
                }
                case 0x03: {
                    var ty = ReadBlockType(module.FunctionTypes);
                    builder.StartLoop(ty);
                    break;
                }
                case 0x04: {
                    var ty = ReadBlockType(module.FunctionTypes);
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
                    var entry = builder.GetBlock(Reader.Read7BitEncodedInt());
                    builder.SpillBlockResult(entry);
                    builder.TerminateBlock(new Jump(builder.CurrentBlock, entry.Block));
                    break;
                }
                case 0x0D: {
                    var entry = builder.GetBlock(Reader.Read7BitEncodedInt());
                    var cond = builder.PopExpression();
                    builder.TeeBlockResult(entry);
                    builder.TerminateBlock(new JumpIf(builder.CurrentBlock, cond, entry.Block));
                    break;
                }
                case 0x0E: {
                    List<Block> opts = new List<Block>();
                    // add one for default
                    var label_count = Reader.Read7BitEncodedInt() + 1;
                    // if there are results, then we need to shuffle values around
                    Local[] shared_spill_locals = null;
                    for (int block_i=0;block_i<label_count;block_i++) {
                        var info = builder.GetBlock(Reader.Read7BitEncodedInt());
                        if (info.SpillLocals.Length > 0) {
                            // created shared spills
                            if (shared_spill_locals == null) {
                                shared_spill_locals = new Local[info.SpillLocals.Length];
                                for (int i=0;i<shared_spill_locals.Length;i++) {
                                    shared_spill_locals[i] = builder.CreateSpillLocal(info.SpillLocals[i].Type);
                                }
                            }
                            // create a new block which moves shared spills to the block's desired registers
                            var new_block = new Block();
                            for (int i=0;i<shared_spill_locals.Length;i++) {
                                new_block.Statements.Add((info.SpillLocals[i], shared_spill_locals[i]));
                            }
                            new_block.Terminator = new Jump(new_block, info.Block);
                            opts.Add(new_block);
                        } else {
                            opts.Add(info.Block);
                        }
                    }
                    var selector = builder.PopExpression();
                    if (shared_spill_locals != null) {
                        foreach (var local in shared_spill_locals.Reverse()) {
                            var res = builder.PopExpression();
                            builder.AddStatement(local,res);
                        }
                    }
                    builder.TerminateBlock(new JumpTable(builder.CurrentBlock, selector, opts));
                    break;
                }
                case 0x0F: {
                    builder.AddReturn(ret_types.Count);
                    break;
                }
                case 0x10: {
                    int func_index = Reader.Read7BitEncodedInt();
                    var func = module.Functions[func_index];
                    builder.AddCall(func.Sig, func.DebugName, CallKind.Static, func_index);
                    break;
                }
                case 0x11: {
                    int sig_index = Reader.Read7BitEncodedInt();
                    int table_index = Reader.Read7BitEncodedInt();
                    var func_ty = module.FunctionTypes[sig_index];
                    int sig_id = module.FindSigId(func_ty);
                    builder.AddCall(func_ty, null, CallKind.Dynamic, sig_id, table_index);
                    break;
                }
                case 0x1A: {
                    var dropped = builder.PopExpression();
                    // memory reads may trap
                    if (dropped.IsMemoryRead()) {
                        builder.AddStatement(null, dropped);
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
                    builder.PushExpression(new Local(local_index, ty, LocalKind.Variable) );
                    break;
                }
                case 0x21: {
                    var local_index = Reader.Read7BitEncodedInt();
                    var ty = local_types[local_index];
                    var expr = builder.PopExpression();
                    builder.AddStatement(new Local(local_index, ty, LocalKind.Variable), expr);
                    break;
                }
                case 0x22: {
                    var local_index = Reader.Read7BitEncodedInt();
                    var ty = local_types[local_index];
                    var expr = builder.PopExpression();
                    var local = new Local(local_index, ty, LocalKind.Variable);
                    builder.AddStatement(local, expr);
                    builder.PushExpression(local);
                    break;
                }
                case 0x23: {
                    var global_index = Reader.Read7BitEncodedInt();
                    var ty = module.Globals[global_index].Item1;
                    builder.PushExpression(new Global(global_index, ty));
                    break;
                }
                case 0x24: {
                    var global_index = Reader.Read7BitEncodedInt();
                    var ty = module.Globals[global_index].Item1;
                    var expr = builder.PopExpression();
                    builder.AddStatement(new Global(global_index, ty), expr);
                    break;
                }
                // memory ops
                // reads
                case 0x28:
                    Reader.Read7BitEncodedInt();
                    builder.PushMemoryRead(ValType.I32,MemSize.SAME,Reader.Read7BitEncodedInt());
                    break;
                case 0x29:
                    Reader.Read7BitEncodedInt();
                    builder.PushMemoryRead(ValType.I64,MemSize.SAME,Reader.Read7BitEncodedInt());
                    break;
                case 0x2A:
                    Reader.Read7BitEncodedInt();
                    builder.PushMemoryRead(ValType.F32,MemSize.SAME,Reader.Read7BitEncodedInt());
                    break;
                case 0x2B:
                    Reader.Read7BitEncodedInt();
                    builder.PushMemoryRead(ValType.F64,MemSize.SAME,Reader.Read7BitEncodedInt());
                    break;
                case 0x2C:
                    Reader.Read7BitEncodedInt();
                    builder.PushMemoryRead(ValType.I32,MemSize.I8_S,Reader.Read7BitEncodedInt());
                    break;
                case 0x2D:
                    Reader.Read7BitEncodedInt();
                    builder.PushMemoryRead(ValType.I32,MemSize.I8_U,Reader.Read7BitEncodedInt());
                    break;
                case 0x2E:
                    Reader.Read7BitEncodedInt();
                    builder.PushMemoryRead(ValType.I32,MemSize.I16_S,Reader.Read7BitEncodedInt());
                    break;
                case 0x2F:
                    Reader.Read7BitEncodedInt();
                    builder.PushMemoryRead(ValType.I32,MemSize.I16_U,Reader.Read7BitEncodedInt());
                    break;
                case 0x30:
                    Reader.Read7BitEncodedInt();
                    builder.PushMemoryRead(ValType.I64,MemSize.I8_S,Reader.Read7BitEncodedInt());
                    break;
                case 0x31:
                    Reader.Read7BitEncodedInt();
                    builder.PushMemoryRead(ValType.I64,MemSize.I8_U,Reader.Read7BitEncodedInt());
                    break;
                case 0x32:
                    Reader.Read7BitEncodedInt();
                    builder.PushMemoryRead(ValType.I64,MemSize.I16_S,Reader.Read7BitEncodedInt());
                    break;
                case 0x33:
                    Reader.Read7BitEncodedInt();
                    builder.PushMemoryRead(ValType.I64,MemSize.I16_U,Reader.Read7BitEncodedInt());
                    break;
                case 0x34:
                    Reader.Read7BitEncodedInt();
                    builder.PushMemoryRead(ValType.I64,MemSize.I32_S,Reader.Read7BitEncodedInt());
                    break;
                case 0x35:
                    Reader.Read7BitEncodedInt();
                    builder.PushMemoryRead(ValType.I64,MemSize.I32_U,Reader.Read7BitEncodedInt());
                    break;

                // writes
                case 0x36:
                    Reader.Read7BitEncodedInt();
                    builder.AddMemoryWrite(ValType.I32,MemSize.SAME,Reader.Read7BitEncodedInt());
                    break;
                case 0x37:
                    Reader.Read7BitEncodedInt();
                    builder.AddMemoryWrite(ValType.I64,MemSize.SAME,Reader.Read7BitEncodedInt());
                    break;
                case 0x38:
                    Reader.Read7BitEncodedInt();
                    builder.AddMemoryWrite(ValType.F32,MemSize.SAME,Reader.Read7BitEncodedInt());
                    break;
                case 0x39:
                    Reader.Read7BitEncodedInt();
                    builder.AddMemoryWrite(ValType.F64,MemSize.SAME,Reader.Read7BitEncodedInt());
                    break;
                case 0x3A:
                    Reader.Read7BitEncodedInt();
                    builder.AddMemoryWrite(ValType.I32,MemSize.I8_S,Reader.Read7BitEncodedInt());
                    break;
                case 0x3B:
                    Reader.Read7BitEncodedInt();
                    builder.AddMemoryWrite(ValType.I32,MemSize.I16_S,Reader.Read7BitEncodedInt());
                    break;
                case 0x3C:
                    Reader.Read7BitEncodedInt();
                    builder.AddMemoryWrite(ValType.I64,MemSize.I8_S,Reader.Read7BitEncodedInt());
                    break;
                case 0x3D:
                    Reader.Read7BitEncodedInt();
                    builder.AddMemoryWrite(ValType.I64,MemSize.I16_S,Reader.Read7BitEncodedInt());
                    break;
                case 0x3E:
                    Reader.Read7BitEncodedInt();
                    builder.AddMemoryWrite(ValType.I64,MemSize.I32_S,Reader.Read7BitEncodedInt());
                    break;
                
                case 0x3F: {
                    // skip byte (should be 0)
                    Reader.ReadByte();
                    builder.PushExpression(new MemorySize());
                    break;
                }
                case 0x40: {
                    builder.PushExpression(new MemoryGrow(builder.PopExpression()));
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
                case 0x43: {
                    var raw = Reader.ReadUInt32();
                    /*var num = BitConverter.UInt32BitsToSingle(raw);
                    int as_int = (int)num;
                    if ( BitConverter.SingleToUInt32Bits( as_int ) == raw ) {
                        Console.WriteLine("!!! "+as_int);
                    }*/
                    builder.PushExpression( Constant.F32(raw) );
                    break;
                }
                case 0x44: {
                    var raw = Reader.ReadInt64();
                    builder.PushExpression( Constant.F64(raw) );
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

                case 0xD0: {
                    var ty = ReadValType();
                    builder.PushExpression(Constant.NULL(ty));
                    break;
                }
                case 0xFC: {
                    byte code2 = Reader.ReadByte();
                    switch (code2) {
                        case 0:
                        case 1:
                        case 2:
                        case 3:
                        case 4:
                        case 5:
                        case 6:
                        case 7:
                            {
                                var a = builder.PopExpression();
                                builder.PushExpression(new UnaryOp((UnaryOpKind)(0x100+code2), a));
                                break;
                            }
                        default:
                            throw new Exception("todo bytecode FC "+code2.ToString("X"));
                    }
                    break;
                }

                default:
                    throw new Exception("todo bytecode "+code.ToString("X"));
            }
        }
        finish:
        builder.AddReturn(ret_types.Count);

        /*int expr_stack_size = builder.GetExpressionStackSize();
        if (expr_stack_size == 0) {
            // just assume this is fine
            builder.AddReturn(0);
        } else if (expr_stack_size == ret_types.Count) {
            builder.AddReturn(ret_types.Count);
        } else {
            throw new Exception("bad final stack size = "+expr_stack_size+" / "+ret_types.Count);
        }*/

        builder.PruneBlocks();
        builder.LowerLocals();

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
