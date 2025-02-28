using System.Data;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

struct StaticCall<FUNC_INDEX,FRAME_INDEX,ARGS> : Expr<long>
    where FUNC_INDEX : struct, Const
    where FRAME_INDEX : struct, Const
    where ARGS : struct, ArgWrite
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public long Run(ref Registers reg, Span<long> frame, WasmInstance inst)
    {
        default(ARGS).Run(reg, frame, inst);
        var arg_span = frame.Slice((int)default(FRAME_INDEX).Run());
        var func = inst.Functions[default(FUNC_INDEX).Run()];
        return func.Call(arg_span, inst);
    }
}

struct DynamicCall<FUNC_INDEX,TABLE_INDEX,FRAME_INDEX,SIG_ID,ARGS> : Expr<long>
    where FUNC_INDEX : struct, Expr<int>
    where TABLE_INDEX : struct, Const
    where FRAME_INDEX : struct, Const
    where SIG_ID : struct, Const
    where ARGS : struct, ArgWrite
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public long Run(ref Registers reg, Span<long> frame, WasmInstance inst)
    {
        default(ARGS).Run(reg, frame, inst);
        var arg_span = frame.Slice((int)default(FRAME_INDEX).Run());
        int func_index = default(FUNC_INDEX).Run(ref reg, frame, inst);
        int table_index = (int)default(TABLE_INDEX).Run();
        var pair = inst.DynamicCallTable[table_index][func_index];
        int expected_sig_id = (int)default(SIG_ID).Run();
        if (pair.SigId != expected_sig_id) {
            throw new Exception("dynamic call type error: "+pair.SigId+" != "+expected_sig_id+" / "+table_index+", "+func_index);
        }
        //throw new Exception("todo call "+func_index);
        //var func = inst.Functions[default(FUNC_INDEX).Run()];
        return pair.Callable.Call(arg_span, inst);
    }
}

interface ArgRead {
    Registers Run(Span<long> args, Span<long> frame);
}

struct ArgRead0 : ArgRead
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public Registers Run(Span<long> args, Span<long> frame) => default;
}

struct ArgRead1 : ArgRead
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public Registers Run(Span<long> args, Span<long> frame) {
        Registers reg = default;
        reg.R0 = args[0];
        return reg;
    }
}

struct ArgRead2 : ArgRead
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public Registers Run(Span<long> args, Span<long> frame) {
        Registers reg = default;
        reg.R0 = args[0];
        reg.R1 = args[1];
        return reg;
    }
}

struct ArgRead3 : ArgRead
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public Registers Run(Span<long> args, Span<long> frame) {
        Registers reg = default;
        reg.R0 = args[0];
        reg.R1 = args[1];
        reg.R2 = args[2];
        return reg;
    }
}

struct ArgRead4 : ArgRead
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public Registers Run(Span<long> args, Span<long> frame) {
        Registers reg = default;
        reg.R0 = args[0];
        reg.R1 = args[1];
        reg.R2 = args[2];
        reg.R3 = args[3];
        return reg;
    }
}

struct ArgRead5 : ArgRead
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public Registers Run(Span<long> args, Span<long> frame) {
        Registers reg = default;
        reg.R0 = args[0];
        reg.R1 = args[1];
        reg.R2 = args[2];
        reg.R3 = args[3];
        reg.R4 = args[4];
        return reg;
    }
}

struct ArgRead6 : ArgRead
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public Registers Run(Span<long> args, Span<long> frame) {
        Registers reg = default;
        reg.R0 = args[0];
        reg.R1 = args[1];
        reg.R2 = args[2];
        reg.R3 = args[3];
        reg.R4 = args[4];
        reg.R5 = args[5];
        return reg;
    }
}

struct ArgRead7 : ArgRead
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public Registers Run(Span<long> args, Span<long> frame) {
        Registers reg = default;
        reg.R0 = args[0];
        reg.R1 = args[1];
        reg.R2 = args[2];
        reg.R3 = args[3];
        reg.R4 = args[4];
        reg.R5 = args[5];
        reg.R6 = args[6];
        return reg;
    }
}

struct ArgReadN<COUNT,VAR_BASE> : ArgRead
    where COUNT : struct, Const
    where VAR_BASE : struct, Const
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public Registers Run(Span<long> args, Span<long> frame) {
        if (Config.USE_REGISTERS) {
            Registers reg = default;
            reg.R0 = args[0];
            reg.R1 = args[1];
            reg.R2 = args[2];
            reg.R3 = args[3];
            reg.R4 = args[4];
            reg.R5 = args[5];
            reg.R6 = args[6];
            int count = (int)default(COUNT).Run();
            int var_base = (int)default(VAR_BASE).Run();
            for (int i=7;i<count;i++) {
                frame[var_base+i-7] = args[i];
            }
            return reg;
        } else {
            int count = (int)default(COUNT).Run();
            int var_base = (int)default(VAR_BASE).Run();
            for (int i=0;i<count;i++) {
                frame[var_base+i] = args[i];
            }
            return default;
        }
    }
}

interface ArgWrite {
    void Run(Registers reg, Span<long> frame, WasmInstance inst);
}

struct ArgWriteNone : ArgWrite {
    public void Run(Registers reg, Span<long> frame, WasmInstance inst) {}
}

struct ArgWriteI32<EXPR, INDEX, NEXT> : ArgWrite
    where EXPR : struct, Expr<int>
    where INDEX : struct, Const
    where NEXT : struct, ArgWrite
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public void Run(Registers reg, Span<long> frame, WasmInstance inst)
    {
        frame[(int)default(INDEX).Run()] = default(EXPR).Run(ref reg, frame, inst);
        default(NEXT).Run(reg, frame, inst);
    }
}

struct ArgWriteI64<EXPR, INDEX, NEXT> : ArgWrite
    where EXPR : struct, Expr<long>
    where INDEX : struct, Const
    where NEXT : struct, ArgWrite
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public void Run(Registers reg, Span<long> frame, WasmInstance inst)
    {
        frame[(int)default(INDEX).Run()] = default(EXPR).Run(ref reg, frame, inst);
        default(NEXT).Run(reg, frame, inst);
    }
}

struct ArgWriteF32<EXPR, INDEX, NEXT> : ArgWrite
    where EXPR : struct, Expr<float>
    where INDEX : struct, Const
    where NEXT : struct, ArgWrite
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public void Run(Registers reg, Span<long> frame, WasmInstance inst)
    {
        frame[(int)default(INDEX).Run()] = BitConverter.SingleToUInt32Bits(default(EXPR).Run(ref reg, frame, inst));
        default(NEXT).Run(reg, frame, inst);
    }
}

struct ArgWriteF64<EXPR, INDEX, NEXT> : ArgWrite
    where EXPR : struct, Expr<double>
    where INDEX : struct, Const
    where NEXT : struct, ArgWrite
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public void Run(Registers reg, Span<long> frame, WasmInstance inst)
    {
        frame[(int)default(INDEX).Run()] = BitConverter.DoubleToInt64Bits(default(EXPR).Run(ref reg, frame, inst));
        default(NEXT).Run(reg, frame, inst);
    }
}
