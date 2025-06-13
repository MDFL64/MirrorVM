using System.Runtime.CompilerServices;

public struct Registers {
    public long R0;
    public long R1;
    public long R2;
    public long R3;
    public long R4;
    public long R5;
    public long R6;
    public int NextBlock;
}

interface Const {
    long Run();
}

interface Expr<T> {
    T Run(ref Registers reg, Span<long> frame, WasmInstance inst);
}

interface Stmt {
    void Run(ref Registers reg, Span<long> frame, WasmInstance inst);
}

interface Terminator {
    void Run(ref Registers reg, Span<long> frame, WasmInstance inst);
}

struct D0 : Const { public long Run() => 0; }
struct D1 : Const { public long Run() => 1; }
struct D2 : Const { public long Run() => 2; }
struct D3 : Const { public long Run() => 3; }
struct D4 : Const { public long Run() => 4; }
struct D5 : Const { public long Run() => 5; }
struct D6 : Const { public long Run() => 6; }
struct D7 : Const { public long Run() => 7; }
struct D8 : Const { public long Run() => 8; }
struct D9 : Const { public long Run() => 9; }
struct DA : Const { public long Run() => 0xA; }
struct DB : Const { public long Run() => 0xB; }
struct DC : Const { public long Run() => 0xC; }
struct DD : Const { public long Run() => 0xD; }
struct DE : Const { public long Run() => 0xE; }
struct DF : Const { public long Run() => 0xF; }

struct Num<A,B> : Const
    where A: struct, Const
    where B: struct, Const
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public long Run() {
        return default(A).Run()<<4 | default(B).Run();
    }
}

struct Num<A,B,C,D> : Const
    where A: struct, Const
    where B: struct, Const
    where C: struct, Const
    where D: struct, Const
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public long Run() {
        return default(A).Run()<<12 | default(B).Run()<<8 | default(C).Run()<<4 | default(D).Run();
    }
}

struct Num<A,B,C,D,E,F,G,H> : Const
    where A: struct, Const
    where B: struct, Const
    where C: struct, Const
    where D: struct, Const
    where E: struct, Const
    where F: struct, Const
    where G: struct, Const
    where H: struct, Const
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public long Run() {
        return default(A).Run()<<28 | default(B).Run()<<24 | default(C).Run()<<20 | default(D).Run()<<16 |
            default(E).Run()<<12 | default(F).Run()<<8 | default(G).Run()<<4 | default(H).Run();
    }
}

struct Num<A,B,C,D,E,F,G,H,I,J,K,L,M,N,O,P> : Const
    where A: struct, Const
    where B: struct, Const
    where C: struct, Const
    where D: struct, Const
    where E: struct, Const
    where F: struct, Const
    where G: struct, Const
    where H: struct, Const
    where I: struct, Const
    where J: struct, Const
    where K: struct, Const
    where L: struct, Const
    where M: struct, Const
    where N: struct, Const
    where O: struct, Const
    where P: struct, Const
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public long Run() {
        return 
            default(A).Run()<<60 | default(B).Run()<<56 | default(C).Run()<<52 | default(D).Run()<<48 |
            default(E).Run()<<44 | default(F).Run()<<40 | default(G).Run()<<36 | default(H).Run()<<32 |
            default(I).Run()<<28 | default(J).Run()<<24 | default(K).Run()<<20 | default(L).Run()<<16 |
            default(M).Run()<<12 | default(N).Run()<<8 | default(O).Run()<<4 | default(P).Run();
    }
}

struct Neg<A> : Const
    where A: struct, Const
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public long Run() {
        return -default(A).Run();
    }
}

struct Select<COND,A,B,T> : Expr<T>
    where COND: struct, Expr<int>
    where A: struct, Expr<T>
    where B: struct, Expr<T>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public T Run(ref Registers reg, Span<long> frame, WasmInstance inst) {
        if (default(COND).Run(ref reg, frame, inst) != 0) {
            return default(A).Run(ref reg, frame, inst);
        } else {
            return default(B).Run(ref reg, frame, inst);
        }
    }
}

struct ExprStmt<VALUE,T> : Stmt where VALUE: struct, Expr<T> {
    public void Run(ref Registers reg, Span<long> frame, WasmInstance inst) {
        default(VALUE).Run(ref reg, frame, inst);
    }
}

struct End: Stmt { public void Run(ref Registers reg, Span<long> frame, WasmInstance inst) {} }

struct TermJump<NEXT,BODY> : Terminator
    where NEXT: struct, Const
    where BODY: struct, Stmt
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public void Run(ref Registers reg, Span<long> frame, WasmInstance inst) {
        default(BODY).Run(ref reg, frame, inst);
        reg.NextBlock = (int)default(NEXT).Run();
    }
}

struct TermJumpIf<COND,TRUE,FALSE,BODY> : Terminator
    where COND: struct, Expr<int>
    where TRUE: struct, Const
    where FALSE: struct, Const
    where BODY: struct, Stmt
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public void Run(ref Registers reg, Span<long> frame, WasmInstance inst) {
        default(BODY).Run(ref reg, frame, inst);
        if (default(COND).Run(ref reg, frame, inst) != 0) {
            reg.NextBlock = (int)default(TRUE).Run();
        } else {
            reg.NextBlock = (int)default(FALSE).Run();
        }
    }
}

struct TermJumpTable<SEL,BASE,COUNT,BODY> : Terminator
    where SEL: struct, Expr<int>
    where BASE: struct, Const
    where COUNT: struct, Const
    where BODY: struct, Stmt

{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public void Run(ref Registers reg, Span<long> frame, WasmInstance inst) {
        default(BODY).Run(ref reg, frame, inst);
        uint sel = (uint)default(SEL).Run(ref reg, frame, inst);
        uint base_block = (uint)default(BASE).Run();
        uint max = (uint)default(COUNT).Run()-1;
        reg.NextBlock = (int)(base_block + uint.Min(sel,max));
    }
}

struct TermReturn<BODY> : Terminator
    where BODY: struct, Stmt
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public void Run(ref Registers reg, Span<long> frame, WasmInstance inst) {
        default(BODY).Run(ref reg, frame, inst);
        reg.NextBlock = -1;
    }
}

struct TermTrap : Terminator {
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public void Run(ref Registers reg, Span<long> frame, WasmInstance inst) => throw new Exception("trap");
}

struct TermVoid : Terminator {
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public void Run(ref Registers reg, Span<long> frame, WasmInstance inst) => throw new Exception("entered void block");
}

public interface ICallable {
    public long Call(Span<long> args, WasmInstance inst);
}

struct Function<BODY, SETUP, EXTRA_RET_COUNT, FRAME_SIZE> : ICallable
    where BODY: struct, Stmt
    where SETUP : struct, ArgRead
    where EXTRA_RET_COUNT : struct, Const
    where FRAME_SIZE : struct, Const
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public long Call(Span<long> args, WasmInstance inst)
    {
        int frame_size = (int)default(FRAME_SIZE).Run();
        Span<long> frame = stackalloc long[frame_size];
        Registers reg = default;
        default(SETUP).Run(args, ref reg, frame);

        default(BODY).Run(ref reg, frame, inst);

        long extra_ret_count = default(EXTRA_RET_COUNT).Run();
        for (int i = 0; i < extra_ret_count; i++)
        {
            args[i] = frame[i];
        }
        return reg.R0;
    }
}

struct Stmts1<A,B,C,D> : Stmt
    where A: struct, Stmt
    where B: struct, Stmt
    where C: struct, Stmt
    where D: struct, Stmt
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public void Run(ref Registers reg, Span<long> frame, WasmInstance inst)
    {
        default(A).Run(ref reg, frame, inst);
        default(B).Run(ref reg, frame, inst);
        default(C).Run(ref reg, frame, inst);
        default(D).Run(ref reg, frame, inst);
    }
}

struct Stmts2<A,B,C,D> : Stmt
    where A: struct, Stmt
    where B: struct, Stmt
    where C: struct, Stmt
    where D: struct, Stmt
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public void Run(ref Registers reg, Span<long> frame, WasmInstance inst)
    {
        default(A).Run(ref reg, frame, inst);
        default(B).Run(ref reg, frame, inst);
        default(C).Run(ref reg, frame, inst);
        default(D).Run(ref reg, frame, inst);
    }
}

struct Stmts3<A,B,C,D> : Stmt
    where A: struct, Stmt
    where B: struct, Stmt
    where C: struct, Stmt
    where D: struct, Stmt
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public void Run(ref Registers reg, Span<long> frame, WasmInstance inst)
    {
        default(A).Run(ref reg, frame, inst);
        default(B).Run(ref reg, frame, inst);
        default(C).Run(ref reg, frame, inst);
        default(D).Run(ref reg, frame, inst);
    }
}

struct Stmts4<A,B,C,D> : Stmt
    where A: struct, Stmt
    where B: struct, Stmt
    where C: struct, Stmt
    where D: struct, Stmt
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public void Run(ref Registers reg, Span<long> frame, WasmInstance inst)
    {
        default(A).Run(ref reg, frame, inst);
        default(B).Run(ref reg, frame, inst);
        default(C).Run(ref reg, frame, inst);
        default(D).Run(ref reg, frame, inst);
    }
}

struct Stmts5<A, B, C, D> : Stmt
    where A : struct, Stmt
    where B : struct, Stmt
    where C : struct, Stmt
    where D : struct, Stmt
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public void Run(ref Registers reg, Span<long> frame, WasmInstance inst)
    {
        default(A).Run(ref reg, frame, inst);
        default(B).Run(ref reg, frame, inst);
        default(C).Run(ref reg, frame, inst);
        default(D).Run(ref reg, frame, inst);
    }
}

struct Stmts6<A, B, C, D> : Stmt
    where A : struct, Stmt
    where B : struct, Stmt
    where C : struct, Stmt
    where D : struct, Stmt
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public void Run(ref Registers reg, Span<long> frame, WasmInstance inst)
    {
        default(A).Run(ref reg, frame, inst);
        default(B).Run(ref reg, frame, inst);
        default(C).Run(ref reg, frame, inst);
        default(D).Run(ref reg, frame, inst);
    }
}

struct Stmts7<A, B, C, D> : Stmt
    where A : struct, Stmt
    where B : struct, Stmt
    where C : struct, Stmt
    where D : struct, Stmt
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public void Run(ref Registers reg, Span<long> frame, WasmInstance inst)
    {
        default(A).Run(ref reg, frame, inst);
        default(B).Run(ref reg, frame, inst);
        default(C).Run(ref reg, frame, inst);
        default(D).Run(ref reg, frame, inst);
    }
}

struct Stmts8<A,B,C,D> : Stmt
    where A: struct, Stmt
    where B: struct, Stmt
    where C: struct, Stmt
    where D: struct, Stmt
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public void Run(ref Registers reg, Span<long> frame, WasmInstance inst)
    {
        default(A).Run(ref reg, frame, inst);
        default(B).Run(ref reg, frame, inst);
        default(C).Run(ref reg, frame, inst);
        default(D).Run(ref reg, frame, inst);
    }
}

struct NoInline<A> : Stmt
    where A : struct, Stmt
{
    [MethodImpl(MethodImplOptions.NoInlining)]
    public void Run(ref Registers reg, Span<long> frame, WasmInstance inst)
    {
        //Registers r2 = reg;
        default(A).Run(ref reg, frame, inst);
        //reg = r2;
    }
}
