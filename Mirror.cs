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

struct Body<
    B0, B1, B2, B3, B4, B5, B6, B7, B8, B9,
    B10, B11, B12, B13, B14, B15, B16, B17, B18, B19,
    B20, B21, B22, B23, B24, B25, B26, B27, B28, B29,
    B30, B31, B32, B33, B34, B35, B36, B37, B38, B39,
    B40, B41, B42, B43, B44, B45, B46, B47, B48, B49,
    B50, B51, B52, B53, B54, B55, B56, B57, B58, B59,
    B60, B61, B62, B63, B64, B65, B66, B67, B68, B69,
    B70, B71, B72, B73, B74, B75, B76, B77, B78, B79,
    B80, B81, B82, B83, B84, B85, B86, B87, B88, B89,
    B90, B91, B92, B93, B94, B95, B96, B97, B98, B99,

    B100, B101, B102, B103, B104, B105, B106, B107, B108, B109,
    B110, B111, B112, B113, B114, B115, B116, B117, B118, B119,
    B120, B121, B122, B123, B124, B125, B126, B127, B128, B129,
    B130, B131, B132, B133, B134, B135, B136, B137, B138, B139,
    B140, B141, B142, B143, B144, B145, B146, B147, B148, B149,
    B150, B151, B152, B153, B154, B155, B156, B157, B158, B159,
    B160, B161, B162, B163, B164, B165, B166, B167, B168, B169,
    B170, B171, B172, B173, B174, B175, B176, B177, B178, B179,
    B180, B181, B182, B183, B184, B185, B186, B187, B188, B189,
    B190, B191, B192, B193, B194, B195, B196, B197, B198, B199,

    SETUP, EXTRA_RET_COUNT, FRAME_SIZE
> : ICallable
    where B0 : struct, Terminator where B1 : struct, Terminator where B2 : struct, Terminator where B3 : struct, Terminator where B4 : struct, Terminator
    where B5 : struct, Terminator where B6 : struct, Terminator where B7 : struct, Terminator where B8 : struct, Terminator where B9 : struct, Terminator

    where B10 : struct, Terminator where B11 : struct, Terminator where B12 : struct, Terminator where B13 : struct, Terminator where B14 : struct, Terminator
    where B15 : struct, Terminator where B16 : struct, Terminator where B17 : struct, Terminator where B18 : struct, Terminator where B19 : struct, Terminator
    where B20 : struct, Terminator where B21 : struct, Terminator where B22 : struct, Terminator where B23 : struct, Terminator where B24 : struct, Terminator
    where B25 : struct, Terminator where B26 : struct, Terminator where B27 : struct, Terminator where B28 : struct, Terminator where B29 : struct, Terminator
    where B30 : struct, Terminator where B31 : struct, Terminator where B32 : struct, Terminator where B33 : struct, Terminator where B34 : struct, Terminator
    where B35 : struct, Terminator where B36 : struct, Terminator where B37 : struct, Terminator where B38 : struct, Terminator where B39 : struct, Terminator
    where B40 : struct, Terminator where B41 : struct, Terminator where B42 : struct, Terminator where B43 : struct, Terminator where B44 : struct, Terminator
    where B45 : struct, Terminator where B46 : struct, Terminator where B47 : struct, Terminator where B48 : struct, Terminator where B49 : struct, Terminator
    where B50 : struct, Terminator where B51 : struct, Terminator where B52 : struct, Terminator where B53 : struct, Terminator where B54 : struct, Terminator
    where B55 : struct, Terminator where B56 : struct, Terminator where B57 : struct, Terminator where B58 : struct, Terminator where B59 : struct, Terminator
    where B60 : struct, Terminator where B61 : struct, Terminator where B62 : struct, Terminator where B63 : struct, Terminator where B64 : struct, Terminator
    where B65 : struct, Terminator where B66 : struct, Terminator where B67 : struct, Terminator where B68 : struct, Terminator where B69 : struct, Terminator
    where B70 : struct, Terminator where B71 : struct, Terminator where B72 : struct, Terminator where B73 : struct, Terminator where B74 : struct, Terminator
    where B75 : struct, Terminator where B76 : struct, Terminator where B77 : struct, Terminator where B78 : struct, Terminator where B79 : struct, Terminator
    where B80 : struct, Terminator where B81 : struct, Terminator where B82 : struct, Terminator where B83 : struct, Terminator where B84 : struct, Terminator
    where B85 : struct, Terminator where B86 : struct, Terminator where B87 : struct, Terminator where B88 : struct, Terminator where B89 : struct, Terminator
    where B90 : struct, Terminator where B91 : struct, Terminator where B92 : struct, Terminator where B93 : struct, Terminator where B94 : struct, Terminator
    where B95 : struct, Terminator where B96 : struct, Terminator where B97 : struct, Terminator where B98 : struct, Terminator where B99 : struct, Terminator

    where B100 : struct, Terminator where B101 : struct, Terminator where B102 : struct, Terminator where B103 : struct, Terminator where B104 : struct, Terminator
    where B105 : struct, Terminator where B106 : struct, Terminator where B107 : struct, Terminator where B108 : struct, Terminator where B109 : struct, Terminator
    where B110 : struct, Terminator where B111 : struct, Terminator where B112 : struct, Terminator where B113 : struct, Terminator where B114 : struct, Terminator
    where B115 : struct, Terminator where B116 : struct, Terminator where B117 : struct, Terminator where B118 : struct, Terminator where B119 : struct, Terminator
    where B120 : struct, Terminator where B121 : struct, Terminator where B122 : struct, Terminator where B123 : struct, Terminator where B124 : struct, Terminator
    where B125 : struct, Terminator where B126 : struct, Terminator where B127 : struct, Terminator where B128 : struct, Terminator where B129 : struct, Terminator
    where B130 : struct, Terminator where B131 : struct, Terminator where B132 : struct, Terminator where B133 : struct, Terminator where B134 : struct, Terminator
    where B135 : struct, Terminator where B136 : struct, Terminator where B137 : struct, Terminator where B138 : struct, Terminator where B139 : struct, Terminator
    where B140 : struct, Terminator where B141 : struct, Terminator where B142 : struct, Terminator where B143 : struct, Terminator where B144 : struct, Terminator
    where B145 : struct, Terminator where B146 : struct, Terminator where B147 : struct, Terminator where B148 : struct, Terminator where B149 : struct, Terminator
    where B150 : struct, Terminator where B151 : struct, Terminator where B152 : struct, Terminator where B153 : struct, Terminator where B154 : struct, Terminator
    where B155 : struct, Terminator where B156 : struct, Terminator where B157 : struct, Terminator where B158 : struct, Terminator where B159 : struct, Terminator
    where B160 : struct, Terminator where B161 : struct, Terminator where B162 : struct, Terminator where B163 : struct, Terminator where B164 : struct, Terminator
    where B165 : struct, Terminator where B166 : struct, Terminator where B167 : struct, Terminator where B168 : struct, Terminator where B169 : struct, Terminator
    where B170 : struct, Terminator where B171 : struct, Terminator where B172 : struct, Terminator where B173 : struct, Terminator where B174 : struct, Terminator
    where B175 : struct, Terminator where B176 : struct, Terminator where B177 : struct, Terminator where B178 : struct, Terminator where B179 : struct, Terminator
    where B180 : struct, Terminator where B181 : struct, Terminator where B182 : struct, Terminator where B183 : struct, Terminator where B184 : struct, Terminator
    where B185 : struct, Terminator where B186 : struct, Terminator where B187 : struct, Terminator where B188 : struct, Terminator where B189 : struct, Terminator
    where B190 : struct, Terminator where B191 : struct, Terminator where B192 : struct, Terminator where B193 : struct, Terminator where B194 : struct, Terminator
    where B195 : struct, Terminator where B196 : struct, Terminator where B197 : struct, Terminator where B198 : struct, Terminator where B199 : struct, Terminator

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
        for (; ; )
        {
            switch (reg.NextBlock)
            {
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

                case 50: default(B50).Run(ref reg, frame, inst); break;
                case 51: default(B51).Run(ref reg, frame, inst); break;
                case 52: default(B52).Run(ref reg, frame, inst); break;
                case 53: default(B53).Run(ref reg, frame, inst); break;
                case 54: default(B54).Run(ref reg, frame, inst); break;
                case 55: default(B55).Run(ref reg, frame, inst); break;
                case 56: default(B56).Run(ref reg, frame, inst); break;
                case 57: default(B57).Run(ref reg, frame, inst); break;
                case 58: default(B58).Run(ref reg, frame, inst); break;
                case 59: default(B59).Run(ref reg, frame, inst); break;

                case 60: default(B60).Run(ref reg, frame, inst); break;
                case 61: default(B61).Run(ref reg, frame, inst); break;
                case 62: default(B62).Run(ref reg, frame, inst); break;
                case 63: default(B63).Run(ref reg, frame, inst); break;
                case 64: default(B64).Run(ref reg, frame, inst); break;
                case 65: default(B65).Run(ref reg, frame, inst); break;
                case 66: default(B66).Run(ref reg, frame, inst); break;
                case 67: default(B67).Run(ref reg, frame, inst); break;
                case 68: default(B68).Run(ref reg, frame, inst); break;
                case 69: default(B69).Run(ref reg, frame, inst); break;

                case 70: default(B70).Run(ref reg, frame, inst); break;
                case 71: default(B71).Run(ref reg, frame, inst); break;
                case 72: default(B72).Run(ref reg, frame, inst); break;
                case 73: default(B73).Run(ref reg, frame, inst); break;
                case 74: default(B74).Run(ref reg, frame, inst); break;
                case 75: default(B75).Run(ref reg, frame, inst); break;
                case 76: default(B76).Run(ref reg, frame, inst); break;
                case 77: default(B77).Run(ref reg, frame, inst); break;
                case 78: default(B78).Run(ref reg, frame, inst); break;
                case 79: default(B79).Run(ref reg, frame, inst); break;

                case 80: default(B80).Run(ref reg, frame, inst); break;
                case 81: default(B81).Run(ref reg, frame, inst); break;
                case 82: default(B82).Run(ref reg, frame, inst); break;
                case 83: default(B83).Run(ref reg, frame, inst); break;
                case 84: default(B84).Run(ref reg, frame, inst); break;
                case 85: default(B85).Run(ref reg, frame, inst); break;
                case 86: default(B86).Run(ref reg, frame, inst); break;
                case 87: default(B87).Run(ref reg, frame, inst); break;
                case 88: default(B88).Run(ref reg, frame, inst); break;
                case 89: default(B89).Run(ref reg, frame, inst); break;

                case 90: default(B90).Run(ref reg, frame, inst); break;
                case 91: default(B91).Run(ref reg, frame, inst); break;
                case 92: default(B92).Run(ref reg, frame, inst); break;
                case 93: default(B93).Run(ref reg, frame, inst); break;
                case 94: default(B94).Run(ref reg, frame, inst); break;
                case 95: default(B95).Run(ref reg, frame, inst); break;
                case 96: default(B96).Run(ref reg, frame, inst); break;
                case 97: default(B97).Run(ref reg, frame, inst); break;
                case 98: default(B98).Run(ref reg, frame, inst); break;
                case 99: default(B99).Run(ref reg, frame, inst); break;

                case 100: default(B100).Run(ref reg, frame, inst); break;
                case 101: default(B101).Run(ref reg, frame, inst); break;
                case 102: default(B102).Run(ref reg, frame, inst); break;
                case 103: default(B103).Run(ref reg, frame, inst); break;
                case 104: default(B104).Run(ref reg, frame, inst); break;
                case 105: default(B105).Run(ref reg, frame, inst); break;
                case 106: default(B106).Run(ref reg, frame, inst); break;
                case 107: default(B107).Run(ref reg, frame, inst); break;
                case 108: default(B108).Run(ref reg, frame, inst); break;
                case 109: default(B109).Run(ref reg, frame, inst); break;

                case 110: default(B110).Run(ref reg, frame, inst); break;
                case 111: default(B111).Run(ref reg, frame, inst); break;
                case 112: default(B112).Run(ref reg, frame, inst); break;
                case 113: default(B113).Run(ref reg, frame, inst); break;
                case 114: default(B114).Run(ref reg, frame, inst); break;
                case 115: default(B115).Run(ref reg, frame, inst); break;
                case 116: default(B116).Run(ref reg, frame, inst); break;
                case 117: default(B117).Run(ref reg, frame, inst); break;
                case 118: default(B118).Run(ref reg, frame, inst); break;
                case 119: default(B119).Run(ref reg, frame, inst); break;

                case 120: default(B120).Run(ref reg, frame, inst); break;
                case 121: default(B121).Run(ref reg, frame, inst); break;
                case 122: default(B122).Run(ref reg, frame, inst); break;
                case 123: default(B123).Run(ref reg, frame, inst); break;
                case 124: default(B124).Run(ref reg, frame, inst); break;
                case 125: default(B125).Run(ref reg, frame, inst); break;
                case 126: default(B126).Run(ref reg, frame, inst); break;
                case 127: default(B127).Run(ref reg, frame, inst); break;
                case 128: default(B128).Run(ref reg, frame, inst); break;
                case 129: default(B129).Run(ref reg, frame, inst); break;

                case 130: default(B130).Run(ref reg, frame, inst); break;
                case 131: default(B131).Run(ref reg, frame, inst); break;
                case 132: default(B132).Run(ref reg, frame, inst); break;
                case 133: default(B133).Run(ref reg, frame, inst); break;
                case 134: default(B134).Run(ref reg, frame, inst); break;
                case 135: default(B135).Run(ref reg, frame, inst); break;
                case 136: default(B136).Run(ref reg, frame, inst); break;
                case 137: default(B137).Run(ref reg, frame, inst); break;
                case 138: default(B138).Run(ref reg, frame, inst); break;
                case 139: default(B139).Run(ref reg, frame, inst); break;

                case 140: default(B140).Run(ref reg, frame, inst); break;
                case 141: default(B141).Run(ref reg, frame, inst); break;
                case 142: default(B142).Run(ref reg, frame, inst); break;
                case 143: default(B143).Run(ref reg, frame, inst); break;
                case 144: default(B144).Run(ref reg, frame, inst); break;
                case 145: default(B145).Run(ref reg, frame, inst); break;
                case 146: default(B146).Run(ref reg, frame, inst); break;
                case 147: default(B147).Run(ref reg, frame, inst); break;
                case 148: default(B148).Run(ref reg, frame, inst); break;
                case 149: default(B149).Run(ref reg, frame, inst); break;

                case 150: default(B150).Run(ref reg, frame, inst); break;
                case 151: default(B151).Run(ref reg, frame, inst); break;
                case 152: default(B152).Run(ref reg, frame, inst); break;
                case 153: default(B153).Run(ref reg, frame, inst); break;
                case 154: default(B154).Run(ref reg, frame, inst); break;
                case 155: default(B155).Run(ref reg, frame, inst); break;
                case 156: default(B156).Run(ref reg, frame, inst); break;
                case 157: default(B157).Run(ref reg, frame, inst); break;
                case 158: default(B158).Run(ref reg, frame, inst); break;
                case 159: default(B159).Run(ref reg, frame, inst); break;

                case 160: default(B160).Run(ref reg, frame, inst); break;
                case 161: default(B161).Run(ref reg, frame, inst); break;
                case 162: default(B162).Run(ref reg, frame, inst); break;
                case 163: default(B163).Run(ref reg, frame, inst); break;
                case 164: default(B164).Run(ref reg, frame, inst); break;
                case 165: default(B165).Run(ref reg, frame, inst); break;
                case 166: default(B166).Run(ref reg, frame, inst); break;
                case 167: default(B167).Run(ref reg, frame, inst); break;
                case 168: default(B168).Run(ref reg, frame, inst); break;
                case 169: default(B169).Run(ref reg, frame, inst); break;

                case 170: default(B170).Run(ref reg, frame, inst); break;
                case 171: default(B171).Run(ref reg, frame, inst); break;
                case 172: default(B172).Run(ref reg, frame, inst); break;
                case 173: default(B173).Run(ref reg, frame, inst); break;
                case 174: default(B174).Run(ref reg, frame, inst); break;
                case 175: default(B175).Run(ref reg, frame, inst); break;
                case 176: default(B176).Run(ref reg, frame, inst); break;
                case 177: default(B177).Run(ref reg, frame, inst); break;
                case 178: default(B178).Run(ref reg, frame, inst); break;
                case 179: default(B179).Run(ref reg, frame, inst); break;

                case 180: default(B180).Run(ref reg, frame, inst); break;
                case 181: default(B181).Run(ref reg, frame, inst); break;
                case 182: default(B182).Run(ref reg, frame, inst); break;
                case 183: default(B183).Run(ref reg, frame, inst); break;
                case 184: default(B184).Run(ref reg, frame, inst); break;
                case 185: default(B185).Run(ref reg, frame, inst); break;
                case 186: default(B186).Run(ref reg, frame, inst); break;
                case 187: default(B187).Run(ref reg, frame, inst); break;
                case 188: default(B188).Run(ref reg, frame, inst); break;
                case 189: default(B189).Run(ref reg, frame, inst); break;

                case 190: default(B190).Run(ref reg, frame, inst); break;
                case 191: default(B191).Run(ref reg, frame, inst); break;
                case 192: default(B192).Run(ref reg, frame, inst); break;
                case 193: default(B193).Run(ref reg, frame, inst); break;
                case 194: default(B194).Run(ref reg, frame, inst); break;
                case 195: default(B195).Run(ref reg, frame, inst); break;
                case 196: default(B196).Run(ref reg, frame, inst); break;
                case 197: default(B197).Run(ref reg, frame, inst); break;
                case 198: default(B198).Run(ref reg, frame, inst); break;
                case 199: default(B199).Run(ref reg, frame, inst); break;

                default:
                    {
                        long extra_ret_count = default(EXTRA_RET_COUNT).Run();
                        for (int i = 0; i < extra_ret_count; i++)
                        {
                            args[i] = frame[i];
                        }
                        return reg.R0;
                    }
            }
        }
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

struct Stmts5<A,B,C,D> : Stmt
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

struct Anchor<A> : Stmt
    where A: struct, Stmt
{
    [MethodImpl(MethodImplOptions.NoInlining)]
    public void Run(ref Registers reg, Span<long> frame, WasmInstance inst)
    {
        //Registers r2 = reg;
        default(A).Run(ref reg, frame, inst);
        //reg = r2;
    }
}
