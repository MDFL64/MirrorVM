using System.Linq.Expressions;
using System.Runtime.CompilerServices;

struct Registers {
    public long R0;
    public long R1;
    public long R2;
    public long R3;
    public long R4;
    public long R5;
    public long R6;
    public long R7;
    public int NextBlock;
}

interface Const {
    long Run();
}

interface Expr<T> {
    T Run(Registers reg);
}

interface Stmt {
    Registers Run(Registers reg);
}

interface Terminator {
    Registers Run(Registers reg);
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

struct Neg<A> : Const
    where A: struct, Const
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public long Run() {
        return -default(A).Run();
    }
}

struct GetR0_I32 : Expr<int> { public int Run(Registers reg) => (int)reg.R0; }
struct GetR1_I32 : Expr<int> { public int Run(Registers reg) => (int)reg.R1; }
struct GetR2_I32 : Expr<int> { public int Run(Registers reg) => (int)reg.R2; }
struct GetR3_I32 : Expr<int> { public int Run(Registers reg) => (int)reg.R3; }

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
struct Op_I32_Div_S<A,B> : Expr<int> where A: struct, Expr<int> where B: struct, Expr<int> {
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public int Run(Registers reg) => default(A).Run(reg) / default(B).Run(reg);
}
struct Op_I32_ShiftLeft<A,B> : Expr<int> where A: struct, Expr<int> where B: struct, Expr<int> {
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public int Run(Registers reg) => default(A).Run(reg) << default(B).Run(reg);
}

struct Op_I32_GreaterEqual_S<A,B> : Expr<int>
    where A: struct, Expr<int>
    where B: struct, Expr<int>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public int Run(Registers reg) {
        bool res = default(A).Run(reg) >= default(B).Run(reg);
        return res ? 1 : 0;
    }
}

struct End: Stmt { public Registers Run(Registers reg) => reg; }

struct SetR0_I32<VALUE,NEXT> : Stmt where VALUE: struct, Expr<int> where NEXT: struct, Stmt {
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public Registers Run(Registers reg) { reg.R0 = default(VALUE).Run(reg); return default(NEXT).Run(reg); }
}
struct SetR1_I32<VALUE,NEXT> : Stmt where VALUE: struct, Expr<int> where NEXT: struct, Stmt {
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public Registers Run(Registers reg) { reg.R1 = default(VALUE).Run(reg); return default(NEXT).Run(reg); }
}
struct SetR2_I32<VALUE,NEXT> : Stmt where VALUE: struct, Expr<int> where NEXT: struct, Stmt {
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public Registers Run(Registers reg) { reg.R2 = default(VALUE).Run(reg); return default(NEXT).Run(reg); }
}
struct SetR3_I32<VALUE,NEXT> : Stmt where VALUE: struct, Expr<int> where NEXT: struct, Stmt {
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public Registers Run(Registers reg) { reg.R3 = default(VALUE).Run(reg); return default(NEXT).Run(reg); }
}

struct TermVoid : Terminator {
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public Registers Run(Registers reg) => throw new Exception("entered void block");
}

struct TermJump<NEXT,BODY> : Terminator
    where NEXT: struct, Const
    where BODY: struct, Stmt
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public Registers Run(Registers old) {
        var reg = default(BODY).Run(old);
        reg.NextBlock = (int)default(NEXT).Run();
        return reg;
    }
}

struct TermJumpIf<COND,TRUE,FALSE,BODY> : Terminator
    where COND: struct, Expr<int>
    where TRUE: struct, Const
    where FALSE: struct, Const
    where BODY: struct, Stmt
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public Registers Run(Registers reg) {
        reg = default(BODY).Run(reg);
        if (default(COND).Run(reg) != 0) {
            reg.NextBlock = (int)default(TRUE).Run();
        } else {
            reg.NextBlock = (int)default(FALSE).Run();
        }
        return reg;
    }
}

struct TermReturn_I32<VALUE,BODY> : Terminator
    where VALUE: struct, Expr<int>
    where BODY: struct, Stmt
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public Registers Run(Registers reg) {
        reg.R0 = default(VALUE).Run(reg);
        reg.NextBlock = -1;
        return reg;
    }
}

interface IBody {
    public long Run(Registers reg);
}

struct Body<B0,B1,B2,B3,B4,B5,B6,B7,B8,B9> : IBody
    where B0: struct, Terminator
    where B1: struct, Terminator
    where B2: struct, Terminator
    where B3: struct, Terminator
    where B4: struct, Terminator
    where B5: struct, Terminator
    where B6: struct, Terminator
    where B7: struct, Terminator
    where B8: struct, Terminator
    where B9: struct, Terminator
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public long Run(Registers reg) {
        for (;;) {
            if (reg.NextBlock == 0)      reg = default(B0).Run(reg);
            else if (reg.NextBlock == 1) reg = default(B1).Run(reg);
            else if (reg.NextBlock == 2) reg = default(B2).Run(reg);
            else if (reg.NextBlock == 3) reg = default(B3).Run(reg);
            else if (reg.NextBlock == 4) reg = default(B4).Run(reg);
            else if (reg.NextBlock == 5) reg = default(B5).Run(reg);
            else if (reg.NextBlock == 6) reg = default(B6).Run(reg);
            else if (reg.NextBlock == 7) reg = default(B7).Run(reg);
            else if (reg.NextBlock == 8) reg = default(B8).Run(reg);
            else if (reg.NextBlock == 9) reg = default(B9).Run(reg);
            else return reg.R0;
        }
    }
}
