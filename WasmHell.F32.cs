using System.Runtime.CompilerServices;

struct Op_F32_Equal<A,B> : Expr<int> where A: struct, Expr<float> where B: struct, Expr<float>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public int Run(Registers reg, Span<long> frame, WasmInstance inst) => default(A).Run(reg, frame, inst) == default(B).Run(reg, frame, inst) ? 1 : 0;
}
struct Op_F32_NotEqual<A,B> : Expr<int> where A: struct, Expr<float> where B: struct, Expr<float>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public int Run(Registers reg, Span<long> frame, WasmInstance inst) => default(A).Run(reg, frame, inst) != default(B).Run(reg, frame, inst) ? 1 : 0;
}
struct Op_F32_Less<A,B> : Expr<int> where A: struct, Expr<float> where B: struct, Expr<float>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public int Run(Registers reg, Span<long> frame, WasmInstance inst) => default(A).Run(reg, frame, inst) < default(B).Run(reg, frame, inst) ? 1 : 0;
}
struct Op_F32_LessEqual<A,B> : Expr<int> where A: struct, Expr<float> where B: struct, Expr<float>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public int Run(Registers reg, Span<long> frame, WasmInstance inst) => default(A).Run(reg, frame, inst) <= default(B).Run(reg, frame, inst) ? 1 : 0;
}
struct Op_F32_Greater<A,B> : Expr<int> where A: struct, Expr<float> where B: struct, Expr<float>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public int Run(Registers reg, Span<long> frame, WasmInstance inst) => default(A).Run(reg, frame, inst) > default(B).Run(reg, frame, inst) ? 1 : 0;
}
struct Op_F32_GreaterEqual<A,B> : Expr<int> where A: struct, Expr<float> where B: struct, Expr<float>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public int Run(Registers reg, Span<long> frame, WasmInstance inst) => default(A).Run(reg, frame, inst) >= default(B).Run(reg, frame, inst) ? 1 : 0;
}
// END COMPARISONS

struct Const_F32<C> : Expr<float>
    where C: struct, Const
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public float Run(Registers reg, Span<long> frame, WasmInstance inst) => BitConverter.UInt32BitsToSingle((uint)default(C).Run());
}
struct Op_F32_Add<A,B> : Expr<float> where A: struct, Expr<float> where B: struct, Expr<float> {
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public float Run(Registers reg, Span<long> frame, WasmInstance inst) => FloatHelper.CanonicalF32( default(A).Run(reg, frame, inst) + default(B).Run(reg, frame, inst) );
}
struct Op_F32_Sub<A,B> : Expr<float> where A: struct, Expr<float> where B: struct, Expr<float> {
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public float Run(Registers reg, Span<long> frame, WasmInstance inst) => FloatHelper.CanonicalF32( default(A).Run(reg, frame, inst) - default(B).Run(reg, frame, inst) );
}
struct Op_F32_Mul<A,B> : Expr<float> where A: struct, Expr<float> where B: struct, Expr<float> {
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public float Run(Registers reg, Span<long> frame, WasmInstance inst) => FloatHelper.CanonicalF32( default(A).Run(reg, frame, inst) * default(B).Run(reg, frame, inst) );
}
struct Op_F32_Div<A,B> : Expr<float> where A: struct, Expr<float> where B: struct, Expr<float> {
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public float Run(Registers reg, Span<long> frame, WasmInstance inst) => FloatHelper.CanonicalF32( default(A).Run(reg, frame, inst) / default(B).Run(reg, frame, inst) );
}
struct Op_F32_Min<A,B> : Expr<float> where A: struct, Expr<float> where B: struct, Expr<float> {
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public float Run(Registers reg, Span<long> frame, WasmInstance inst) => FloatHelper.CanonicalF32( MathF.Min(default(A).Run(reg, frame, inst), default(B).Run(reg, frame, inst)) );
}
struct Op_F32_Max<A,B> : Expr<float> where A: struct, Expr<float> where B: struct, Expr<float> {
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public float Run(Registers reg, Span<long> frame, WasmInstance inst) => FloatHelper.CanonicalF32( MathF.Max(default(A).Run(reg, frame, inst), default(B).Run(reg, frame, inst)) );
}
struct Op_F32_CopySign<A,B> : Expr<float> where A: struct, Expr<float> where B: struct, Expr<float> {
    // canonicalization not required by spec
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public float Run(Registers reg, Span<long> frame, WasmInstance inst) => MathF.CopySign(default(A).Run(reg, frame, inst), default(B).Run(reg, frame, inst));
}

// UNARY

struct Op_F32_Neg<A> : Expr<float> where A: struct, Expr<float> {
    // canonicalization not required by spec
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public float Run(Registers reg, Span<long> frame, WasmInstance inst) => -default(A).Run(reg, frame, inst);
}
struct Op_F32_Abs<A> : Expr<float> where A: struct, Expr<float> {
    // canonicalization not required by spec
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public float Run(Registers reg, Span<long> frame, WasmInstance inst) => MathF.Abs(default(A).Run(reg, frame, inst));
}
struct Op_F32_Sqrt<A> : Expr<float> where A: struct, Expr<float> {
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public float Run(Registers reg, Span<long> frame, WasmInstance inst) => FloatHelper.CanonicalF32( MathF.Sqrt(default(A).Run(reg, frame, inst)) );
}
struct Op_F32_Floor<A> : Expr<float> where A: struct, Expr<float> {
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public float Run(Registers reg, Span<long> frame, WasmInstance inst) => FloatHelper.CanonicalF32(  MathF.Floor(default(A).Run(reg, frame, inst)) );
}
struct Op_F32_Ceil<A> : Expr<float> where A: struct, Expr<float> {
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public float Run(Registers reg, Span<long> frame, WasmInstance inst) => FloatHelper.CanonicalF32( MathF.Ceiling(default(A).Run(reg, frame, inst)) );
}
struct Op_F32_Truncate<A> : Expr<float> where A: struct, Expr<float> {
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public float Run(Registers reg, Span<long> frame, WasmInstance inst) => FloatHelper.CanonicalF32( MathF.Truncate(default(A).Run(reg, frame, inst)) );
}
struct Op_F32_Nearest<A> : Expr<float> where A: struct, Expr<float> {
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public float Run(Registers reg, Span<long> frame, WasmInstance inst) => FloatHelper.CanonicalF32( MathF.Round(default(A).Run(reg, frame, inst)) );
}

// conversions, probably don't need canonicalized?
struct Op_F32_Convert_I32_S<A> : Expr<float> where A: struct, Expr<int> {
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public float Run(Registers reg, Span<long> frame, WasmInstance inst) => default(A).Run(reg, frame, inst);
}
struct Op_F32_Convert_I32_U<A> : Expr<float> where A: struct, Expr<int> {
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public float Run(Registers reg, Span<long> frame, WasmInstance inst) => (uint)default(A).Run(reg, frame, inst);
}
struct Op_F32_Convert_I64_S<A> : Expr<float> where A: struct, Expr<long> {
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public float Run(Registers reg, Span<long> frame, WasmInstance inst) => default(A).Run(reg, frame, inst);
}
struct Op_F32_Convert_I64_U<A> : Expr<float> where A: struct, Expr<long> {
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public float Run(Registers reg, Span<long> frame, WasmInstance inst) {
        var a = default(A).Run(reg, frame, inst);
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
    public float Run(Registers reg, Span<long> frame, WasmInstance inst) => (float)default(A).Run(reg, frame, inst);
}
struct Op_F32_Reinterpret_I32<A> : Expr<float> where A: struct, Expr<int> {
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public float Run(Registers reg, Span<long> frame, WasmInstance inst) => BitConverter.Int32BitsToSingle(default(A).Run(reg, frame, inst));
}
