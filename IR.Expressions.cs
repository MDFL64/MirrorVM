public abstract class Expression {
    public readonly ValType Type;

    public Expression(ValType ty) {
        Type = ty;
    }

    public virtual Type BuildHell() {
        throw new Exception("todo build hell: "+this.GetType());
    }
}

class GetLocal : Expression {
    int LocalIndex;

    public GetLocal(int index, ValType ty) : base(ty) {
        LocalIndex = index;
    }

    public override string ToString()
    {
        return "L"+LocalIndex;
    }

    public override Type BuildHell() {
        if (Type == ValType.I32) {
            switch (LocalIndex) {
                case 0: return typeof(GetR0_I32);
                case 1: return typeof(GetR1_I32);
                case 2: return typeof(GetR2_I32);
                case 3: return typeof(GetR3_I32);
                case 4: return typeof(GetR4_I32);
                case 5: return typeof(GetR5_I32);
                case 6: return typeof(GetR6_I32);
                case 7: return typeof(GetR7_I32);

                default: throw new Exception("register-get out of bounds");
            }
        } else if (Type == ValType.I64) {
            switch (LocalIndex) {
                case 0: return typeof(GetR0_I64);
                case 1: return typeof(GetR1_I64);
                case 2: return typeof(GetR2_I64);
                case 3: return typeof(GetR3_I64);
                case 4: return typeof(GetR4_I64);
                case 5: return typeof(GetR5_I64);
                case 6: return typeof(GetR6_I64);
                case 7: return typeof(GetR7_I64);

                default: throw new Exception("register-get out of bounds");
            }
        } else {
            throw new Exception("todo local type: "+Type);
        }
    }
}

class Constant : Expression {
    long Value;

    public static Constant I32(long x) {
        return new Constant(x, ValType.I32);
    }

    private Constant(long value, ValType ty) : base(ty) {
        Value = value;
    }

    public override string ToString()
    {
        return Value.ToString();
    }

    public override Type BuildHell() {
        if (Type != ValType.I32) {
            throw new Exception("todo non-i32 constants");
        }
        return HellBuilder.MakeGeneric(typeof(Const_I32<>),[HellBuilder.MakeConstant(Value)]);
    }
}

class BinaryOp : Expression {
    public BinaryOpKind Kind;
    public Expression A;
    public Expression B;

    public BinaryOp(BinaryOpKind kind, Expression a, Expression b) : base(GetOpType(kind)) {
        Kind = kind;
        A = a;
        B = b;
    }

    static private ValType GetOpType(BinaryOpKind kind) {
        if (kind < BinaryOpKind.LAST_I32) {
            return ValType.I32;
        } else {
            return ValType.I64;
        }
    }

    public override string ToString()
    {
        return Kind+"("+A+","+B+")";
    }

    public override Type BuildHell() {
        Type ty;
        switch (Kind) {
            case BinaryOpKind.I32_Add: ty = typeof(Op_I32_Add<,>); break;
            case BinaryOpKind.I32_Sub: ty = typeof(Op_I32_Sub<,>); break;
            case BinaryOpKind.I32_Mul: ty = typeof(Op_I32_Mul<,>); break;
            case BinaryOpKind.I32_Div_S: ty = typeof(Op_I32_Div_S<,>); break;
            case BinaryOpKind.I32_Div_U: ty = typeof(Op_I32_Div_U<,>); break;
            case BinaryOpKind.I32_Rem_S: ty = typeof(Op_I32_Rem_S<,>); break;
            case BinaryOpKind.I32_Rem_U: ty = typeof(Op_I32_Rem_U<,>); break;
            case BinaryOpKind.I32_And: ty = typeof(Op_I32_And<,>); break;
            case BinaryOpKind.I32_Or: ty = typeof(Op_I32_Or<,>); break;
            case BinaryOpKind.I32_Xor: ty = typeof(Op_I32_Xor<,>); break;
            case BinaryOpKind.I32_ShiftLeft: ty = typeof(Op_I32_ShiftLeft<,>); break;
            case BinaryOpKind.I32_ShiftRight_S: ty = typeof(Op_I32_ShiftRight_S<,>); break;
            case BinaryOpKind.I32_ShiftRight_U: ty = typeof(Op_I32_ShiftRight_U<,>); break;
            case BinaryOpKind.I32_RotateLeft: ty = typeof(Op_I32_RotateLeft<,>); break;
            case BinaryOpKind.I32_RotateRight: ty = typeof(Op_I32_RotateRight<,>); break;
            case BinaryOpKind.I32_Equal: ty = typeof(Op_I32_Equal<,>); break;
            case BinaryOpKind.I32_NotEqual: ty = typeof(Op_I32_NotEqual<,>); break;
            case BinaryOpKind.I32_Less_S: ty = typeof(Op_I32_Less_S<,>); break;
            case BinaryOpKind.I32_LessEqual_S: ty = typeof(Op_I32_LessEqual_S<,>); break;
            case BinaryOpKind.I32_Greater_S: ty = typeof(Op_I32_Greater_S<,>); break;
            case BinaryOpKind.I32_GreaterEqual_S: ty = typeof(Op_I32_GreaterEqual_S<,>); break;
            case BinaryOpKind.I32_Less_U: ty = typeof(Op_I32_Less_U<,>); break;
            case BinaryOpKind.I32_LessEqual_U: ty = typeof(Op_I32_LessEqual_U<,>); break;
            case BinaryOpKind.I32_Greater_U: ty = typeof(Op_I32_Greater_U<,>); break;
            case BinaryOpKind.I32_GreaterEqual_U: ty = typeof(Op_I32_GreaterEqual_U<,>); break;

            case BinaryOpKind.I64_Add: ty = typeof(Op_I64_Add<,>); break;
            case BinaryOpKind.I64_Sub: ty = typeof(Op_I64_Sub<,>); break;
            case BinaryOpKind.I64_Mul: ty = typeof(Op_I64_Mul<,>); break;
            case BinaryOpKind.I64_Div_S: ty = typeof(Op_I64_Div_S<,>); break;
            case BinaryOpKind.I64_Div_U: ty = typeof(Op_I64_Div_U<,>); break;
            case BinaryOpKind.I64_Rem_S: ty = typeof(Op_I64_Rem_S<,>); break;
            case BinaryOpKind.I64_Rem_U: ty = typeof(Op_I64_Rem_U<,>); break;
            case BinaryOpKind.I64_And: ty = typeof(Op_I64_And<,>); break;
            case BinaryOpKind.I64_Or: ty = typeof(Op_I64_Or<,>); break;
            case BinaryOpKind.I64_Xor: ty = typeof(Op_I64_Xor<,>); break;
            case BinaryOpKind.I64_ShiftLeft: ty = typeof(Op_I64_ShiftLeft<,>); break;
            case BinaryOpKind.I64_ShiftRight_S: ty = typeof(Op_I64_ShiftRight_S<,>); break;
            case BinaryOpKind.I64_ShiftRight_U: ty = typeof(Op_I64_ShiftRight_U<,>); break;
            case BinaryOpKind.I64_RotateLeft: ty = typeof(Op_I64_RotateLeft<,>); break;
            case BinaryOpKind.I64_RotateRight: ty = typeof(Op_I64_RotateRight<,>); break;
            case BinaryOpKind.I64_Equal: ty = typeof(Op_I64_Equal<,>); break;
            case BinaryOpKind.I64_NotEqual: ty = typeof(Op_I64_NotEqual<,>); break;
            case BinaryOpKind.I64_Less_S: ty = typeof(Op_I64_Less_S<,>); break;
            case BinaryOpKind.I64_LessEqual_S: ty = typeof(Op_I64_LessEqual_S<,>); break;
            case BinaryOpKind.I64_Greater_S: ty = typeof(Op_I64_Greater_S<,>); break;
            case BinaryOpKind.I64_GreaterEqual_S: ty = typeof(Op_I64_GreaterEqual_S<,>); break;
            case BinaryOpKind.I64_Less_U: ty = typeof(Op_I64_Less_U<,>); break;
            case BinaryOpKind.I64_LessEqual_U: ty = typeof(Op_I64_LessEqual_U<,>); break;
            case BinaryOpKind.I64_Greater_U: ty = typeof(Op_I64_Greater_U<,>); break;
            case BinaryOpKind.I64_GreaterEqual_U: ty = typeof(Op_I64_GreaterEqual_U<,>); break;

            default:
                throw new Exception("todo build hell: "+Kind);
        }
        var lhs = A.BuildHell();
        var rhs = B.BuildHell();

        return HellBuilder.MakeGeneric(ty,[lhs,rhs]);
    }
}

class UnaryOp : Expression {
    public UnaryOpKind Kind;
    public Expression A;

    public UnaryOp(UnaryOpKind kind, Expression a) : base(GetOpType(kind)) {
        Kind = kind;
        A = a;
    }

    static private ValType GetOpType(UnaryOpKind kind) {
        switch (kind) {
            case UnaryOpKind.I32_EqualZero:
            case UnaryOpKind.I64_EqualZero:
                return ValType.I32;
            case UnaryOpKind.I32_LeadingZeros:
            case UnaryOpKind.I32_TrailingZeros:
            case UnaryOpKind.I32_PopCount:
            case UnaryOpKind.I32_Extend8_S:
            case UnaryOpKind.I32_Extend16_S:
                return ValType.I32;
            case UnaryOpKind.I64_LeadingZeros:
            case UnaryOpKind.I64_TrailingZeros:
            case UnaryOpKind.I64_PopCount:
            case UnaryOpKind.I64_Extend8_S:
            case UnaryOpKind.I64_Extend16_S:
            case UnaryOpKind.I64_Extend32_S:
                return ValType.I64;
            default:
                throw new Exception("todo unop ty: "+kind);
        }
    }

    public override string ToString()
    {
        return Kind+"("+A+")";
    }

    public override Type BuildHell() {
        Type ty;
        switch (Kind) {
            case UnaryOpKind.I32_EqualZero: ty = typeof(Op_I32_EqualZero<>); break;
            case UnaryOpKind.I32_LeadingZeros: ty = typeof(Op_I32_LeadingZeros<>); break;
            case UnaryOpKind.I32_TrailingZeros: ty = typeof(Op_I32_TrailingZeros<>); break;
            case UnaryOpKind.I32_PopCount: ty = typeof(Op_I32_PopCount<>); break;
            case UnaryOpKind.I32_Extend8_S: ty = typeof(Op_I32_Extend8_S<>); break;
            case UnaryOpKind.I32_Extend16_S: ty = typeof(Op_I32_Extend16_S<>); break;

            case UnaryOpKind.I64_EqualZero: ty = typeof(Op_I64_EqualZero<>); break;
            case UnaryOpKind.I64_LeadingZeros: ty = typeof(Op_I64_LeadingZeros<>); break;
            case UnaryOpKind.I64_TrailingZeros: ty = typeof(Op_I64_TrailingZeros<>); break;
            case UnaryOpKind.I64_PopCount: ty = typeof(Op_I64_PopCount<>); break;
            case UnaryOpKind.I64_Extend8_S: ty = typeof(Op_I64_Extend8_S<>); break;
            case UnaryOpKind.I64_Extend16_S: ty = typeof(Op_I64_Extend16_S<>); break;
            case UnaryOpKind.I64_Extend32_S: ty = typeof(Op_I64_Extend32_S<>); break;

            default:
                throw new Exception("todo build hell: "+Kind);
        }
        var arg = A.BuildHell();

        return HellBuilder.MakeGeneric(ty,[arg]);
    }
}

class SelectOp : Expression {
    public Expression A;
    public Expression B;
    public Expression Cond;

    public SelectOp(Expression a, Expression b, Expression cond) : base(a.Type) {
        A = a;
        B = b;
        Cond = cond;
    }

    public override string ToString()
    {
        return "Select("+Cond+","+A+","+B+")";
    }

    public override Type BuildHell()
    {
        var cond = Cond.BuildHell();
        var a = A.BuildHell();
        var b = B.BuildHell();

        if (Type != ValType.I32) {
            throw new Exception("todo other select");
        }

        return HellBuilder.MakeGeneric(typeof(Select_I32<,,>),[cond,a,b]);
    }
}

enum BinaryOpKind {
    // comparison ops, all of these return i32
    I32_Equal = 0x46,
    I32_NotEqual,
    I32_Less_S,
    I32_Less_U,
    I32_Greater_S,
    I32_Greater_U,
    I32_LessEqual_S,
    I32_LessEqual_U,
    I32_GreaterEqual_S,
    I32_GreaterEqual_U,

    I64_Equal = 0x51,
    I64_NotEqual,
    I64_Less_S,
    I64_Less_U,
    I64_Greater_S,
    I64_Greater_U,
    I64_LessEqual_S,
    I64_LessEqual_U,
    I64_GreaterEqual_S,
    I64_GreaterEqual_U,

    LAST_I32,

    I32_Add = 0x6A,
    I32_Sub,
    I32_Mul,
    I32_Div_S,
    I32_Div_U,
    I32_Rem_S,
    I32_Rem_U,
    I32_And,
    I32_Or,
    I32_Xor,
    I32_ShiftLeft,
    I32_ShiftRight_S,
    I32_ShiftRight_U,
    I32_RotateLeft,
    I32_RotateRight,

    I64_Add = 0x7C,
    I64_Sub,
    I64_Mul,
    I64_Div_S,
    I64_Div_U,
    I64_Rem_S,
    I64_Rem_U,
    I64_And,
    I64_Or,
    I64_Xor,
    I64_ShiftLeft,
    I64_ShiftRight_S,
    I64_ShiftRight_U,
    I64_RotateLeft,
    I64_RotateRight
}

enum UnaryOpKind {
    // i32
    I32_EqualZero = 0x45,
    I64_EqualZero = 0x50,

    I32_LeadingZeros = 0x67,
    I32_TrailingZeros,
    I32_PopCount,

    I64_LeadingZeros = 0x79,
    I64_TrailingZeros,
    I64_PopCount,

    I32_Extend8_S = 0xC0,
    I32_Extend16_S,
    I64_Extend8_S,
    I64_Extend16_S,
    I64_Extend32_S,
}
