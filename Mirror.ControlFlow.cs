using System.Runtime.CompilerServices;

struct StmtIf<COND, THEN, ELSE> : Stmt
    where COND : struct, Expr<int>
    where THEN : struct, Stmt
    where ELSE : struct, Stmt
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public void Run(ref Registers reg, Span<long> frame, WasmInstance inst)
    {
        if (default(COND).Run(ref reg, frame, inst) != 0)
        {
            default(THEN).Run(ref reg, frame, inst);
        }
        else
        {
            default(ELSE).Run(ref reg, frame, inst);
        }
    }
}

struct StmtLoopTrue<COND, BODY> : Stmt
    where COND : struct, Expr<int>
    where BODY : struct, Stmt
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public void Run(ref Registers reg, Span<long> frame, WasmInstance inst)
    {
        //Registers reg = reg_ref;
        do
        {
            default(BODY).Run(ref reg, frame, inst);
        } while (default(COND).Run(ref reg, frame, inst) != 0);
        //reg_ref = reg;
    }
}

struct StmtTrap : Stmt
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public void Run(ref Registers reg, Span<long> frame, WasmInstance inst)
    {
        throw new Exception("trap");
    }
}
