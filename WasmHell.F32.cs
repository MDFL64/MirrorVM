using System.Runtime.CompilerServices;

struct Op_F32_Equal<A,B> : Expr<int> where A: struct, Expr<float> where B: struct, Expr<float>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public int Run(Registers reg) => default(A).Run(reg) == default(B).Run(reg) ? 1 : 0;
}
struct Op_F32_NotEqual<A,B> : Expr<int> where A: struct, Expr<float> where B: struct, Expr<float>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public int Run(Registers reg) => default(A).Run(reg) != default(B).Run(reg) ? 1 : 0;
}
struct Op_F32_Less<A,B> : Expr<int> where A: struct, Expr<float> where B: struct, Expr<float>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public int Run(Registers reg) => default(A).Run(reg) < default(B).Run(reg) ? 1 : 0;
}
struct Op_F32_LessEqual<A,B> : Expr<int> where A: struct, Expr<float> where B: struct, Expr<float>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public int Run(Registers reg) => default(A).Run(reg) <= default(B).Run(reg) ? 1 : 0;
}
struct Op_F32_Greater<A,B> : Expr<int> where A: struct, Expr<float> where B: struct, Expr<float>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public int Run(Registers reg) => default(A).Run(reg) > default(B).Run(reg) ? 1 : 0;
}
struct Op_F32_GreaterEqual<A,B> : Expr<int> where A: struct, Expr<float> where B: struct, Expr<float>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public int Run(Registers reg) => default(A).Run(reg) >= default(B).Run(reg) ? 1 : 0;
}
// END COMPARISONS

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
struct Op_F32_CopySign<A,B> : Expr<float> where A: struct, Expr<float> where B: struct, Expr<float> {
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public float Run(Registers reg) {
        return MathF.CopySign(default(A).Run(reg), default(B).Run(reg));
    }
}

// UNARY

struct Op_F32_Neg<A> : Expr<float> where A: struct, Expr<float> {
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public float Run(Registers reg) => -default(A).Run(reg);
}
struct Op_F32_Abs<A> : Expr<float> where A: struct, Expr<float> {
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public float Run(Registers reg) => MathF.Abs(default(A).Run(reg));
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
struct Op_F32_Convert_I32_S<A> : Expr<float> where A: struct, Expr<int> {
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public float Run(Registers reg) => default(A).Run(reg);
}
struct Op_F32_Convert_I32_U<A> : Expr<float> where A: struct, Expr<int> {
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public float Run(Registers reg) => (uint)default(A).Run(reg);
}
struct Op_F32_Convert_I64_S<A> : Expr<float> where A: struct, Expr<long> {
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public float Run(Registers reg) => default(A).Run(reg);
}
struct Op_F32_Convert_I64_U<A> : Expr<float> where A: struct, Expr<long> {
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public float Run(Registers reg) {
        var a = default(A).Run(reg);
        if (a < 0) {
            float f = (a>>>1)|(a&1);
            return f*2;
        } else {
            return a;
        }
    }
}
struct Op_F32_Demote_F64<A> : Expr<float> where A: struct, Expr<double> {
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public float Run(Registers reg) => (float)default(A).Run(reg);
}
struct Op_F32_Reinterpret_I32<A> : Expr<float> where A: struct, Expr<int> {
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public float Run(Registers reg) => BitConverter.Int32BitsToSingle(default(A).Run(reg));
}
