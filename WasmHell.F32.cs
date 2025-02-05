using System.Runtime.CompilerServices;

struct Op_F32_Add<A,B> : Expr<float> where A: struct, Expr<float> where B: struct, Expr<float> {
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public float Run(Registers reg) => default(A).Run(reg) + default(B).Run(reg);
}
struct Op_F32_Sub<A,B> : Expr<float> where A: struct, Expr<float> where B: struct, Expr<float> {
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public float Run(Registers reg) => default(A).Run(reg) - default(B).Run(reg);
}
struct Op_F32_Mul<A,B> : Expr<float> where A: struct, Expr<float> where B: struct, Expr<float> {
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public float Run(Registers reg) => default(A).Run(reg) * default(B).Run(reg);
}
struct Op_F32_Div<A,B> : Expr<float> where A: struct, Expr<float> where B: struct, Expr<float> {
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public float Run(Registers reg) => default(A).Run(reg) / default(B).Run(reg);
}

struct Op_F32_Min<A,B> : Expr<float> where A: struct, Expr<float> where B: struct, Expr<float> {
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public float Run(Registers reg) {
        float res = MathF.Min(default(A).Run(reg), default(B).Run(reg));
        // replace bad NaNs
        if (Single.IsNaN(res)) {
            return Single.NaN;
        }
        return res;
    }
}
struct Op_F32_Max<A,B> : Expr<float> where A: struct, Expr<float> where B: struct, Expr<float> {
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public float Run(Registers reg) {
        float res = MathF.Max(default(A).Run(reg), default(B).Run(reg));
        // replace bad NaNs
        if (Single.IsNaN(res)) {
            return Single.NaN;
        }
        return res;
    }
}

struct Op_F32_Sqrt<A> : Expr<float> where A: struct, Expr<float> {
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public float Run(Registers reg) => MathF.Sqrt(default(A).Run(reg));
}
struct Op_F32_Floor<A> : Expr<float> where A: struct, Expr<float> {
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public float Run(Registers reg) => MathF.Floor(default(A).Run(reg));
}
struct Op_F32_Ceil<A> : Expr<float> where A: struct, Expr<float> {
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public float Run(Registers reg) => MathF.Ceiling(default(A).Run(reg));
}
struct Op_F32_Truncate<A> : Expr<float> where A: struct, Expr<float> {
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public float Run(Registers reg) => MathF.Truncate(default(A).Run(reg));
}
struct Op_F32_Nearest<A> : Expr<float> where A: struct, Expr<float> {
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public float Run(Registers reg) => MathF.Round(default(A).Run(reg));
}
