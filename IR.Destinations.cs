public abstract class Destination : Expression {
    public Destination(ValType ty) : base(ty) {

    }

    public abstract Type BuildDestination(Type input, Type next);
}

enum LocalKind {
    // front-end kinds
    Variable, // front-end wasm variables
    Spill,    // spills from the virtual stack, inserted before barriers
    Return,   // additional return values, placed at the start of frame
    Call,     // call arguments and extra return values, placed at end of frame
    // back-end kinds
    Register, // locals that live in registers
    Frame,    // locals that live in the frame
}

class Local : Destination {
    int Index;
    LocalKind Kind;

    public Local(int index, ValType ty, LocalKind kind) : base(ty) {
        Index = index;
        Kind = kind;
    }

    public Local WithType(ValType ty) {
        return new Local(Index, ty, Kind);
    }

    public override string ToString()
    {
        switch (Kind) {
            case LocalKind.Variable: return "V"+Index;
            case LocalKind.Spill:    return "S"+Index;
            case LocalKind.Call:     return "C"+Index;
            default: return Kind+"_"+Index;
        }
    }

    public override Expression Traverse(Func<Expression, Expression> f)
    {
        return f(this);
    }

    public override Type BuildHell() {
        if (Type == ValType.I32) {
            switch (Index) {
                case 0: return typeof(GetR0_I32);
                case 1: return typeof(GetR1_I32);
                case 2: return typeof(GetR2_I32);
                case 3: return typeof(GetR3_I32);
                case 4: return typeof(GetR4_I32);
                case 5: return typeof(GetR5_I32);
                case 6: return typeof(GetR6_I32);
                default: throw new Exception("register-get out of bounds");
            }
        } else if (Type == ValType.I64) {
            switch (Index) {
                case 0: return typeof(GetR0_I64);
                case 1: return typeof(GetR1_I64);
                case 2: return typeof(GetR2_I64);
                case 3: return typeof(GetR3_I64);
                case 4: return typeof(GetR4_I64);
                case 5: return typeof(GetR5_I64);
                case 6: return typeof(GetR6_I64);
                default: throw new Exception("register-get out of bounds");
            }
        } else if (Type == ValType.F32) {
            switch (Index) {
                case 0: return typeof(GetR0_F32);
                case 1: return typeof(GetR1_F32);
                case 2: return typeof(GetR2_F32);
                case 3: return typeof(GetR3_F32);
                case 4: return typeof(GetR4_F32);
                case 5: return typeof(GetR5_F32);
                case 6: return typeof(GetR6_F32);
                default: throw new Exception("register-get out of bounds");
            }
        } else if (Type == ValType.F64) {
            switch (Index) {
                case 0: return typeof(GetR0_F64);
                case 1: return typeof(GetR1_F64);
                case 2: return typeof(GetR2_F64);
                case 3: return typeof(GetR3_F64);
                case 4: return typeof(GetR4_F64);
                case 5: return typeof(GetR5_F64);
                case 6: return typeof(GetR6_F64);
                default: throw new Exception("register-get out of bounds");
            }
        } else {
            throw new Exception("todo local type: "+Type);
        }
    }

    public override Type BuildDestination(Type input, Type next) {
        Type base_ty;
        if (Type == ValType.I32) {
            switch (Index) {
                case 0: base_ty = typeof(SetR0_I32<,>); break;
                case 1: base_ty = typeof(SetR1_I32<,>); break;
                case 2: base_ty = typeof(SetR2_I32<,>); break;
                case 3: base_ty = typeof(SetR3_I32<,>); break;
                case 4: base_ty = typeof(SetR4_I32<,>); break;
                case 5: base_ty = typeof(SetR5_I32<,>); break;
                case 6: base_ty = typeof(SetR6_I32<,>); break;

                default: throw new Exception("register-set out of bounds");
            }
        } else if (Type == ValType.I64) {
            switch (Index) {
                case 0: base_ty = typeof(SetR0_I64<,>); break;
                case 1: base_ty = typeof(SetR1_I64<,>); break;
                case 2: base_ty = typeof(SetR2_I64<,>); break;
                case 3: base_ty = typeof(SetR3_I64<,>); break;
                case 4: base_ty = typeof(SetR4_I64<,>); break;
                case 5: base_ty = typeof(SetR5_I64<,>); break;
                case 6: base_ty = typeof(SetR6_I64<,>); break;

                default: throw new Exception("register-set out of bounds");
            }
        } else if (Type == ValType.F32) {
            switch (Index) {
                case 0: base_ty = typeof(SetR0_F32<,>); break;
                case 1: base_ty = typeof(SetR1_F32<,>); break;
                case 2: base_ty = typeof(SetR2_F32<,>); break;
                case 3: base_ty = typeof(SetR3_F32<,>); break;
                case 4: base_ty = typeof(SetR4_F32<,>); break;
                case 5: base_ty = typeof(SetR5_F32<,>); break;
                case 6: base_ty = typeof(SetR6_F32<,>); break;

                default: throw new Exception("register-set out of bounds");
            }
        } else {
            throw new Exception("todo locals-set "+Type);
        }
        return HellBuilder.MakeGeneric(base_ty,[input,next]);
    }
}

class MemoryOp : Destination {
    private MemSize Size;
    private int Offset;
    public Expression Addr;

    public MemoryOp(ValType ty, MemSize size, Expression addr, int offset) : base(ty) {
        Addr = addr;
        Size = size;
        Offset = offset;
    }

    public override Type BuildDestination(Type input, Type next)
    {
        Type base_ty = (Type,Size) switch {
            (ValType.I64,MemSize.SAME) => typeof(Memory_I64_Store<,,,>),
            
            (ValType.I32,MemSize.I8_S) => typeof(Memory_I32_Store8<,,,>),
            (ValType.I32,MemSize.I16_S) => typeof(Memory_I32_Store16<,,,>),
            
            (ValType.I64,MemSize.I8_S) => typeof(Memory_I64_Store8<,,,>),
            (ValType.I64,MemSize.I16_S) => typeof(Memory_I64_Store16<,,,>),
            (ValType.I64,MemSize.I32_S) => typeof(Memory_I64_Store32<,,,>),

            _ => throw new Exception("WRITE "+Type+" "+Size)
        };
        return HellBuilder.MakeGeneric(base_ty,[
            input,
            Addr.BuildHell(),
            HellBuilder.MakeConstant(Offset),
            next
        ]);
    }

    public override Expression Traverse(Func<Expression, Expression> f)
    {
        var res = f(this);
        if (res != this) {
            return res;
        }
        Addr = Addr.Traverse(f);
        return this;
    }

    public override Type BuildHell()
    {
        Type base_ty = (Type,Size) switch {
            (ValType.I32,MemSize.SAME) => typeof(Memory_I32_Load<,>),
            (ValType.I32,MemSize.I8_S) => typeof(Memory_I32_Load8_S<,>),
            (ValType.I32,MemSize.I8_U) => typeof(Memory_I32_Load8_U<,>),
            (ValType.I32,MemSize.I16_S) => typeof(Memory_I32_Load16_S<,>),
            (ValType.I32,MemSize.I16_U) => typeof(Memory_I32_Load16_U<,>),

            (ValType.I64,MemSize.SAME) => typeof(Memory_I64_Load<,>),
            (ValType.I64,MemSize.I8_S) => typeof(Memory_I64_Load8_S<,>),
            (ValType.I64,MemSize.I8_U) => typeof(Memory_I64_Load8_U<,>),
            (ValType.I64,MemSize.I16_S) => typeof(Memory_I64_Load16_S<,>),
            (ValType.I64,MemSize.I16_U) => typeof(Memory_I64_Load16_U<,>),
            (ValType.I64,MemSize.I32_S) => typeof(Memory_I64_Load32_S<,>),
            (ValType.I64,MemSize.I32_U) => typeof(Memory_I64_Load32_U<,>),

            (ValType.F64,MemSize.SAME) => typeof(Memory_F64_Load<,>),
            (ValType.F32,MemSize.SAME) => typeof(Memory_F32_Load<,>),
            _ => throw new Exception("READ "+Type+" "+Size)
        };
        return HellBuilder.MakeGeneric(base_ty,[
            Addr.BuildHell(),
            HellBuilder.MakeConstant(Offset)
        ]);
    }

    public override string ToString()
    {
        string ty_name = Type.ToString();
        if (Size != MemSize.SAME) {
            ty_name += "_"+Size.ToString();
        }
        if (Offset != 0) {
            return "M_"+ty_name+"["+Addr.ToString()+" + "+Offset+"]";
        } else {
            return "M_"+ty_name+"["+Addr.ToString()+"]";
        }
    }
}
