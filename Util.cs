using System.Runtime.CompilerServices;

class FloatHelper {
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static float CanonicalF32(float a) {
        return float.IsNaN(a) ? float.NaN : a;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static double CanonicalF64(double a) {
        return double.IsNaN(a) ? double.NaN : a;
    }
}

public class Foo {
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization | MethodImplOptions.NoInlining)]
    public static long Bar() {
        var array = new byte[100];
        
        BitConverter.TryWriteBytes(array.AsSpan(16), (int)0xDEAD);
        BitConverter.TryWriteBytes(array.AsSpan(20), (int)0xBEEF);
        BitConverter.TryWriteBytes(array.AsSpan(24), (int)0xF00D);
        
        long a = BitConverter.ToInt32(array.AsSpan(16));
        long b = BitConverter.ToInt32(array.AsSpan(24));
        
        return a + b;
    }
}
