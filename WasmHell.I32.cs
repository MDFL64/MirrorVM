using System.Numerics;
using System.Runtime.CompilerServices;

struct Op_I32_EqualZero<A> : Expr<int> where A: struct, Expr<int>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public int Run(Registers reg) => default(A).Run(reg) == 0 ? 1 : 0;
}
struct Op_I32_Equal<A,B> : Expr<int> where A: struct, Expr<int> where B: struct, Expr<int>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public int Run(Registers reg) => default(A).Run(reg) == default(B).Run(reg) ? 1 : 0;
}
struct Op_I32_NotEqual<A,B> : Expr<int> where A: struct, Expr<int> where B: struct, Expr<int>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public int Run(Registers reg) => default(A).Run(reg) != default(B).Run(reg) ? 1 : 0;
}
struct Op_I32_GreaterEqual_S<A,B> : Expr<int> where A: struct, Expr<int> where B: struct, Expr<int>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public int Run(Registers reg) => default(A).Run(reg) >= default(B).Run(reg) ? 1 : 0;
}
struct Op_I32_Greater_S<A,B> : Expr<int> where A: struct, Expr<int> where B: struct, Expr<int>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public int Run(Registers reg) => default(A).Run(reg) > default(B).Run(reg) ? 1 : 0;
}
struct Op_I32_LessEqual_S<A,B> : Expr<int> where A: struct, Expr<int> where B: struct, Expr<int>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public int Run(Registers reg) => default(A).Run(reg) <= default(B).Run(reg) ? 1 : 0;
}
struct Op_I32_Less_S<A,B> : Expr<int> where A: struct, Expr<int> where B: struct, Expr<int>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public int Run(Registers reg) => default(A).Run(reg) < default(B).Run(reg) ? 1 : 0;
}
struct Op_I32_GreaterEqual_U<A,B> : Expr<int> where A: struct, Expr<int> where B: struct, Expr<int>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public int Run(Registers reg) => (uint)default(A).Run(reg) >= (uint)default(B).Run(reg) ? 1 : 0;
}
struct Op_I32_Greater_U<A,B> : Expr<int> where A: struct, Expr<int> where B: struct, Expr<int>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public int Run(Registers reg) => (uint)default(A).Run(reg) > (uint)default(B).Run(reg) ? 1 : 0;
}
struct Op_I32_LessEqual_U<A,B> : Expr<int> where A: struct, Expr<int> where B: struct, Expr<int>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public int Run(Registers reg) => (uint)default(A).Run(reg) <= (uint)default(B).Run(reg) ? 1 : 0;
}
struct Op_I32_Less_U<A,B> : Expr<int> where A: struct, Expr<int> where B: struct, Expr<int>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public int Run(Registers reg) => (uint)default(A).Run(reg) < (uint)default(B).Run(reg) ? 1 : 0;
}
// END COMPARISONS

struct Const_I32<C> : Expr<int>
    where C: struct, Const
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public int Run(Registers reg) => (int)default(C).Run();
}
struct Op_I32_Add<A,B> : Expr<int> where A: struct, Expr<int> where B: struct, Expr<int> {
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public int Run(Registers reg) => default(A).Run(reg) + default(B).Run(reg);
}
struct Op_I32_Sub<A,B> : Expr<int> where A: struct, Expr<int> where B: struct, Expr<int> {
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public int Run(Registers reg) => default(A).Run(reg) - default(B).Run(reg);
}
struct Op_I32_Mul<A,B> : Expr<int> where A: struct, Expr<int> where B: struct, Expr<int> {
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public int Run(Registers reg) => default(A).Run(reg) * default(B).Run(reg);
}
struct Op_I32_Div_S<A,B> : Expr<int> where A: struct, Expr<int> where B: struct, Expr<int> {
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public int Run(Registers reg) => default(A).Run(reg) / default(B).Run(reg);
}
struct Op_I32_Div_U<A,B> : Expr<int> where A: struct, Expr<int> where B: struct, Expr<int> {
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public int Run(Registers reg) => (int)((uint)default(A).Run(reg) / (uint)default(B).Run(reg));
}
struct Op_I32_Rem_S<A,B> : Expr<int> where A: struct, Expr<int> where B: struct, Expr<int> {
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public int Run(Registers reg) {
        int a = default(A).Run(reg);
        int b = default(B).Run(reg);
        // I tried to find a better way -- casting to longs is slower
        if (a == -2147483648 && b == -1) {
            return 0;
        } else {
            return a % b;
        }
    }
}
struct Op_I32_Rem_U<A,B> : Expr<int> where A: struct, Expr<int> where B: struct, Expr<int> {
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public int Run(Registers reg) => (int)((uint)default(A).Run(reg) % (uint)default(B).Run(reg));
}
struct Op_I32_And<A,B> : Expr<int> where A: struct, Expr<int> where B: struct, Expr<int> {
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public int Run(Registers reg) => default(A).Run(reg) & default(B).Run(reg);
}
struct Op_I32_Or<A,B> : Expr<int> where A: struct, Expr<int> where B: struct, Expr<int> {
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public int Run(Registers reg) => default(A).Run(reg) | default(B).Run(reg);
}
struct Op_I32_Xor<A,B> : Expr<int> where A: struct, Expr<int> where B: struct, Expr<int> {
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public int Run(Registers reg) => default(A).Run(reg) ^ default(B).Run(reg);
}
struct Op_I32_ShiftLeft<A,B> : Expr<int> where A: struct, Expr<int> where B: struct, Expr<int> {
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public int Run(Registers reg) => default(A).Run(reg) << default(B).Run(reg);
}
struct Op_I32_ShiftRight_S<A,B> : Expr<int> where A: struct, Expr<int> where B: struct, Expr<int> {
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public int Run(Registers reg) => default(A).Run(reg) >> default(B).Run(reg);
}
struct Op_I32_ShiftRight_U<A,B> : Expr<int> where A: struct, Expr<int> where B: struct, Expr<int> {
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public int Run(Registers reg) => default(A).Run(reg) >>> default(B).Run(reg);
}
struct Op_I32_RotateLeft<A,B> : Expr<int> where A: struct, Expr<int> where B: struct, Expr<int> {
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public int Run(Registers reg) => (int)BitOperations.RotateLeft((uint)default(A).Run(reg),default(B).Run(reg));
}
struct Op_I32_RotateRight<A,B> : Expr<int> where A: struct, Expr<int> where B: struct, Expr<int> {
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public int Run(Registers reg) => (int)BitOperations.RotateRight((uint)default(A).Run(reg),default(B).Run(reg));
}
struct Op_I32_LeadingZeros<A> : Expr<int> where A: struct, Expr<int>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public int Run(Registers reg) => BitOperations.LeadingZeroCount((uint)default(A).Run(reg));
}
struct Op_I32_TrailingZeros<A> : Expr<int> where A: struct, Expr<int>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public int Run(Registers reg) => BitOperations.TrailingZeroCount((uint)default(A).Run(reg));
}
struct Op_I32_PopCount<A> : Expr<int> where A: struct, Expr<int>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public int Run(Registers reg) => BitOperations.PopCount((uint)default(A).Run(reg));
}
struct Op_I32_Extend8_S<A> : Expr<int> where A: struct, Expr<int>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public int Run(Registers reg) => (sbyte)default(A).Run(reg);
}
struct Op_I32_Extend16_S<A> : Expr<int> where A: struct, Expr<int>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public int Run(Registers reg) => (short)default(A).Run(reg);
}
