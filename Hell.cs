using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

interface Op {
    int Run(int index, byte[] data, Output output);
}

struct Stop : Op
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public int Run(int index, byte[] data, Output output)
    {
        return index;
    }
}

struct Wrapper<NEXT> : Op
    where NEXT: struct, Op
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public int Run(int index, byte[] data, Output output)
    {
        return default(NEXT).Run(index, data, output);
    }
}

struct UpdateCell<OFFSET,INC,NEXT> : Op
    where OFFSET: struct, Const
    where INC: struct, Const
    where NEXT: struct, Op
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public int Run(int index, byte[] data, Output output)
    {
        data[index + default(OFFSET).Run()] += (byte)default(INC).Run();
        return default(NEXT).Run(index, data, output);
    }
}

struct ZeroCell<OFFSET,NEXT> : Op
    where OFFSET: struct, Const
    where NEXT: struct, Op
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public int Run(int index, byte[] data, Output output)
    {
        data[index + default(OFFSET).Run()] = 0;
        return default(NEXT).Run(index, data, output);
    }
}

struct OutputCell<OFFSET,NEXT> : Op
    where OFFSET: struct, Const
    where NEXT: struct, Op
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public int Run(int index, byte[] data, Output output)
    {
        output.WriteChar((char)data[index + default(OFFSET).Run()]);
        return default(NEXT).Run(index, data, output);
    }
}

struct UpdatePointer<OFFSET,NEXT> : Op
    where OFFSET: struct, Const
    where NEXT: struct, Op
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public int Run(int index, byte[] data, Output output)
    {
        index += default(OFFSET).Run();
        return default(NEXT).Run(index, data, output);
    }
}

struct Loop<BODY,NEXT> : Op
    where BODY: struct, Op
    where NEXT: struct, Op
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public int Run(int index, byte[] data, Output output)
    {
        var body = default(BODY);
        while (data[index] != 0) {
            output.CheckKill();
            index = body.Run(index, data, output);
        }
        return default(NEXT).Run(index, data, output);
    }
}
