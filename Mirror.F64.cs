using System.Runtime.CompilerServices;

struct Op_F64_Equal<A,B> : Expr<int> where A: struct, Expr<double> where B: struct, Expr<double>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public int Run(ref Registers reg, Span<long> frame, WasmInstance inst) => default(A).Run(ref reg, frame, inst) == default(B).Run(ref reg, frame, inst) ? 1 : 0;
}
struct Op_F64_NotEqual<A,B> : Expr<int> where A: struct, Expr<double> where B: struct, Expr<double>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public int Run(ref Registers reg, Span<long> frame, WasmInstance inst) => default(A).Run(ref reg, frame, inst) != default(B).Run(ref reg, frame, inst) ? 1 : 0;
}
struct Op_F64_Less<A,B> : Expr<int> where A: struct, Expr<double> where B: struct, Expr<double>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public int Run(ref Registers reg, Span<long> frame, WasmInstance inst) => default(A).Run(ref reg, frame, inst) < default(B).Run(ref reg, frame, inst) ? 1 : 0;
}
struct Op_F64_LessEqual<A,B> : Expr<int> where A: struct, Expr<double> where B: struct, Expr<double>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public int Run(ref Registers reg, Span<long> frame, WasmInstance inst) => default(A).Run(ref reg, frame, inst) <= default(B).Run(ref reg, frame, inst) ? 1 : 0;
}
struct Op_F64_Greater<A,B> : Expr<int> where A: struct, Expr<double> where B: struct, Expr<double>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public int Run(ref Registers reg, Span<long> frame, WasmInstance inst) => default(A).Run(ref reg, frame, inst) > default(B).Run(ref reg, frame, inst) ? 1 : 0;
}
struct Op_F64_GreaterEqual<A,B> : Expr<int> where A: struct, Expr<double> where B: struct, Expr<double>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public int Run(ref Registers reg, Span<long> frame, WasmInstance inst) => default(A).Run(ref reg, frame, inst) >= default(B).Run(ref reg, frame, inst) ? 1 : 0;
}
// END COMPARISONS

struct Const_F64<C> : Expr<double>
    where C: struct, Const
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public double Run(ref Registers reg, Span<long> frame, WasmInstance inst) => BitConverter.Int64BitsToDouble(default(C).Run());
}
struct Op_F64_Add<A,B> : Expr<double> where A: struct, Expr<double> where B: struct, Expr<double> {
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public double Run(ref Registers reg, Span<long> frame, WasmInstance inst) => FloatHelper.CanonicalF64( default(A).Run(ref reg, frame, inst) + default(B).Run(ref reg, frame, inst) );
}
struct Op_F64_Sub<A,B> : Expr<double> where A: struct, Expr<double> where B: struct, Expr<double> {
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public double Run(ref Registers reg, Span<long> frame, WasmInstance inst) => FloatHelper.CanonicalF64( default(A).Run(ref reg, frame, inst) - default(B).Run(ref reg, frame, inst) );
}
struct Op_F64_Mul<A,B> : Expr<double> where A: struct, Expr<double> where B: struct, Expr<double> {
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public double Run(ref Registers reg, Span<long> frame, WasmInstance inst) => FloatHelper.CanonicalF64( default(A).Run(ref reg, frame, inst) * default(B).Run(ref reg, frame, inst) );
}
struct Op_F64_Div<A,B> : Expr<double> where A: struct, Expr<double> where B: struct, Expr<double> {
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public double Run(ref Registers reg, Span<long> frame, WasmInstance inst) => FloatHelper.CanonicalF64( default(A).Run(ref reg, frame, inst) / default(B).Run(ref reg, frame, inst) );
}
struct Op_F64_Min<A,B> : Expr<double> where A: struct, Expr<double> where B: struct, Expr<double> {
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public double Run(ref Registers reg, Span<long> frame, WasmInstance inst) => FloatHelper.CanonicalF64( Math.Min(default(A).Run(ref reg, frame, inst), default(B).Run(ref reg, frame, inst)) );
}
struct Op_F64_Max<A,B> : Expr<double> where A: struct, Expr<double> where B: struct, Expr<double> {
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public double Run(ref Registers reg, Span<long> frame, WasmInstance inst) => FloatHelper.CanonicalF64( Math.Max(default(A).Run(ref reg, frame, inst), default(B).Run(ref reg, frame, inst)) );
}
struct Op_F64_CopySign<A,B> : Expr<double> where A: struct, Expr<double> where B: struct, Expr<double> {
    // canonicalization not required by spec
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public double Run(ref Registers reg, Span<long> frame, WasmInstance inst) => Math.CopySign(default(A).Run(ref reg, frame, inst), default(B).Run(ref reg, frame, inst));
}

// UNARY

struct Op_F64_Neg<A> : Expr<double> where A: struct, Expr<double> {
    // canonicalization not required by spec
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public double Run(ref Registers reg, Span<long> frame, WasmInstance inst) => -default(A).Run(ref reg, frame, inst);
}
struct Op_F64_Abs<A> : Expr<double> where A: struct, Expr<double> {
    // canonicalization not required by spec
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public double Run(ref Registers reg, Span<long> frame, WasmInstance inst) => Math.Abs(default(A).Run(ref reg, frame, inst));
}
struct Op_F64_Sqrt<A> : Expr<double> where A: struct, Expr<double> {
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public double Run(ref Registers reg, Span<long> frame, WasmInstance inst) => FloatHelper.CanonicalF64( Math.Sqrt(default(A).Run(ref reg, frame, inst)) );
}
struct Op_F64_Floor<A> : Expr<double> where A: struct, Expr<double> {
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public double Run(ref Registers reg, Span<long> frame, WasmInstance inst) => FloatHelper.CanonicalF64( Math.Floor(default(A).Run(ref reg, frame, inst)) );
}
struct Op_F64_Ceil<A> : Expr<double> where A: struct, Expr<double> {
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public double Run(ref Registers reg, Span<long> frame, WasmInstance inst) => FloatHelper.CanonicalF64( Math.Ceiling(default(A).Run(ref reg, frame, inst)) );
}
struct Op_F64_Truncate<A> : Expr<double> where A: struct, Expr<double> {
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public double Run(ref Registers reg, Span<long> frame, WasmInstance inst) => FloatHelper.CanonicalF64( Math.Truncate(default(A).Run(ref reg, frame, inst)) );
}
struct Op_F64_Nearest<A> : Expr<double> where A: struct, Expr<double> {
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public double Run(ref Registers reg, Span<long> frame, WasmInstance inst) => FloatHelper.CanonicalF64( Math.Round(default(A).Run(ref reg, frame, inst)) );
}

// conversions, probably don't need canonicalized?
struct Op_F64_Convert_I32_S<A> : Expr<double> where A: struct, Expr<int> {
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public double Run(ref Registers reg, Span<long> frame, WasmInstance inst) => default(A).Run(ref reg, frame, inst);
}
struct Op_F64_Convert_I32_U<A> : Expr<double> where A: struct, Expr<int> {
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public double Run(ref Registers reg, Span<long> frame, WasmInstance inst) => (uint)default(A).Run(ref reg, frame, inst);
}
struct Op_F64_Convert_I64_S<A> : Expr<double> where A: struct, Expr<long> {
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public double Run(ref Registers reg, Span<long> frame, WasmInstance inst) => default(A).Run(ref reg, frame, inst);
}
struct Op_F64_Convert_I64_U<A> : Expr<double> where A: struct, Expr<long> {
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public double Run(ref Registers reg, Span<long> frame, WasmInstance inst) {
        var a = default(A).Run(ref reg, frame, inst);
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
    public double Run(ref Registers reg, Span<long> frame, WasmInstance inst) => default(A).Run(ref reg, frame, inst);
}
struct Op_F64_Reinterpret_I64<A> : Expr<double> where A: struct, Expr<long> {
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public double Run(ref Registers reg, Span<long> frame, WasmInstance inst) => BitConverter.Int64BitsToDouble(default(A).Run(ref reg, frame, inst));
}
