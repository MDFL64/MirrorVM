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

struct ExprStmt<VALUE,T,NEXT> : Stmt where VALUE: struct, Expr<T> where NEXT: struct, Stmt {
    public void Run(ref Registers reg, Span<long> frame, WasmInstance inst) {
        default(VALUE).Run(ref reg, frame, inst);
        default(NEXT).Run(ref reg, frame, inst);
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

struct Body<
    B0,B1,B2,B3,B4,B5,B6,B7,B8,B9,
    B10,B11,B12,B13,B14,B15,B16,B17,B18,B19,
    B20,B21,B22,B23,B24,B25,B26,B27,B28,B29,
    B30,B31,B32,B33,B34,B35,B36,B37,B38,B39,
    B40,B41,B42,B43,B44,B45,B46,B47,B48,B49,
    SETUP,EXTRA_RET_COUNT,FRAME_SIZE
> : ICallable
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

    where B10: struct, Terminator
    where B11: struct, Terminator
    where B12: struct, Terminator
    where B13: struct, Terminator
    where B14: struct, Terminator
    where B15: struct, Terminator
    where B16: struct, Terminator
    where B17: struct, Terminator
    where B18: struct, Terminator
    where B19: struct, Terminator

    where B20: struct, Terminator
    where B21: struct, Terminator
    where B22: struct, Terminator
    where B23: struct, Terminator
    where B24: struct, Terminator
    where B25: struct, Terminator
    where B26: struct, Terminator
    where B27: struct, Terminator
    where B28: struct, Terminator
    where B29: struct, Terminator

    where B30: struct, Terminator
    where B31: struct, Terminator
    where B32: struct, Terminator
    where B33: struct, Terminator
    where B34: struct, Terminator
    where B35: struct, Terminator
    where B36: struct, Terminator
    where B37: struct, Terminator
    where B38: struct, Terminator
    where B39: struct, Terminator

    where B40: struct, Terminator
    where B41: struct, Terminator
    where B42: struct, Terminator
    where B43: struct, Terminator
    where B44: struct, Terminator
    where B45: struct, Terminator
    where B46: struct, Terminator
    where B47: struct, Terminator
    where B48: struct, Terminator
    where B49: struct, Terminator

    where SETUP: struct, ArgRead
    where EXTRA_RET_COUNT: struct, Const
    where FRAME_SIZE: struct, Const
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public long Call(Span<long> args, WasmInstance inst) {
        int frame_size = (int)default(FRAME_SIZE).Run();
        Span<long> frame = stackalloc long[frame_size];
        Registers reg = default(SETUP).Run(args, frame);
        for (;;) {
            switch (reg.NextBlock) {
                case 0: default(B0).Run(ref reg, frame, inst); break;
                case 1: default(B1).Run(ref reg, frame, inst); break;
                case 2: default(B2).Run(ref reg, frame, inst); break;
                case 3: default(B3).Run(ref reg, frame, inst); break;
                case 4: default(B4).Run(ref reg, frame, inst); break;
                case 5: default(B5).Run(ref reg, frame, inst); break;
                case 6: default(B6).Run(ref reg, frame, inst); break;
                case 7: default(B7).Run(ref reg, frame, inst); break;
                case 8: default(B8).Run(ref reg, frame, inst); break;
                case 9: default(B9).Run(ref reg, frame, inst); break;

                case 10: default(B10).Run(ref reg, frame, inst); break;
                case 11: default(B11).Run(ref reg, frame, inst); break;
                case 12: default(B12).Run(ref reg, frame, inst); break;
                case 13: default(B13).Run(ref reg, frame, inst); break;
                case 14: default(B14).Run(ref reg, frame, inst); break;
                case 15: default(B15).Run(ref reg, frame, inst); break;
                case 16: default(B16).Run(ref reg, frame, inst); break;
                case 17: default(B17).Run(ref reg, frame, inst); break;
                case 18: default(B18).Run(ref reg, frame, inst); break;
                case 19: default(B19).Run(ref reg, frame, inst); break;

                case 20: default(B20).Run(ref reg, frame, inst); break;
                case 21: default(B21).Run(ref reg, frame, inst); break;
                case 22: default(B22).Run(ref reg, frame, inst); break;
                case 23: default(B23).Run(ref reg, frame, inst); break;
                case 24: default(B24).Run(ref reg, frame, inst); break;
                case 25: default(B25).Run(ref reg, frame, inst); break;
                case 26: default(B26).Run(ref reg, frame, inst); break;
                case 27: default(B27).Run(ref reg, frame, inst); break;
                case 28: default(B28).Run(ref reg, frame, inst); break;
                case 29: default(B29).Run(ref reg, frame, inst); break;

                case 30: default(B30).Run(ref reg, frame, inst); break;
                case 31: default(B31).Run(ref reg, frame, inst); break;
                case 32: default(B32).Run(ref reg, frame, inst); break;
                case 33: default(B33).Run(ref reg, frame, inst); break;
                case 34: default(B34).Run(ref reg, frame, inst); break;
                case 35: default(B35).Run(ref reg, frame, inst); break;
                case 36: default(B36).Run(ref reg, frame, inst); break;
                case 37: default(B37).Run(ref reg, frame, inst); break;
                case 38: default(B38).Run(ref reg, frame, inst); break;
                case 39: default(B39).Run(ref reg, frame, inst); break;

                case 40: default(B40).Run(ref reg, frame, inst); break;
                case 41: default(B41).Run(ref reg, frame, inst); break;
                case 42: default(B42).Run(ref reg, frame, inst); break;
                case 43: default(B43).Run(ref reg, frame, inst); break;
                case 44: default(B44).Run(ref reg, frame, inst); break;
                case 45: default(B45).Run(ref reg, frame, inst); break;
                case 46: default(B46).Run(ref reg, frame, inst); break;
                case 47: default(B47).Run(ref reg, frame, inst); break;
                case 48: default(B48).Run(ref reg, frame, inst); break;
                case 49: default(B49).Run(ref reg, frame, inst); break;

                default: {
                    long extra_ret_count = default(EXTRA_RET_COUNT).Run();
                    for (int i=0;i<extra_ret_count;i++) {
                        args[i] = frame[i];
                    }
                    return reg.R0;
                }
            }
        }
    }
}

struct PairGlue<A,B> : Stmt
    where A: struct, Stmt
    where B: struct, Stmt
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public void Run(ref Registers reg, Span<long> frame, WasmInstance inst)
    {
        Alpha(ref reg, frame, inst);
        default(B).Run(ref reg, frame, inst);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    private static void Alpha(ref Registers reg, Span<long> frame, WasmInstance inst) {
        default(A).Run(ref reg, frame, inst);
    }
}
