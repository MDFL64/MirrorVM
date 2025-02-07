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
