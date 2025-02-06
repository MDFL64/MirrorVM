using System.Runtime.CompilerServices;

struct Op_F64_Equal<A,B> : Expr<int> where A: struct, Expr<double> where B: struct, Expr<double>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public int Run(Registers reg) => default(A).Run(reg) == default(B).Run(reg) ? 1 : 0;
}
struct Op_F64_NotEqual<A,B> : Expr<int> where A: struct, Expr<double> where B: struct, Expr<double>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public int Run(Registers reg) => default(A).Run(reg) != default(B).Run(reg) ? 1 : 0;
}
struct Op_F64_Less<A,B> : Expr<int> where A: struct, Expr<double> where B: struct, Expr<double>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public int Run(Registers reg) => default(A).Run(reg) < default(B).Run(reg) ? 1 : 0;
}
struct Op_F64_LessEqual<A,B> : Expr<int> where A: struct, Expr<double> where B: struct, Expr<double>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public int Run(Registers reg) => default(A).Run(reg) <= default(B).Run(reg) ? 1 : 0;
}
struct Op_F64_Greater<A,B> : Expr<int> where A: struct, Expr<double> where B: struct, Expr<double>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public int Run(Registers reg) => default(A).Run(reg) > default(B).Run(reg) ? 1 : 0;
}
struct Op_F64_GreaterEqual<A,B> : Expr<int> where A: struct, Expr<double> where B: struct, Expr<double>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public int Run(Registers reg) => default(A).Run(reg) >= default(B).Run(reg) ? 1 : 0;
}
// END COMPARISONS

struct Op_F64_Add<A,B> : Expr<double> where A: struct, Expr<double> where B: struct, Expr<double> {
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public double Run(Registers reg) => default(A).Run(reg) + default(B).Run(reg);
}
struct Op_F64_Sub<A,B> : Expr<double> where A: struct, Expr<double> where B: struct, Expr<double> {
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public double Run(Registers reg) => default(A).Run(reg) - default(B).Run(reg);
}
struct Op_F64_Mul<A,B> : Expr<double> where A: struct, Expr<double> where B: struct, Expr<double> {
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public double Run(Registers reg) => default(A).Run(reg) * default(B).Run(reg);
}
struct Op_F64_Div<A,B> : Expr<double> where A: struct, Expr<double> where B: struct, Expr<double> {
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public double Run(Registers reg) => default(A).Run(reg) / default(B).Run(reg);
}
struct Op_F64_Min<A,B> : Expr<double> where A: struct, Expr<double> where B: struct, Expr<double> {
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public double Run(Registers reg) {
        double res = Math.Min(default(A).Run(reg), default(B).Run(reg));
        // replace bad NaNs
        if (Double.IsNaN(res)) {
            return Double.NaN;
        }
        return res;
    }
}
struct Op_F64_Max<A,B> : Expr<double> where A: struct, Expr<double> where B: struct, Expr<double> {
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public double Run(Registers reg) {
        double res = Math.Max(default(A).Run(reg), default(B).Run(reg));
        // replace bad NaNs
        if (Double.IsNaN(res)) {
            return Double.NaN;
        }
        return res;
    }
}
struct Op_F64_CopySign<A,B> : Expr<double> where A: struct, Expr<double> where B: struct, Expr<double> {
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public double Run(Registers reg) {
        return Math.CopySign(default(A).Run(reg), default(B).Run(reg));
    }
}

// UNARY

struct Op_F64_Neg<A> : Expr<double> where A: struct, Expr<double> {
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public double Run(Registers reg) => -default(A).Run(reg);
}
struct Op_F64_Abs<A> : Expr<double> where A: struct, Expr<double> {
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public double Run(Registers reg) => Math.Abs(default(A).Run(reg));
}
struct Op_F64_Sqrt<A> : Expr<double> where A: struct, Expr<double> {
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public double Run(Registers reg) => Math.Sqrt(default(A).Run(reg));
}
struct Op_F64_Floor<A> : Expr<double> where A: struct, Expr<double> {
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public double Run(Registers reg) => Math.Floor(default(A).Run(reg));
}
struct Op_F64_Ceil<A> : Expr<double> where A: struct, Expr<double> {
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public double Run(Registers reg) => Math.Ceiling(default(A).Run(reg));
}
struct Op_F64_Truncate<A> : Expr<double> where A: struct, Expr<double> {
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public double Run(Registers reg) => Math.Truncate(default(A).Run(reg));
}
struct Op_F64_Nearest<A> : Expr<double> where A: struct, Expr<double> {
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public double Run(Registers reg) => Math.Round(default(A).Run(reg));
}
struct Op_F64_Convert_I32_S<A> : Expr<double> where A: struct, Expr<int> {
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public double Run(Registers reg) => default(A).Run(reg);
}
struct Op_F64_Convert_I32_U<A> : Expr<double> where A: struct, Expr<int> {
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public double Run(Registers reg) => (uint)default(A).Run(reg);
}
struct Op_F64_Convert_I64_S<A> : Expr<double> where A: struct, Expr<long> {
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public double Run(Registers reg) => default(A).Run(reg);
}
struct Op_F64_Convert_I64_U<A> : Expr<double> where A: struct, Expr<long> {
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public double Run(Registers reg) {
        var a = default(A).Run(reg);
        if (a < 0) {
            double f = (a>>>1)|(a&1);
            return f*2;
        } else {
            return a;
        }
    }
}
struct Op_F64_Promote_F32<A> : Expr<double> where A: struct, Expr<float> {
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public double Run(Registers reg) => default(A).Run(reg);
}
struct Op_F64_Reinterpret_I64<A> : Expr<double> where A: struct, Expr<long> {
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public double Run(Registers reg) => BitConverter.Int64BitsToDouble(default(A).Run(reg));
}
