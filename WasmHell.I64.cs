using System.Numerics;
using System.Runtime.CompilerServices;

struct Op_I64_EqualZero<A> : Expr<int> where A: struct, Expr<long>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public int Run(Registers reg) => default(A).Run(reg) == 0 ? 1 : 0;
}
struct Op_I64_Equal<A,B> : Expr<int> where A: struct, Expr<long> where B: struct, Expr<long>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public int Run(Registers reg) => default(A).Run(reg) == default(B).Run(reg) ? 1 : 0;
}
struct Op_I64_NotEqual<A,B> : Expr<int> where A: struct, Expr<long> where B: struct, Expr<long>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public int Run(Registers reg) => default(A).Run(reg) != default(B).Run(reg) ? 1 : 0;
}
struct Op_I64_GreaterEqual_S<A,B> : Expr<int> where A: struct, Expr<long> where B: struct, Expr<long>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public int Run(Registers reg) => default(A).Run(reg) >= default(B).Run(reg) ? 1 : 0;
}
struct Op_I64_Greater_S<A,B> : Expr<int> where A: struct, Expr<long> where B: struct, Expr<long>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public int Run(Registers reg) => default(A).Run(reg) > default(B).Run(reg) ? 1 : 0;
}
struct Op_I64_LessEqual_S<A,B> : Expr<int> where A: struct, Expr<long> where B: struct, Expr<long>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public int Run(Registers reg) => default(A).Run(reg) <= default(B).Run(reg) ? 1 : 0;
}
struct Op_I64_Less_S<A,B> : Expr<int> where A: struct, Expr<long> where B: struct, Expr<long>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public int Run(Registers reg) => default(A).Run(reg) < default(B).Run(reg) ? 1 : 0;
}
struct Op_I64_GreaterEqual_U<A,B> : Expr<int> where A: struct, Expr<long> where B: struct, Expr<long>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public int Run(Registers reg) => (ulong)default(A).Run(reg) >= (ulong)default(B).Run(reg) ? 1 : 0;
}
struct Op_I64_Greater_U<A,B> : Expr<int> where A: struct, Expr<long> where B: struct, Expr<long>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public int Run(Registers reg) => (ulong)default(A).Run(reg) > (ulong)default(B).Run(reg) ? 1 : 0;
}
struct Op_I64_LessEqual_U<A,B> : Expr<int> where A: struct, Expr<long> where B: struct, Expr<long>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public int Run(Registers reg) => (ulong)default(A).Run(reg) <= (ulong)default(B).Run(reg) ? 1 : 0;
}
struct Op_I64_Less_U<A,B> : Expr<int> where A: struct, Expr<long> where B: struct, Expr<long>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public int Run(Registers reg) => (ulong)default(A).Run(reg) < (ulong)default(B).Run(reg) ? 1 : 0;
}
// END COMPARISONS

struct Const_I64<C> : Expr<long>
    where C: struct, Const
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public long Run(Registers reg) => (int)default(C).Run();
}
struct Op_I64_Add<A,B> : Expr<long> where A: struct, Expr<long> where B: struct, Expr<long> {
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public long Run(Registers reg) => default(A).Run(reg) + default(B).Run(reg);
}
struct Op_I64_Sub<A,B> : Expr<long> where A: struct, Expr<long> where B: struct, Expr<long> {
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public long Run(Registers reg) => default(A).Run(reg) - default(B).Run(reg);
}
struct Op_I64_Mul<A,B> : Expr<long> where A: struct, Expr<long> where B: struct, Expr<long> {
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public long Run(Registers reg) => default(A).Run(reg) * default(B).Run(reg);
}
struct Op_I64_Div_S<A,B> : Expr<long> where A: struct, Expr<long> where B: struct, Expr<long> {
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public long Run(Registers reg) => default(A).Run(reg) / default(B).Run(reg);
}
struct Op_I64_Div_U<A,B> : Expr<long> where A: struct, Expr<long> where B: struct, Expr<long> {
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public long Run(Registers reg) => (long)((ulong)default(A).Run(reg) / (ulong)default(B).Run(reg));
}
struct Op_I64_Rem_S<A,B> : Expr<long> where A: struct, Expr<long> where B: struct, Expr<long> {
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public long Run(Registers reg) {
        long a = default(A).Run(reg);
        long b = default(B).Run(reg);
        // I tried to find a better way -- casting to longs is slower
        if (a == -9223372036854775808 && b == -1) {
            return 0;
        } else {
            return a % b;
        }
    }
}
struct Op_I64_Rem_U<A,B> : Expr<long> where A: struct, Expr<long> where B: struct, Expr<long> {
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public long Run(Registers reg) => (long)((ulong)default(A).Run(reg) % (ulong)default(B).Run(reg));
}
struct Op_I64_And<A,B> : Expr<long> where A: struct, Expr<long> where B: struct, Expr<long> {
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public long Run(Registers reg) => default(A).Run(reg) & default(B).Run(reg);
}
struct Op_I64_Or<A,B> : Expr<long> where A: struct, Expr<long> where B: struct, Expr<long> {
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public long Run(Registers reg) => default(A).Run(reg) | default(B).Run(reg);
}
struct Op_I64_Xor<A,B> : Expr<long> where A: struct, Expr<long> where B: struct, Expr<long> {
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public long Run(Registers reg) => default(A).Run(reg) ^ default(B).Run(reg);
}

struct Op_I64_ShiftLeft<A,B> : Expr<long> where A: struct, Expr<long> where B: struct, Expr<long> {
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public long Run(Registers reg) => default(A).Run(reg) << (int)default(B).Run(reg);
}
struct Op_I64_ShiftRight_S<A,B> : Expr<long> where A: struct, Expr<long> where B: struct, Expr<long> {
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public long Run(Registers reg) => default(A).Run(reg) >> (int)default(B).Run(reg);
}
struct Op_I64_ShiftRight_U<A,B> : Expr<long> where A: struct, Expr<long> where B: struct, Expr<long> {
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public long Run(Registers reg) => default(A).Run(reg) >>> (int)default(B).Run(reg);
}
struct Op_I64_RotateLeft<A,B> : Expr<long> where A: struct, Expr<long> where B: struct, Expr<long> {
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public long Run(Registers reg) => (long)BitOperations.RotateLeft((ulong)default(A).Run(reg),(int)default(B).Run(reg));
}
struct Op_I64_RotateRight<A,B> : Expr<long> where A: struct, Expr<long> where B: struct, Expr<long> {
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public long Run(Registers reg) => (long)BitOperations.RotateRight((ulong)default(A).Run(reg),(int)default(B).Run(reg));
}
struct Op_I64_LeadingZeros<A> : Expr<long> where A: struct, Expr<long>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public long Run(Registers reg) => BitOperations.LeadingZeroCount((ulong)default(A).Run(reg));
}
struct Op_I64_TrailingZeros<A> : Expr<long> where A: struct, Expr<long>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public long Run(Registers reg) => BitOperations.TrailingZeroCount((ulong)default(A).Run(reg));
}
struct Op_I64_PopCount<A> : Expr<long> where A: struct, Expr<long>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public long Run(Registers reg) => BitOperations.PopCount((ulong)default(A).Run(reg));
}
struct Op_I64_Extend8_S<A> : Expr<long> where A: struct, Expr<long>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public long Run(Registers reg) => (sbyte)default(A).Run(reg);
}
struct Op_I64_Extend16_S<A> : Expr<long> where A: struct, Expr<long>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public long Run(Registers reg) => (short)default(A).Run(reg);
}
struct Op_I64_Extend32_S<A> : Expr<long> where A: struct, Expr<long>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public long Run(Registers reg) => (int)default(A).Run(reg);
}
