using System.Runtime.CompilerServices;

// I don't think canonicalization is worth supporting,
// but uncommenting these might do the trick
class FloatHelper {
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static float CanonicalF32(float a) {
        //return float.IsNaN(a) ? float.NaN : a;
        return a;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static double CanonicalF64(double a) {
        //return double.IsNaN(a) ? double.NaN : a;
        return a;
    }
}

class Config {
    public const bool USE_REGISTERS = false;

    public static int GetRegisterCount() {
        if (USE_REGISTERS) {
            return 7;
        } else {
            return 0;
        }
    }
}
