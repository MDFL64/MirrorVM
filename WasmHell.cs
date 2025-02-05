using System.Runtime.CompilerServices;

public struct Registers {
    public long R0;
    public long R1;
    public long R2;
    public long R3;
    public long R4;
    public long R5;
    public long R6;
    public long R7;
    public int NextBlock;

    public void Set(int index, long value) {
        switch (index) {
            case 0: R0 = value; break;
            case 1: R1 = value; break;
            case 2: R2 = value; break;
            case 3: R3 = value; break;
            case 4: R4 = value; break;
            case 5: R5 = value; break;
            case 6: R6 = value; break;
            case 7: R7 = value; break;
        }
    }
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

struct GetR0_I32 : Expr<int> { public int Run(Registers reg) => (int)reg.R0; }
struct GetR1_I32 : Expr<int> { public int Run(Registers reg) => (int)reg.R1; }
struct GetR2_I32 : Expr<int> { public int Run(Registers reg) => (int)reg.R2; }
struct GetR3_I32 : Expr<int> { public int Run(Registers reg) => (int)reg.R3; }
struct GetR4_I32 : Expr<int> { public int Run(Registers reg) => (int)reg.R4; }
struct GetR5_I32 : Expr<int> { public int Run(Registers reg) => (int)reg.R5; }
struct GetR6_I32 : Expr<int> { public int Run(Registers reg) => (int)reg.R6; }
struct GetR7_I32 : Expr<int> { public int Run(Registers reg) => (int)reg.R7; }

struct GetR0_I64 : Expr<long> { public long Run(Registers reg) => reg.R0; }
struct GetR1_I64 : Expr<long> { public long Run(Registers reg) => reg.R1; }
struct GetR2_I64 : Expr<long> { public long Run(Registers reg) => reg.R2; }
struct GetR3_I64 : Expr<long> { public long Run(Registers reg) => reg.R3; }
struct GetR4_I64 : Expr<long> { public long Run(Registers reg) => reg.R4; }
struct GetR5_I64 : Expr<long> { public long Run(Registers reg) => reg.R5; }
struct GetR6_I64 : Expr<long> { public long Run(Registers reg) => reg.R6; }
struct GetR7_I64 : Expr<long> { public long Run(Registers reg) => reg.R7; }

struct GetR0_F32 : Expr<float> { public float Run(Registers reg) => BitConverter.Int32BitsToSingle((int)reg.R0); }
struct GetR1_F32 : Expr<float> { public float Run(Registers reg) => BitConverter.Int32BitsToSingle((int)reg.R1); }
struct GetR2_F32 : Expr<float> { public float Run(Registers reg) => BitConverter.Int32BitsToSingle((int)reg.R2); }
struct GetR3_F32 : Expr<float> { public float Run(Registers reg) => BitConverter.Int32BitsToSingle((int)reg.R3); }
struct GetR4_F32 : Expr<float> { public float Run(Registers reg) => BitConverter.Int32BitsToSingle((int)reg.R4); }
struct GetR5_F32 : Expr<float> { public float Run(Registers reg) => BitConverter.Int32BitsToSingle((int)reg.R5); }
struct GetR6_F32 : Expr<float> { public float Run(Registers reg) => BitConverter.Int32BitsToSingle((int)reg.R6); }
struct GetR7_F32 : Expr<float> { public float Run(Registers reg) => BitConverter.Int32BitsToSingle((int)reg.R7); }

struct GetR0_F64 : Expr<double> { public double Run(Registers reg) => BitConverter.Int64BitsToDouble(reg.R0); }
struct GetR1_F64 : Expr<double> { public double Run(Registers reg) => BitConverter.Int64BitsToDouble(reg.R1); }
struct GetR2_F64 : Expr<double> { public double Run(Registers reg) => BitConverter.Int64BitsToDouble(reg.R2); }
struct GetR3_F64 : Expr<double> { public double Run(Registers reg) => BitConverter.Int64BitsToDouble(reg.R3); }
struct GetR4_F64 : Expr<double> { public double Run(Registers reg) => BitConverter.Int64BitsToDouble(reg.R4); }
struct GetR5_F64 : Expr<double> { public double Run(Registers reg) => BitConverter.Int64BitsToDouble(reg.R5); }
struct GetR6_F64 : Expr<double> { public double Run(Registers reg) => BitConverter.Int64BitsToDouble(reg.R6); }
struct GetR7_F64 : Expr<double> { public double Run(Registers reg) => BitConverter.Int64BitsToDouble(reg.R7); }

struct Select_I32<COND,A,B> : Expr<int>
    where COND: struct, Expr<int>
    where A: struct, Expr<int>
    where B: struct, Expr<int>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public int Run(Registers reg) {
        if (default(COND).Run(reg) != 0) {
            return default(A).Run(reg);
        } else {
            return default(B).Run(reg);
        }
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
struct SetR4_I32<VALUE,NEXT> : Stmt where VALUE: struct, Expr<int> where NEXT: struct, Stmt {
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public Registers Run(Registers reg) { reg.R4 = default(VALUE).Run(reg); return default(NEXT).Run(reg); }
}
struct SetR5_I32<VALUE,NEXT> : Stmt where VALUE: struct, Expr<int> where NEXT: struct, Stmt {
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public Registers Run(Registers reg) { reg.R5 = default(VALUE).Run(reg); return default(NEXT).Run(reg); }
}
struct SetR6_I32<VALUE,NEXT> : Stmt where VALUE: struct, Expr<int> where NEXT: struct, Stmt {
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public Registers Run(Registers reg) { reg.R6 = default(VALUE).Run(reg); return default(NEXT).Run(reg); }
}
struct SetR7_I32<VALUE,NEXT> : Stmt where VALUE: struct, Expr<int> where NEXT: struct, Stmt {
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public Registers Run(Registers reg) { reg.R7 = default(VALUE).Run(reg); return default(NEXT).Run(reg); }
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
        reg.R0 = (uint)default(VALUE).Run(reg);
        reg.NextBlock = -1;
        return reg;
    }
}

struct TermReturn_I64<VALUE,BODY> : Terminator
    where VALUE: struct, Expr<long>
    where BODY: struct, Stmt
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public Registers Run(Registers reg) {
        reg.R0 = default(VALUE).Run(reg);
        reg.NextBlock = -1;
        return reg;
    }
}

struct TermReturn_F32<VALUE,BODY> : Terminator
    where VALUE: struct, Expr<float>
    where BODY: struct, Stmt
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public Registers Run(Registers reg) {
        reg.R0 = BitConverter.SingleToUInt32Bits(default(VALUE).Run(reg));
        reg.NextBlock = -1;
        return reg;
    }
}

struct TermReturn_F64<VALUE,BODY> : Terminator
    where VALUE: struct, Expr<double>
    where BODY: struct, Stmt
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public Registers Run(Registers reg) {
        reg.R0 = BitConverter.DoubleToInt64Bits(default(VALUE).Run(reg));
        reg.NextBlock = -1;
        return reg;
    }
}

struct TermTrap : Terminator {
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public Registers Run(Registers reg) {
        throw new Exception("trap");
    }
}

public interface IBody {
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
    public long Run(Registers reg_in) {
        // move to a real local instead of fucking around with a pointer
        // seems to reduce the amount of unnecessary copies?
        Registers reg = reg_in;
        for (;;) {
            switch (reg.NextBlock) {
                case 0: reg = default(B0).Run(reg); break;
                case 1: reg = default(B1).Run(reg); break;
                case 2: reg = default(B2).Run(reg); break;
                case 3: reg = default(B3).Run(reg); break;
                case 4: reg = default(B4).Run(reg); break;
                case 5: reg = default(B5).Run(reg); break;
                case 6: reg = default(B6).Run(reg); break;
                case 7: reg = default(B7).Run(reg); break;
                case 8: reg = default(B8).Run(reg); break;
                case 9: reg = default(B9).Run(reg); break;
                default: return reg.R0;
            }
        }
    }
}
