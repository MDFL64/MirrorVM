
using System.Runtime.CompilerServices;

struct Memory_GetSize : Expr<int>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public int Run(Registers reg, Span<long> frame, WasmInstance inst) => inst.Memory.Length / 65536;
}

// operations that work on individual bytes are written slightly differently,
// since indexing actually works with longs, but the AsSpan method does not

// i32 loads
struct Memory_I32_Load<ADDR,OFFSET> : Expr<int> where ADDR: struct, Expr<int> where OFFSET: struct, Const
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public int Run(Registers reg, Span<long> frame, WasmInstance inst) {
        uint addr = (uint)default(ADDR).Run(reg, frame, inst);
        uint offset = (uint)default(OFFSET).Run();
        return BitConverter.ToInt32(inst.Memory.AsSpan((int)checked(addr + offset)));
    }
}
struct Memory_I32_Load8_S<ADDR,OFFSET> : Expr<int> where ADDR: struct, Expr<int> where OFFSET: struct, Const
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public int Run(Registers reg, Span<long> frame, WasmInstance inst) {
        long addr = (uint)default(ADDR).Run(reg, frame, inst);
        long offset = (uint)default(OFFSET).Run();
        return (sbyte)inst.Memory[addr + offset];
    }
}
struct Memory_I32_Load8_U<ADDR,OFFSET> : Expr<int> where ADDR: struct, Expr<int> where OFFSET: struct, Const
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public int Run(Registers reg, Span<long> frame, WasmInstance inst) {
        long addr = (uint)default(ADDR).Run(reg, frame, inst);
        long offset = (uint)default(OFFSET).Run();
        return inst.Memory[addr + offset];
    }
}
struct Memory_I32_Load16_S<ADDR,OFFSET> : Expr<int> where ADDR: struct, Expr<int> where OFFSET: struct, Const
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public int Run(Registers reg, Span<long> frame, WasmInstance inst) {
        uint addr = (uint)default(ADDR).Run(reg, frame, inst);
        uint offset = (uint)default(OFFSET).Run();
        return BitConverter.ToInt16(inst.Memory.AsSpan((int)checked(addr + offset)));
    }
}
struct Memory_I32_Load16_U<ADDR,OFFSET> : Expr<int> where ADDR: struct, Expr<int> where OFFSET: struct, Const
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public int Run(Registers reg, Span<long> frame, WasmInstance inst) {
        uint addr = (uint)default(ADDR).Run(reg, frame, inst);
        uint offset = (uint)default(OFFSET).Run();
        return BitConverter.ToUInt16(inst.Memory.AsSpan((int)checked(addr + offset)));
    }
}

// i64 loads
struct Memory_I64_Load<ADDR,OFFSET> : Expr<long> where ADDR: struct, Expr<int> where OFFSET: struct, Const
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public long Run(Registers reg, Span<long> frame, WasmInstance inst) {
        uint addr = (uint)default(ADDR).Run(reg, frame, inst);
        uint offset = (uint)default(OFFSET).Run();
        return BitConverter.ToInt64(inst.Memory.AsSpan((int)checked(addr + offset)));
    }
}
struct Memory_I64_Load8_S<ADDR,OFFSET> : Expr<long> where ADDR: struct, Expr<int> where OFFSET: struct, Const
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public long Run(Registers reg, Span<long> frame, WasmInstance inst) {
        long addr = (uint)default(ADDR).Run(reg, frame, inst);
        long offset = (uint)default(OFFSET).Run();
        return (sbyte)inst.Memory[addr + offset];
    }
}
struct Memory_I64_Load8_U<ADDR,OFFSET> : Expr<long> where ADDR: struct, Expr<int> where OFFSET: struct, Const
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public long Run(Registers reg, Span<long> frame, WasmInstance inst) {
        long addr = (uint)default(ADDR).Run(reg, frame, inst);
        long offset = (uint)default(OFFSET).Run();
        return inst.Memory[addr + offset];
    }
}
struct Memory_I64_Load16_S<ADDR,OFFSET> : Expr<long> where ADDR: struct, Expr<int> where OFFSET: struct, Const
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public long Run(Registers reg, Span<long> frame, WasmInstance inst) {
        uint addr = (uint)default(ADDR).Run(reg, frame, inst);
        uint offset = (uint)default(OFFSET).Run();
        return BitConverter.ToInt16(inst.Memory.AsSpan((int)checked(addr + offset)));
    }
}
struct Memory_I64_Load16_U<ADDR,OFFSET> : Expr<long> where ADDR: struct, Expr<int> where OFFSET: struct, Const
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public long Run(Registers reg, Span<long> frame, WasmInstance inst) {
        uint addr = (uint)default(ADDR).Run(reg, frame, inst);
        uint offset = (uint)default(OFFSET).Run();
        return BitConverter.ToUInt16(inst.Memory.AsSpan((int)checked(addr + offset)));
    }
}
struct Memory_I64_Load32_S<ADDR,OFFSET> : Expr<long> where ADDR: struct, Expr<int> where OFFSET: struct, Const
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public long Run(Registers reg, Span<long> frame, WasmInstance inst) {
        uint addr = (uint)default(ADDR).Run(reg, frame, inst);
        uint offset = (uint)default(OFFSET).Run();
        return BitConverter.ToInt32(inst.Memory.AsSpan((int)checked(addr + offset)));
    }
}
struct Memory_I64_Load32_U<ADDR,OFFSET> : Expr<long> where ADDR: struct, Expr<int> where OFFSET: struct, Const
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public long Run(Registers reg, Span<long> frame, WasmInstance inst) {
        uint addr = (uint)default(ADDR).Run(reg, frame, inst);
        uint offset = (uint)default(OFFSET).Run();
        return BitConverter.ToUInt32(inst.Memory.AsSpan((int)checked(addr + offset)));
    }
}

struct Memory_F64_Load<ADDR,OFFSET> : Expr<double> where ADDR: struct, Expr<int> where OFFSET: struct, Const
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public double Run(Registers reg, Span<long> frame, WasmInstance inst) {
        uint addr = (uint)default(ADDR).Run(reg, frame, inst);
        uint offset = (uint)default(OFFSET).Run();
        return BitConverter.ToDouble(inst.Memory.AsSpan((int)checked(addr + offset)));
    }
}

struct Memory_F32_Load<ADDR,OFFSET> : Expr<float> where ADDR: struct, Expr<int> where OFFSET: struct, Const
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public float Run(Registers reg, Span<long> frame, WasmInstance inst) {
        uint addr = (uint)default(ADDR).Run(reg, frame, inst);
        uint offset = (uint)default(OFFSET).Run();
        return BitConverter.ToSingle(inst.Memory.AsSpan((int)checked(addr + offset)));
    }
}

// store

// i32 stores
struct Memory_I32_Store<VALUE,ADDR,OFFSET,NEXT> : Stmt
    where VALUE: struct, Expr<int>
    where ADDR: struct, Expr<int>
    where OFFSET: struct, Const
    where NEXT: struct, Stmt
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public Registers Run(Registers reg, Span<long> frame, WasmInstance inst) {
        int value = default(VALUE).Run(reg, frame, inst);
        uint addr = (uint)default(ADDR).Run(reg, frame, inst);
        uint offset = (uint)default(OFFSET).Run();
        BitConverter.TryWriteBytes(inst.Memory.AsSpan((int)checked(addr + offset)), value);
        return default(NEXT).Run(reg, frame, inst);
    }
}
struct Memory_I32_Store8<VALUE,ADDR,OFFSET,NEXT> : Stmt
    where VALUE: struct, Expr<int>
    where ADDR: struct, Expr<int>
    where OFFSET: struct, Const
    where NEXT: struct, Stmt
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public Registers Run(Registers reg, Span<long> frame, WasmInstance inst) {
        byte value = (byte)default(VALUE).Run(reg, frame, inst);
        long addr = (uint)default(ADDR).Run(reg, frame, inst);
        long offset = (uint)default(OFFSET).Run();
        inst.Memory[addr + offset] = value;
        return default(NEXT).Run(reg, frame, inst);
    }
}
struct Memory_I32_Store16<VALUE,ADDR,OFFSET,NEXT> : Stmt
    where VALUE: struct, Expr<int>
    where ADDR: struct, Expr<int>
    where OFFSET: struct, Const
    where NEXT: struct, Stmt
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public Registers Run(Registers reg, Span<long> frame, WasmInstance inst) {
        short value = (short)default(VALUE).Run(reg, frame, inst);
        uint addr = (uint)default(ADDR).Run(reg, frame, inst);
        uint offset = (uint)default(OFFSET).Run();
        BitConverter.TryWriteBytes(inst.Memory.AsSpan((int)checked(addr + offset)), value);
        return default(NEXT).Run(reg, frame, inst);
    }
}

// i64 stores
struct Memory_I64_Store<VALUE,ADDR,OFFSET,NEXT> : Stmt
    where VALUE: struct, Expr<long>
    where ADDR: struct, Expr<int>
    where OFFSET: struct, Const
    where NEXT: struct, Stmt
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public Registers Run(Registers reg, Span<long> frame, WasmInstance inst) {
        long value = default(VALUE).Run(reg, frame, inst);
        uint addr = (uint)default(ADDR).Run(reg, frame, inst);
        uint offset = (uint)default(OFFSET).Run();
        if (!BitConverter.TryWriteBytes(inst.Memory.AsSpan((int)checked(addr + offset)), value)) {
            throw new IndexOutOfRangeException();
        }
        return default(NEXT).Run(reg, frame, inst);
    }
}
struct Memory_I64_Store8<VALUE,ADDR,OFFSET,NEXT> : Stmt
    where VALUE: struct, Expr<long>
    where ADDR: struct, Expr<int>
    where OFFSET: struct, Const
    where NEXT: struct, Stmt
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public Registers Run(Registers reg, Span<long> frame, WasmInstance inst) {
        byte value = (byte)default(VALUE).Run(reg, frame, inst);
        long addr = (uint)default(ADDR).Run(reg, frame, inst);
        long offset = (uint)default(OFFSET).Run();
        inst.Memory[addr + offset] = value;
        return default(NEXT).Run(reg, frame, inst);
    }
}
struct Memory_I64_Store16<VALUE,ADDR,OFFSET,NEXT> : Stmt
    where VALUE: struct, Expr<long>
    where ADDR: struct, Expr<int>
    where OFFSET: struct, Const
    where NEXT: struct, Stmt
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public Registers Run(Registers reg, Span<long> frame, WasmInstance inst) {
        short value = (short)default(VALUE).Run(reg, frame, inst);
        uint addr = (uint)default(ADDR).Run(reg, frame, inst);
        uint offset = (uint)default(OFFSET).Run();
        if (!BitConverter.TryWriteBytes(inst.Memory.AsSpan((int)checked(addr + offset)), value)) {
            throw new IndexOutOfRangeException();
        }
        return default(NEXT).Run(reg, frame, inst);
    }
}
struct Memory_I64_Store32<VALUE,ADDR,OFFSET,NEXT> : Stmt
    where VALUE: struct, Expr<long>
    where ADDR: struct, Expr<int>
    where OFFSET: struct, Const
    where NEXT: struct, Stmt
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public Registers Run(Registers reg, Span<long> frame, WasmInstance inst) {
        int value = (int)default(VALUE).Run(reg, frame, inst);
        uint addr = (uint)default(ADDR).Run(reg, frame, inst);
        uint offset = (uint)default(OFFSET).Run();
        if (!BitConverter.TryWriteBytes(inst.Memory.AsSpan((int)checked(addr + offset)), value)) {
            throw new IndexOutOfRangeException();
        }
        return default(NEXT).Run(reg, frame, inst);
    }
}

// float stores
struct Memory_F32_Store<VALUE,ADDR,OFFSET,NEXT> : Stmt
    where VALUE: struct, Expr<float>
    where ADDR: struct, Expr<int>
    where OFFSET: struct, Const
    where NEXT: struct, Stmt
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public Registers Run(Registers reg, Span<long> frame, WasmInstance inst) {
        float value = default(VALUE).Run(reg, frame, inst);
        uint addr = (uint)default(ADDR).Run(reg, frame, inst);
        uint offset = (uint)default(OFFSET).Run();
        if (!BitConverter.TryWriteBytes(inst.Memory.AsSpan((int)checked(addr + offset)), value)) {
            throw new IndexOutOfRangeException();
        }
        return default(NEXT).Run(reg, frame, inst);
    }
}
struct Memory_F64_Store<VALUE,ADDR,OFFSET,NEXT> : Stmt
    where VALUE: struct, Expr<double>
    where ADDR: struct, Expr<int>
    where OFFSET: struct, Const
    where NEXT: struct, Stmt
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public Registers Run(Registers reg, Span<long> frame, WasmInstance inst) {
        double value = default(VALUE).Run(reg, frame, inst);
        uint addr = (uint)default(ADDR).Run(reg, frame, inst);
        uint offset = (uint)default(OFFSET).Run();
        if (!BitConverter.TryWriteBytes(inst.Memory.AsSpan((int)checked(addr + offset)), value)) {
            throw new IndexOutOfRangeException();
        }
        return default(NEXT).Run(reg, frame, inst);
    }
}
