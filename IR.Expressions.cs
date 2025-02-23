public abstract class Expression {
    public readonly ValType Type;

    public Expression(ValType ty) {
        Type = ty;
    }

    // Poor man's visitor, can be used to check some property or to edit the expression tree.
    public abstract void Traverse(Action<Expression> f);

    public bool IsMemoryRead() {
        bool result = false;
        Traverse((e)=>{
            if (e is MemoryOp) {
                result = true;
            }
        });
        return result;
    }

    public bool IsAnyRead() {
        bool result = false;
        Traverse((e)=>{
            if (e is MemoryOp) {
                result = true;
            }
            if (e is Local local) {
                if (local.Kind == LocalKind.Variable) {
                    result = true;
                }
            }
            // TODO globals
        });
        return result;
    }

    public abstract Type BuildHell();
}

class ErrorExpression : Expression {
    public ErrorExpression() : base(ValType.Error) {
        
    }

    public override Type BuildHell()
    {
        throw new NotImplementedException();
    }

    public override void Traverse(Action<Expression> f)
    {
        f(this);
    }
}

class Constant : Expression {
    long Value;
    bool IsConciseFloat;

    public static Constant I32(long x) {
        return new Constant(x, ValType.I32);
    }

    public static Constant I64(long x) {
        return new Constant(x, ValType.I64);
    }

    public static Constant F32(long x) {
        return new Constant(x, ValType.F32);
    }

    public static Constant F64(long x) {
        return new Constant(x, ValType.F64);
    }

    public static Constant NULL(ValType ty) {
        return new Constant(0, ty);
    }

    public static Constant REF_FUNC(long x) {
        return new Constant(x, ValType.FuncRef);
    }

    private Constant(long value, ValType ty) : base(ty) {
        Value = value;
    }

    public override string ToString()
    {
        return Value.ToString();
    }

    public override void Traverse(Action<Expression> f)
    {
        f(this);
    }

    public override Type BuildHell() {
        switch (Type) {
            case ValType.I32: return HellBuilder.MakeGeneric(typeof(Const_I32<>),[HellBuilder.MakeConstant(Value)]);
            case ValType.I64:
            case ValType.ExternRef:
            case ValType.FuncRef:
                return HellBuilder.MakeGeneric(typeof(Const_I64<>),[HellBuilder.MakeConstant(Value)]);

            case ValType.F32: return HellBuilder.MakeGeneric(typeof(Const_F32<>),[HellBuilder.MakeConstant(Value)]);
            case ValType.F64: return HellBuilder.MakeGeneric(typeof(Const_F64<>),[HellBuilder.MakeConstant(Value)]);

            default:
                throw new Exception("todo constant "+Type);
        }
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
        } else if (kind < BinaryOpKind.LAST_I64) {
            return ValType.I64;
        } else if (kind < BinaryOpKind.LAST_F32) {
            return ValType.F32;
        } else {
            return ValType.F64;
        }
    }

    public override string ToString()
    {
        return Kind+"("+A+","+B+")";
    }

    public override void Traverse(Action<Expression> f)
    {
        f(this);
        A.Traverse(f);
        B.Traverse(f);
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

            case BinaryOpKind.F32_Add: ty = typeof(Op_F32_Add<,>); break;
            case BinaryOpKind.F32_Sub: ty = typeof(Op_F32_Sub<,>); break;
            case BinaryOpKind.F32_Mul: ty = typeof(Op_F32_Mul<,>); break;
            case BinaryOpKind.F32_Div: ty = typeof(Op_F32_Div<,>); break;
            case BinaryOpKind.F32_Min: ty = typeof(Op_F32_Min<,>); break;
            case BinaryOpKind.F32_Max: ty = typeof(Op_F32_Max<,>); break;
            case BinaryOpKind.F32_CopySign: ty = typeof(Op_F32_CopySign<,>); break;
            case BinaryOpKind.F32_Equal: ty = typeof(Op_F32_Equal<,>); break;
            case BinaryOpKind.F32_NotEqual: ty = typeof(Op_F32_NotEqual<,>); break;
            case BinaryOpKind.F32_Less: ty = typeof(Op_F32_Less<,>); break;
            case BinaryOpKind.F32_LessEqual: ty = typeof(Op_F32_LessEqual<,>); break;
            case BinaryOpKind.F32_Greater: ty = typeof(Op_F32_Greater<,>); break;
            case BinaryOpKind.F32_GreaterEqual: ty = typeof(Op_F32_GreaterEqual<,>); break;

            case BinaryOpKind.F64_Add: ty = typeof(Op_F64_Add<,>); break;
            case BinaryOpKind.F64_Sub: ty = typeof(Op_F64_Sub<,>); break;
            case BinaryOpKind.F64_Mul: ty = typeof(Op_F64_Mul<,>); break;
            case BinaryOpKind.F64_Div: ty = typeof(Op_F64_Div<,>); break;
            case BinaryOpKind.F64_Min: ty = typeof(Op_F64_Min<,>); break;
            case BinaryOpKind.F64_Max: ty = typeof(Op_F64_Max<,>); break;
            case BinaryOpKind.F64_CopySign: ty = typeof(Op_F64_CopySign<,>); break;
            case BinaryOpKind.F64_Equal: ty = typeof(Op_F64_Equal<,>); break;
            case BinaryOpKind.F64_NotEqual: ty = typeof(Op_F64_NotEqual<,>); break;
            case BinaryOpKind.F64_Less: ty = typeof(Op_F64_Less<,>); break;
            case BinaryOpKind.F64_LessEqual: ty = typeof(Op_F64_LessEqual<,>); break;
            case BinaryOpKind.F64_Greater: ty = typeof(Op_F64_Greater<,>); break;
            case BinaryOpKind.F64_GreaterEqual: ty = typeof(Op_F64_GreaterEqual<,>); break;

            default:
                throw new Exception("todo build binary: "+Kind);
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
            case UnaryOpKind.I32_Wrap_I64:
            case UnaryOpKind.I32_Truncate_F32_S:
            case UnaryOpKind.I32_Truncate_F32_U:
            case UnaryOpKind.I32_Truncate_F64_S:
            case UnaryOpKind.I32_Truncate_F64_U:
            case UnaryOpKind.I32_TruncateSat_F32_S:
            case UnaryOpKind.I32_TruncateSat_F32_U:
            case UnaryOpKind.I32_TruncateSat_F64_S:
            case UnaryOpKind.I32_TruncateSat_F64_U:
            case UnaryOpKind.I32_Reinterpret_F32:
                return ValType.I32;

            case UnaryOpKind.I64_LeadingZeros:
            case UnaryOpKind.I64_TrailingZeros:
            case UnaryOpKind.I64_PopCount:
            case UnaryOpKind.I64_Extend8_S:
            case UnaryOpKind.I64_Extend16_S:
            case UnaryOpKind.I64_Extend32_S:
            case UnaryOpKind.I64_Extend_I32_S:
            case UnaryOpKind.I64_Extend_I32_U:
            case UnaryOpKind.I64_Truncate_F32_S:
            case UnaryOpKind.I64_Truncate_F32_U:
            case UnaryOpKind.I64_Truncate_F64_S:
            case UnaryOpKind.I64_Truncate_F64_U:
            case UnaryOpKind.I64_TruncateSat_F32_S:
            case UnaryOpKind.I64_TruncateSat_F32_U:
            case UnaryOpKind.I64_TruncateSat_F64_S:
            case UnaryOpKind.I64_TruncateSat_F64_U:
            case UnaryOpKind.I64_Reinterpret_F64:
                return ValType.I64;

            case UnaryOpKind.F32_Abs:
            case UnaryOpKind.F32_Neg:
            case UnaryOpKind.F32_Sqrt:
            case UnaryOpKind.F32_Ceil:
            case UnaryOpKind.F32_Floor:
            case UnaryOpKind.F32_Truncate:
            case UnaryOpKind.F32_Nearest:
            case UnaryOpKind.F32_Convert_I32_S:
            case UnaryOpKind.F32_Convert_I32_U:
            case UnaryOpKind.F32_Convert_I64_S:
            case UnaryOpKind.F32_Convert_I64_U:
            case UnaryOpKind.F32_Demote_F64:
            case UnaryOpKind.F32_Reinterpret_I32:
                return ValType.F32;

            case UnaryOpKind.F64_Abs:
            case UnaryOpKind.F64_Neg:
            case UnaryOpKind.F64_Sqrt:
            case UnaryOpKind.F64_Ceil:
            case UnaryOpKind.F64_Floor:
            case UnaryOpKind.F64_Truncate:
            case UnaryOpKind.F64_Nearest:
            case UnaryOpKind.F64_Convert_I32_S:
            case UnaryOpKind.F64_Convert_I32_U:
            case UnaryOpKind.F64_Convert_I64_S:
            case UnaryOpKind.F64_Convert_I64_U:
            case UnaryOpKind.F64_Promote_F32:
            case UnaryOpKind.F64_Reinterpret_I64:
                return ValType.F64;

            default:
                throw new Exception("todo unop ty: "+kind);
        }
    }

    public override string ToString()
    {
        return Kind+"("+A+")";
    }

    public override void Traverse(Action<Expression> f)
    {
        f(this);
        A.Traverse(f);
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

            case UnaryOpKind.F32_Neg: ty = typeof(Op_F32_Neg<>); break;
            case UnaryOpKind.F32_Abs: ty = typeof(Op_F32_Abs<>); break;
            case UnaryOpKind.F32_Sqrt: ty = typeof(Op_F32_Sqrt<>); break;
            case UnaryOpKind.F32_Floor: ty = typeof(Op_F32_Floor<>); break;
            case UnaryOpKind.F32_Ceil: ty = typeof(Op_F32_Ceil<>); break;
            case UnaryOpKind.F32_Truncate: ty = typeof(Op_F32_Truncate<>); break;
            case UnaryOpKind.F32_Nearest: ty = typeof(Op_F32_Nearest<>); break;
            case UnaryOpKind.F32_Convert_I32_S: ty = typeof(Op_F32_Convert_I32_S<>); break;
            case UnaryOpKind.F32_Convert_I32_U: ty = typeof(Op_F32_Convert_I32_U<>); break;
            case UnaryOpKind.F32_Convert_I64_S: ty = typeof(Op_F32_Convert_I64_S<>); break;
            case UnaryOpKind.F32_Convert_I64_U: ty = typeof(Op_F32_Convert_I64_U<>); break;
            case UnaryOpKind.F32_Demote_F64: ty = typeof(Op_F32_Demote_F64<>); break;
            case UnaryOpKind.F32_Reinterpret_I32: ty = typeof(Op_F32_Reinterpret_I32<>); break;

            case UnaryOpKind.F64_Neg: ty = typeof(Op_F64_Neg<>); break;
            case UnaryOpKind.F64_Abs: ty = typeof(Op_F64_Abs<>); break;
            case UnaryOpKind.F64_Sqrt: ty = typeof(Op_F64_Sqrt<>); break;
            case UnaryOpKind.F64_Floor: ty = typeof(Op_F64_Floor<>); break;
            case UnaryOpKind.F64_Ceil: ty = typeof(Op_F64_Ceil<>); break;
            case UnaryOpKind.F64_Truncate: ty = typeof(Op_F64_Truncate<>); break;
            case UnaryOpKind.F64_Nearest: ty = typeof(Op_F64_Nearest<>); break;
            case UnaryOpKind.F64_Convert_I32_S: ty = typeof(Op_F64_Convert_I32_S<>); break;
            case UnaryOpKind.F64_Convert_I32_U: ty = typeof(Op_F64_Convert_I32_U<>); break;
            case UnaryOpKind.F64_Convert_I64_S: ty = typeof(Op_F64_Convert_I64_S<>); break;
            case UnaryOpKind.F64_Convert_I64_U: ty = typeof(Op_F64_Convert_I64_U<>); break;
            case UnaryOpKind.F64_Promote_F32: ty = typeof(Op_F64_Promote_F32<>); break;
            case UnaryOpKind.F64_Reinterpret_I64: ty = typeof(Op_F64_Reinterpret_I64<>); break;

            // conversions
            case UnaryOpKind.I32_Wrap_I64: ty = typeof(Op_I32_Wrap_I64<>); break;
            case UnaryOpKind.I32_Truncate_F32_S: ty = typeof(Op_I32_Truncate_F32_S<>); break;
            case UnaryOpKind.I32_Truncate_F32_U: ty = typeof(Op_I32_Truncate_F32_U<>); break;
            case UnaryOpKind.I32_Truncate_F64_S: ty = typeof(Op_I32_Truncate_F64_S<>); break;
            case UnaryOpKind.I32_Truncate_F64_U: ty = typeof(Op_I32_Truncate_F64_U<>); break;
            case UnaryOpKind.I32_TruncateSat_F32_S: ty = typeof(Op_I32_TruncateSat_F32_S<>); break;
            case UnaryOpKind.I32_TruncateSat_F32_U: ty = typeof(Op_I32_TruncateSat_F32_U<>); break;
            case UnaryOpKind.I32_TruncateSat_F64_S: ty = typeof(Op_I32_TruncateSat_F64_S<>); break;
            case UnaryOpKind.I32_TruncateSat_F64_U: ty = typeof(Op_I32_TruncateSat_F64_U<>); break;
            case UnaryOpKind.I32_Reinterpret_F32: ty = typeof(Op_I32_Reinterpret_F32<>); break;

            case UnaryOpKind.I64_Extend_I32_S: ty = typeof(Op_I64_Extend_I32_S<>); break;
            case UnaryOpKind.I64_Extend_I32_U: ty = typeof(Op_I64_Extend_I32_U<>); break;
            case UnaryOpKind.I64_Truncate_F32_S: ty = typeof(Op_I64_Truncate_F32_S<>); break;
            case UnaryOpKind.I64_Truncate_F32_U: ty = typeof(Op_I64_Truncate_F32_U<>); break;
            case UnaryOpKind.I64_Truncate_F64_S: ty = typeof(Op_I64_Truncate_F64_S<>); break;
            case UnaryOpKind.I64_Truncate_F64_U: ty = typeof(Op_I64_Truncate_F64_U<>); break;
            case UnaryOpKind.I64_TruncateSat_F32_S: ty = typeof(Op_I64_TruncateSat_F32_S<>); break;
            case UnaryOpKind.I64_TruncateSat_F32_U: ty = typeof(Op_I64_TruncateSat_F32_U<>); break;
            case UnaryOpKind.I64_TruncateSat_F64_S: ty = typeof(Op_I64_TruncateSat_F64_S<>); break;
            case UnaryOpKind.I64_TruncateSat_F64_U: ty = typeof(Op_I64_TruncateSat_F64_U<>); break;
            case UnaryOpKind.I64_Reinterpret_F64: ty = typeof(Op_I64_Reinterpret_F64<>); break;

            default:
                throw new Exception("todo build unary: "+Kind);
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

    public override void Traverse(Action<Expression> f)
    {
        f(this);
        Cond.Traverse(f);
        A.Traverse(f);
        B.Traverse(f);
    }

    public override Type BuildHell()
    {
        var cond = Cond.BuildHell();
        var a = A.BuildHell();
        var b = B.BuildHell();

        var arg_ty = Type switch {
            ValType.I32 => typeof(int),
            ValType.I64 => typeof(long),
            ValType.F32 => typeof(float),
            ValType.F64 => typeof(double),
            _ => throw new Exception("select "+Type)
        };

        return HellBuilder.MakeGeneric(typeof(Select<,,,>),[cond,a,b,arg_ty]);
    }
}

class MemorySize : Expression {
    public MemorySize() : base(ValType.I32) {}

    public override void Traverse(Action<Expression> f)
    {
        f(this);
    }

    public override Type BuildHell()
    {
        return typeof(Memory_GetSize);
    }
}

class MemoryGrow : Expression {
    Expression Arg;

    public MemoryGrow(Expression arg) : base(ValType.I32) {
        Arg = arg;
    }

    public override void Traverse(Action<Expression> f)
    {
        Arg.Traverse(f);
        f(this);
    }

    public override Type BuildHell()
    {
        var arg = Arg.BuildHell();
        return HellBuilder.MakeGeneric(typeof(Memory_Grow<>), [arg]);
    }
}

class Call : Expression
{
    public int FunctionIndex;
    public int FrameIndex;
    List<Expression> Args;
    string DebugName;

    public Call(int func_index, int frame_index, List<Expression> args, string debug_name) :
        base(ValType.I64)
    {
        FunctionIndex = func_index;
        FrameIndex = frame_index;
        DebugName = debug_name;
        args.Reverse();
        Args = args;
    }

    public override Type BuildHell()
    {
        var func_index = HellBuilder.MakeConstant(FunctionIndex);
        var frame_index = HellBuilder.MakeConstant(FrameIndex);
        var args = typeof(ArgWriteNone);

        for (int i=0;i<Args.Count;i++) {
            var arg = Args[i];
            var writer = arg.Type switch {
                ValType.I32 => typeof(ArgWriteI32<,,>),
                ValType.I64 => typeof(ArgWriteI64<,,>),
                ValType.F32 => typeof(ArgWriteF32<,,>),
                ValType.F64 => typeof(ArgWriteF64<,,>),
                _ => throw new Exception("todo arg ty "+arg.Type)
            };
            args = HellBuilder.MakeGeneric(writer,[
                arg.BuildHell(),
                HellBuilder.MakeConstant(FrameIndex + i),
                args
            ]);
        }

        return HellBuilder.MakeGeneric(typeof(StaticCall<,,>),[func_index,frame_index,args]);
    }

    public override void Traverse(Action<Expression> f)
    {
        f(this);
        for (int i=0;i<Args.Count;i++) {
            Args[i].Traverse(f);
        }
    }

    public override string ToString()
    {
        string debug_name = DebugName ?? FunctionIndex.ToString();
        string res = "@"+debug_name+"["+FrameIndex+"](";
        for (int i=0;i<Args.Count;i++) {
            if (i != 0) {
                res += ", ";
            }
            res += Args[i];
        }
        return res+")";
    }
}

class CallIndirect : Expression {
    Expression FunctionIndex;
    public int FrameIndex;
    List<Expression> Args;
    int SigId;
    int TableIndex;

    public CallIndirect(Expression func_index, int frame_index, List<Expression> args, int sig_id, int table_index) :
        base(ValType.I64)
    {
        FunctionIndex = func_index;
        FrameIndex = frame_index;
        args.Reverse();
        Args = args;
        SigId = sig_id;
        TableIndex = table_index;
    }

    public override void Traverse(Action<Expression> f)
    {
        f(this);
        FunctionIndex.Traverse(f);
        for (int i=0;i<Args.Count;i++) {
            Args[i].Traverse(f);
        }
    }

    public override Type BuildHell()
    {
        var func_index = FunctionIndex.BuildHell();
        var frame_index = HellBuilder.MakeConstant(FrameIndex);
        var table_index = HellBuilder.MakeConstant(TableIndex);
        var sig_id = HellBuilder.MakeConstant(SigId);
        var args = typeof(ArgWriteNone);

        for (int i=0;i<Args.Count;i++) {
            var arg = Args[i];
            var writer = arg.Type switch {
                ValType.I32 => typeof(ArgWriteI32<,,>),
                ValType.I64 => typeof(ArgWriteI64<,,>),
                ValType.F32 => typeof(ArgWriteF32<,,>),
                ValType.F64 => typeof(ArgWriteF64<,,>),
                _ => throw new Exception("todo arg ty "+arg.Type)
            };
            args = HellBuilder.MakeGeneric(writer,[
                arg.BuildHell(),
                HellBuilder.MakeConstant(FrameIndex + i),
                args
            ]);
        }

        return HellBuilder.MakeGeneric(typeof(DynamicCall<,,,,>),[func_index,table_index,frame_index,sig_id,args]);
    }

    public override string ToString()
    {
        string debug_name = FunctionIndex.ToString();
        string res = "@["+debug_name+"]["+FrameIndex+"](";
        for (int i=0;i<Args.Count;i++) {
            if (i != 0) {
                res += ", ";
            }
            res += Args[i];
        }
        return res+")";
    }
}

enum MemSize {
    I8_U,
    I16_U,
    I32_U,
    I64_U,
    I8_S,
    I16_S,
    I32_S,
    I64_S,

    SAME
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

    F32_Equal,
    F32_NotEqual,
    F32_Less,
    F32_Greater,
    F32_LessEqual,
    F32_GreaterEqual,

    F64_Equal,
    F64_NotEqual,
    F64_Less,
    F64_Greater,
    F64_LessEqual,
    F64_GreaterEqual,

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

    LAST_I32,

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
    I64_RotateRight,

    LAST_I64,

    F32_Add = 0x92,
    F32_Sub,
    F32_Mul,
    F32_Div,
    F32_Min,
    F32_Max,
    F32_CopySign,

    LAST_F32,

    F64_Add = 0xA0,
    F64_Sub,
    F64_Mul,
    F64_Div,
    F64_Min,
    F64_Max,
    F64_CopySign
}

enum UnaryOpKind {
    // comparisons return i32
    I32_EqualZero = 0x45,
    I64_EqualZero = 0x50,

    I32_LeadingZeros = 0x67,
    I32_TrailingZeros,
    I32_PopCount,

    I64_LeadingZeros = 0x79,
    I64_TrailingZeros,
    I64_PopCount,

    F32_Abs = 0x8B,
    F32_Neg,
    F32_Ceil,
    F32_Floor,
    F32_Truncate,
    F32_Nearest,
    F32_Sqrt,

    F64_Abs = 0x99,
    F64_Neg,
    F64_Ceil,
    F64_Floor,
    F64_Truncate,
    F64_Nearest,
    F64_Sqrt,

    I32_Wrap_I64 = 0xA7,
    I32_Truncate_F32_S,
    I32_Truncate_F32_U,
    I32_Truncate_F64_S,
    I32_Truncate_F64_U,

    I64_Extend_I32_S,
    I64_Extend_I32_U,
    I64_Truncate_F32_S,
    I64_Truncate_F32_U,
    I64_Truncate_F64_S,
    I64_Truncate_F64_U,

    F32_Convert_I32_S,
    F32_Convert_I32_U,
    F32_Convert_I64_S,
    F32_Convert_I64_U,
    F32_Demote_F64,

    F64_Convert_I32_S,
    F64_Convert_I32_U,
    F64_Convert_I64_S,
    F64_Convert_I64_U,
    F64_Promote_F32,

    I32_Reinterpret_F32,
    I64_Reinterpret_F64,
    F32_Reinterpret_I32,
    F64_Reinterpret_I64,

    I32_Extend8_S,
    I32_Extend16_S,
    I64_Extend8_S,
    I64_Extend16_S,
    I64_Extend32_S,
    
    // FC extensions
    I32_TruncateSat_F32_S = 0x100,
    I32_TruncateSat_F32_U,
    I32_TruncateSat_F64_S,
    I32_TruncateSat_F64_U,
    I64_TruncateSat_F32_S,
    I64_TruncateSat_F32_U,
    I64_TruncateSat_F64_S,
    I64_TruncateSat_F64_U,
}
