using System.Runtime.CompilerServices;

struct GetGlobal_I32<INDEX> : Expr<int> where INDEX: struct, Const
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public int Run(Registers reg, Span<long> frame, WasmInstance inst) =>
        (int)inst.Globals[(int)default(INDEX).Run()];
}

struct GetGlobal_I64<INDEX> : Expr<long> where INDEX: struct, Const
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public long Run(Registers reg, Span<long> frame, WasmInstance inst) =>
        inst.Globals[(int)default(INDEX).Run()];
}

struct GetGlobal_F32<INDEX> : Expr<float> where INDEX: struct, Const
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public float Run(Registers reg, Span<long> frame, WasmInstance inst) =>
        BitConverter.Int32BitsToSingle((int)inst.Globals[(int)default(INDEX).Run()]);
}

struct GetGlobal_F64<INDEX> : Expr<double> where INDEX: struct, Const
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public double Run(Registers reg, Span<long> frame, WasmInstance inst) =>
        BitConverter.Int64BitsToDouble(inst.Globals[(int)default(INDEX).Run()]);
}

// setters

struct SetGlobal_I32<INDEX,VALUE,NEXT> : Stmt where INDEX: struct, Const where VALUE: struct, Expr<int> where NEXT: struct, Stmt {
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public Registers Run(Registers reg, Span<long> frame, WasmInstance inst) {
        inst.Globals[(int)default(INDEX).Run()] = (uint)default(VALUE).Run(reg, frame, inst);
        return default(NEXT).Run(reg, frame, inst);
    }
}

struct SetGlobal_I64<INDEX,VALUE,NEXT> : Stmt where INDEX: struct, Const where VALUE: struct, Expr<long> where NEXT: struct, Stmt {
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public Registers Run(Registers reg, Span<long> frame, WasmInstance inst) {
        inst.Globals[(int)default(INDEX).Run()] = default(VALUE).Run(reg, frame, inst);
        return default(NEXT).Run(reg, frame, inst);
    }
}

struct SetGlobal_F32<INDEX,VALUE,NEXT> : Stmt where INDEX: struct, Const where VALUE: struct, Expr<float> where NEXT: struct, Stmt {
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public Registers Run(Registers reg, Span<long> frame, WasmInstance inst) {
        inst.Globals[(int)default(INDEX).Run()] = BitConverter.SingleToUInt32Bits(default(VALUE).Run(reg, frame, inst));
        return default(NEXT).Run(reg, frame, inst);
    }
}

struct SetGlobal_F64<INDEX,VALUE,NEXT> : Stmt where INDEX: struct, Const where VALUE: struct, Expr<double> where NEXT: struct, Stmt {
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public Registers Run(Registers reg, Span<long> frame, WasmInstance inst) {
        inst.Globals[(int)default(INDEX).Run()] = BitConverter.DoubleToInt64Bits(default(VALUE).Run(reg, frame, inst));
        return default(NEXT).Run(reg, frame, inst);
    }
}
