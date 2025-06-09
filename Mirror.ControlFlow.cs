using System.Runtime.CompilerServices;

struct ExprIf<COND, THEN, ELSE> : Expr<int>
    where COND : struct, Expr<int>
    where THEN : struct, Stmt
    where ELSE : struct, Stmt
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public int Run(ref Registers reg, Span<long> frame, WasmInstance inst)
    {
        if (default(COND).Run(ref reg, frame, inst) != 0)
        {
            default(THEN).Run(ref reg, frame, inst);
        }
        else
        {
            default(ELSE).Run(ref reg, frame, inst);
        }
        return 0;
    }
}

struct ExprLoopTrue<COND, BODY> : Expr<int>
    where COND : struct, Expr<int>
    where BODY : struct, Stmt
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public int Run(ref Registers reg, Span<long> frame, WasmInstance inst)
    {
        do
        {
            default(BODY).Run(ref reg, frame, inst);
        } while (default(COND).Run(ref reg, frame, inst) != 0);
        return 0;
    }
}

struct ExprTrap : Expr<int>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public int Run(ref Registers reg, Span<long> frame, WasmInstance inst)
    {
        throw new Exception("trap");
    }
}
