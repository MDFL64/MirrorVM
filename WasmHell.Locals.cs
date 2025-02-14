using System.Runtime.CompilerServices;

struct GetR0_I32 : Expr<int> { public int Run(Registers reg, Span<long> frame, WasmInstance inst) => (int)reg.R0; }
struct GetR1_I32 : Expr<int> { public int Run(Registers reg, Span<long> frame, WasmInstance inst) => (int)reg.R1; }
struct GetR2_I32 : Expr<int> { public int Run(Registers reg, Span<long> frame, WasmInstance inst) => (int)reg.R2; }
struct GetR3_I32 : Expr<int> { public int Run(Registers reg, Span<long> frame, WasmInstance inst) => (int)reg.R3; }
struct GetR4_I32 : Expr<int> { public int Run(Registers reg, Span<long> frame, WasmInstance inst) => (int)reg.R4; }
struct GetR5_I32 : Expr<int> { public int Run(Registers reg, Span<long> frame, WasmInstance inst) => (int)reg.R5; }
struct GetR6_I32 : Expr<int> { public int Run(Registers reg, Span<long> frame, WasmInstance inst) => (int)reg.R6; }

struct GetR0_I64 : Expr<long> { public long Run(Registers reg, Span<long> frame, WasmInstance inst) => reg.R0; }
struct GetR1_I64 : Expr<long> { public long Run(Registers reg, Span<long> frame, WasmInstance inst) => reg.R1; }
struct GetR2_I64 : Expr<long> { public long Run(Registers reg, Span<long> frame, WasmInstance inst) => reg.R2; }
struct GetR3_I64 : Expr<long> { public long Run(Registers reg, Span<long> frame, WasmInstance inst) => reg.R3; }
struct GetR4_I64 : Expr<long> { public long Run(Registers reg, Span<long> frame, WasmInstance inst) => reg.R4; }
struct GetR5_I64 : Expr<long> { public long Run(Registers reg, Span<long> frame, WasmInstance inst) => reg.R5; }
struct GetR6_I64 : Expr<long> { public long Run(Registers reg, Span<long> frame, WasmInstance inst) => reg.R6; }

struct GetR0_F32 : Expr<float> { public float Run(Registers reg, Span<long> frame, WasmInstance inst) => BitConverter.Int32BitsToSingle((int)reg.R0); }
struct GetR1_F32 : Expr<float> { public float Run(Registers reg, Span<long> frame, WasmInstance inst) => BitConverter.Int32BitsToSingle((int)reg.R1); }
struct GetR2_F32 : Expr<float> { public float Run(Registers reg, Span<long> frame, WasmInstance inst) => BitConverter.Int32BitsToSingle((int)reg.R2); }
struct GetR3_F32 : Expr<float> { public float Run(Registers reg, Span<long> frame, WasmInstance inst) => BitConverter.Int32BitsToSingle((int)reg.R3); }
struct GetR4_F32 : Expr<float> { public float Run(Registers reg, Span<long> frame, WasmInstance inst) => BitConverter.Int32BitsToSingle((int)reg.R4); }
struct GetR5_F32 : Expr<float> { public float Run(Registers reg, Span<long> frame, WasmInstance inst) => BitConverter.Int32BitsToSingle((int)reg.R5); }
struct GetR6_F32 : Expr<float> { public float Run(Registers reg, Span<long> frame, WasmInstance inst) => BitConverter.Int32BitsToSingle((int)reg.R6); }

struct GetR0_F64 : Expr<double> { public double Run(Registers reg, Span<long> frame, WasmInstance inst) => BitConverter.Int64BitsToDouble(reg.R0); }
struct GetR1_F64 : Expr<double> { public double Run(Registers reg, Span<long> frame, WasmInstance inst) => BitConverter.Int64BitsToDouble(reg.R1); }
struct GetR2_F64 : Expr<double> { public double Run(Registers reg, Span<long> frame, WasmInstance inst) => BitConverter.Int64BitsToDouble(reg.R2); }
struct GetR3_F64 : Expr<double> { public double Run(Registers reg, Span<long> frame, WasmInstance inst) => BitConverter.Int64BitsToDouble(reg.R3); }
struct GetR4_F64 : Expr<double> { public double Run(Registers reg, Span<long> frame, WasmInstance inst) => BitConverter.Int64BitsToDouble(reg.R4); }
struct GetR5_F64 : Expr<double> { public double Run(Registers reg, Span<long> frame, WasmInstance inst) => BitConverter.Int64BitsToDouble(reg.R5); }
struct GetR6_F64 : Expr<double> { public double Run(Registers reg, Span<long> frame, WasmInstance inst) => BitConverter.Int64BitsToDouble(reg.R6); }

struct SetR0_I32<VALUE,NEXT> : Stmt where VALUE: struct, Expr<int> where NEXT: struct, Stmt {
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public Registers Run(Registers reg, Span<long> frame, WasmInstance inst) {
        reg.R0 = (uint)default(VALUE).Run(reg, frame, inst); return default(NEXT).Run(reg, frame, inst);
    }
}
struct SetR1_I32<VALUE,NEXT> : Stmt where VALUE: struct, Expr<int> where NEXT: struct, Stmt {
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public Registers Run(Registers reg, Span<long> frame, WasmInstance inst) {
        reg.R1 = (uint)default(VALUE).Run(reg, frame, inst); return default(NEXT).Run(reg, frame, inst);
    }
}
struct SetR2_I32<VALUE,NEXT> : Stmt where VALUE: struct, Expr<int> where NEXT: struct, Stmt {
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public Registers Run(Registers reg, Span<long> frame, WasmInstance inst) {
        reg.R2 = (uint)default(VALUE).Run(reg, frame, inst); return default(NEXT).Run(reg, frame, inst);
    }
}
struct SetR3_I32<VALUE,NEXT> : Stmt where VALUE: struct, Expr<int> where NEXT: struct, Stmt {
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public Registers Run(Registers reg, Span<long> frame, WasmInstance inst) {
        reg.R3 = (uint)default(VALUE).Run(reg, frame, inst); return default(NEXT).Run(reg, frame, inst);
    }
}
struct SetR4_I32<VALUE,NEXT> : Stmt where VALUE: struct, Expr<int> where NEXT: struct, Stmt {
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public Registers Run(Registers reg, Span<long> frame, WasmInstance inst) {
        reg.R4 = (uint)default(VALUE).Run(reg, frame, inst); return default(NEXT).Run(reg, frame, inst);
    }
}
struct SetR5_I32<VALUE,NEXT> : Stmt where VALUE: struct, Expr<int> where NEXT: struct, Stmt {
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public Registers Run(Registers reg, Span<long> frame, WasmInstance inst) {
        reg.R5 = (uint)default(VALUE).Run(reg, frame, inst); return default(NEXT).Run(reg, frame, inst);
    }
}
struct SetR6_I32<VALUE,NEXT> : Stmt where VALUE: struct, Expr<int> where NEXT: struct, Stmt {
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public Registers Run(Registers reg, Span<long> frame, WasmInstance inst) {
        reg.R6 = (uint)default(VALUE).Run(reg, frame, inst); return default(NEXT).Run(reg, frame, inst);
    }
}

struct SetR0_I64<VALUE,NEXT> : Stmt where VALUE: struct, Expr<long> where NEXT: struct, Stmt {
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public Registers Run(Registers reg, Span<long> frame, WasmInstance inst) {
        reg.R0 = default(VALUE).Run(reg, frame, inst); return default(NEXT).Run(reg, frame, inst);
    }
}
struct SetR1_I64<VALUE,NEXT> : Stmt where VALUE: struct, Expr<long> where NEXT: struct, Stmt {
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public Registers Run(Registers reg, Span<long> frame, WasmInstance inst) {
        reg.R1 = default(VALUE).Run(reg, frame, inst); return default(NEXT).Run(reg, frame, inst);
    }
}
struct SetR2_I64<VALUE,NEXT> : Stmt where VALUE: struct, Expr<long> where NEXT: struct, Stmt {
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public Registers Run(Registers reg, Span<long> frame, WasmInstance inst) {
        reg.R2 = default(VALUE).Run(reg, frame, inst); return default(NEXT).Run(reg, frame, inst);
    }
}
struct SetR3_I64<VALUE,NEXT> : Stmt where VALUE: struct, Expr<long> where NEXT: struct, Stmt {
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public Registers Run(Registers reg, Span<long> frame, WasmInstance inst) {
        reg.R3 = default(VALUE).Run(reg, frame, inst); return default(NEXT).Run(reg, frame, inst);
    }
}
struct SetR4_I64<VALUE,NEXT> : Stmt where VALUE: struct, Expr<long> where NEXT: struct, Stmt {
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public Registers Run(Registers reg, Span<long> frame, WasmInstance inst) {
        reg.R4 = default(VALUE).Run(reg, frame, inst); return default(NEXT).Run(reg, frame, inst);
    }
}
struct SetR5_I64<VALUE,NEXT> : Stmt where VALUE: struct, Expr<long> where NEXT: struct, Stmt {
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public Registers Run(Registers reg, Span<long> frame, WasmInstance inst) {
        reg.R5 = default(VALUE).Run(reg, frame, inst); return default(NEXT).Run(reg, frame, inst);
    }
}
struct SetR6_I64<VALUE,NEXT> : Stmt where VALUE: struct, Expr<long> where NEXT: struct, Stmt {
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public Registers Run(Registers reg, Span<long> frame, WasmInstance inst) {
        reg.R6 = default(VALUE).Run(reg, frame, inst); return default(NEXT).Run(reg, frame, inst);
    }
}

struct SetR0_F32<VALUE,NEXT> : Stmt where VALUE: struct, Expr<float> where NEXT: struct, Stmt {
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public Registers Run(Registers reg, Span<long> frame, WasmInstance inst) {
        reg.R0 = BitConverter.SingleToUInt32Bits(default(VALUE).Run(reg, frame, inst)); return default(NEXT).Run(reg, frame, inst);
    }
}
struct SetR1_F32<VALUE,NEXT> : Stmt where VALUE: struct, Expr<float> where NEXT: struct, Stmt {
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public Registers Run(Registers reg, Span<long> frame, WasmInstance inst) {
        reg.R1 = BitConverter.SingleToUInt32Bits(default(VALUE).Run(reg, frame, inst)); return default(NEXT).Run(reg, frame, inst);
    }
}
struct SetR2_F32<VALUE,NEXT> : Stmt where VALUE: struct, Expr<float> where NEXT: struct, Stmt {
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public Registers Run(Registers reg, Span<long> frame, WasmInstance inst) {
        reg.R2 = BitConverter.SingleToUInt32Bits(default(VALUE).Run(reg, frame, inst)); return default(NEXT).Run(reg, frame, inst);
    }
}
struct SetR3_F32<VALUE,NEXT> : Stmt where VALUE: struct, Expr<float> where NEXT: struct, Stmt {
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public Registers Run(Registers reg, Span<long> frame, WasmInstance inst) {
        reg.R3 = BitConverter.SingleToUInt32Bits(default(VALUE).Run(reg, frame, inst)); return default(NEXT).Run(reg, frame, inst);
    }
}
struct SetR4_F32<VALUE,NEXT> : Stmt where VALUE: struct, Expr<float> where NEXT: struct, Stmt {
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public Registers Run(Registers reg, Span<long> frame, WasmInstance inst) {
        reg.R4 = BitConverter.SingleToUInt32Bits(default(VALUE).Run(reg, frame, inst)); return default(NEXT).Run(reg, frame, inst);
    }
}
struct SetR5_F32<VALUE,NEXT> : Stmt where VALUE: struct, Expr<float> where NEXT: struct, Stmt {
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public Registers Run(Registers reg, Span<long> frame, WasmInstance inst) {
        reg.R5 = BitConverter.SingleToUInt32Bits(default(VALUE).Run(reg, frame, inst)); return default(NEXT).Run(reg, frame, inst);
    }
}
struct SetR6_F32<VALUE,NEXT> : Stmt where VALUE: struct, Expr<float> where NEXT: struct, Stmt {
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public Registers Run(Registers reg, Span<long> frame, WasmInstance inst) {
        reg.R6 = BitConverter.SingleToUInt32Bits(default(VALUE).Run(reg, frame, inst)); return default(NEXT).Run(reg, frame, inst);
    }
}

struct SetR0_F64<VALUE,NEXT> : Stmt where VALUE: struct, Expr<double> where NEXT: struct, Stmt {
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public Registers Run(Registers reg, Span<long> frame, WasmInstance inst) {
        reg.R0 = BitConverter.DoubleToInt64Bits(default(VALUE).Run(reg, frame, inst)); return default(NEXT).Run(reg, frame, inst);
    }
}
struct SetR1_F64<VALUE,NEXT> : Stmt where VALUE: struct, Expr<double> where NEXT: struct, Stmt {
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public Registers Run(Registers reg, Span<long> frame, WasmInstance inst) {
        reg.R1 = BitConverter.DoubleToInt64Bits(default(VALUE).Run(reg, frame, inst)); return default(NEXT).Run(reg, frame, inst);
    }
}
struct SetR2_F64<VALUE,NEXT> : Stmt where VALUE: struct, Expr<double> where NEXT: struct, Stmt {
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public Registers Run(Registers reg, Span<long> frame, WasmInstance inst) {
        reg.R2 = BitConverter.DoubleToInt64Bits(default(VALUE).Run(reg, frame, inst)); return default(NEXT).Run(reg, frame, inst);
    }
}
struct SetR3_F64<VALUE,NEXT> : Stmt where VALUE: struct, Expr<double> where NEXT: struct, Stmt {
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public Registers Run(Registers reg, Span<long> frame, WasmInstance inst) {
        reg.R3 = BitConverter.DoubleToInt64Bits(default(VALUE).Run(reg, frame, inst)); return default(NEXT).Run(reg, frame, inst);
    }
}
struct SetR4_F64<VALUE,NEXT> : Stmt where VALUE: struct, Expr<double> where NEXT: struct, Stmt {
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public Registers Run(Registers reg, Span<long> frame, WasmInstance inst) {
        reg.R4 = BitConverter.DoubleToInt64Bits(default(VALUE).Run(reg, frame, inst)); return default(NEXT).Run(reg, frame, inst);
    }
}
struct SetR5_F64<VALUE,NEXT> : Stmt where VALUE: struct, Expr<double> where NEXT: struct, Stmt {
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public Registers Run(Registers reg, Span<long> frame, WasmInstance inst) {
        reg.R5 = BitConverter.DoubleToInt64Bits(default(VALUE).Run(reg, frame, inst)); return default(NEXT).Run(reg, frame, inst);
    }
}
struct SetR6_F64<VALUE,NEXT> : Stmt where VALUE: struct, Expr<double> where NEXT: struct, Stmt {
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public Registers Run(Registers reg, Span<long> frame, WasmInstance inst) {
        reg.R6 = BitConverter.DoubleToInt64Bits(default(VALUE).Run(reg, frame, inst)); return default(NEXT).Run(reg, frame, inst);
    }
}

// frame

struct GetFrame_I32<INDEX> : Expr<int> where INDEX: struct, Const
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public int Run(Registers reg, Span<long> frame, WasmInstance inst) =>
        (int)frame[(int)default(INDEX).Run()];
}

struct GetFrame_I64<INDEX> : Expr<long> where INDEX: struct, Const
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public long Run(Registers reg, Span<long> frame, WasmInstance inst) =>
        frame[(int)default(INDEX).Run()];
}

struct GetFrame_F32<INDEX> : Expr<float> where INDEX: struct, Const
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public float Run(Registers reg, Span<long> frame, WasmInstance inst) =>
        BitConverter.Int32BitsToSingle((int)frame[(int)default(INDEX).Run()]);
}

struct GetFrame_F64<INDEX> : Expr<double> where INDEX: struct, Const
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public double Run(Registers reg, Span<long> frame, WasmInstance inst) =>
        BitConverter.Int64BitsToDouble(frame[(int)default(INDEX).Run()]);
}

// setters

struct SetFrame_I32<INDEX,VALUE,NEXT> : Stmt where INDEX: struct, Const where VALUE: struct, Expr<int> where NEXT: struct, Stmt {
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public Registers Run(Registers reg, Span<long> frame, WasmInstance inst) {
        frame[(int)default(INDEX).Run()] = (uint)default(VALUE).Run(reg, frame, inst);
        return default(NEXT).Run(reg, frame, inst);
    }
}

struct SetFrame_I64<INDEX,VALUE,NEXT> : Stmt where INDEX: struct, Const where VALUE: struct, Expr<long> where NEXT: struct, Stmt {
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public Registers Run(Registers reg, Span<long> frame, WasmInstance inst) {
        frame[(int)default(INDEX).Run()] = default(VALUE).Run(reg, frame, inst);
        return default(NEXT).Run(reg, frame, inst);
    }
}

struct SetFrame_F32<INDEX,VALUE,NEXT> : Stmt where INDEX: struct, Const where VALUE: struct, Expr<float> where NEXT: struct, Stmt {
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public Registers Run(Registers reg, Span<long> frame, WasmInstance inst) {
        frame[(int)default(INDEX).Run()] = BitConverter.SingleToUInt32Bits(default(VALUE).Run(reg, frame, inst));
        return default(NEXT).Run(reg, frame, inst);
    }
}

struct SetFrame_F64<INDEX,VALUE,NEXT> : Stmt where INDEX: struct, Const where VALUE: struct, Expr<double> where NEXT: struct, Stmt {
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public Registers Run(Registers reg, Span<long> frame, WasmInstance inst) {
        frame[(int)default(INDEX).Run()] = BitConverter.DoubleToInt64Bits(default(VALUE).Run(reg, frame, inst));
        return default(NEXT).Run(reg, frame, inst);
    }
}
