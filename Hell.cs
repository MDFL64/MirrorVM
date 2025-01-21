using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

interface Op {
    int Run(int index, byte[] data);
}

interface Const {
    int Run();
}

struct D0 : Const { public int Run() => 0; }
struct D1 : Const { public int Run() => 1; }
struct D2 : Const { public int Run() => 2; }
struct D3 : Const { public int Run() => 3; }
struct D4 : Const { public int Run() => 4; }
struct D5 : Const { public int Run() => 5; }
struct D6 : Const { public int Run() => 6; }
struct D7 : Const { public int Run() => 7; }
struct D8 : Const { public int Run() => 8; }
struct D9 : Const { public int Run() => 9; }
struct DA : Const { public int Run() => 0xA; }
struct DB : Const { public int Run() => 0xB; }
struct DC : Const { public int Run() => 0xC; }
struct DD : Const { public int Run() => 0xD; }
struct DE : Const { public int Run() => 0xE; }
struct DF : Const { public int Run() => 0xF; }

struct Num<A,B> : Const
    where A: struct, Const
    where B: struct, Const
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int Run() {
        return default(A).Run()<<4 | default(B).Run();
    }
}

struct Neg<A> : Const
    where A: struct, Const
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int Run() {
        return -default(A).Run();
    }
}

struct Stop : Op
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int Run(int index, byte[] data)
    {
        return index;
    }
}

struct DumpCode<NEXT> : Op
    where NEXT: struct, Op
{
    static int COUNT = 0;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int Run(int index, byte[] data)
    {
        COUNT += 1;
        if (COUNT == 100_000) {
            Chungus.Dump();
        }
        return default(NEXT).Run(index, data);
    }
}

struct UpdateCell<OFFSET,INC,NEXT> : Op
    where OFFSET: struct, Const
    where INC: struct, Const
    where NEXT: struct, Op
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int Run(int index, byte[] data)
    {
        data[index + default(OFFSET).Run()] += (byte)default(INC).Run();
        return default(NEXT).Run(index, data);
    }
}

struct ZeroCell<OFFSET,NEXT> : Op
    where OFFSET: struct, Const
    where NEXT: struct, Op
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int Run(int index, byte[] data)
    {
        data[index + default(OFFSET).Run()] = 0;
        return default(NEXT).Run(index, data);
    }
}

struct OutputCell<OFFSET,NEXT> : Op
    where OFFSET: struct, Const
    where NEXT: struct, Op
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int Run(int index, byte[] data)
    {
        Console.Write((char)data[index + default(OFFSET).Run()]);
        return default(NEXT).Run(index, data);
    }
}

struct UpdatePointer<OFFSET,NEXT> : Op
    where OFFSET: struct, Const
    where NEXT: struct, Op
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int Run(int index, byte[] data)
    {
        index += default(OFFSET).Run();
        return default(NEXT).Run(index, data);
    }
}

struct Loop<BODY,NEXT> : Op
    where BODY: struct, Op
    where NEXT: struct, Op
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int Run(int index, byte[] data)
    {
        var body = default(BODY);
        while (data[index] != 0) {
            index = body.Run(index, data);
        }
        return default(NEXT).Run(index, data);
    }
}
