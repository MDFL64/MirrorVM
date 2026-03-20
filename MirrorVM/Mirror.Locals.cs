using System;
using System.Runtime.CompilerServices;

namespace MirrorVM
{
    
    struct Registers
    {
        public const int COUNT = 7;
    
        public long R0;
        public long R1;
        public long R2;
        public long R3;
        public long R4;
        public long R5;
        public long R6;
    
        public int NextBlock;
    
        public static Type OpGetI32(int n) {
            return n switch {
                0 => typeof(GetR0_I32),
                1 => typeof(GetR1_I32),
                2 => typeof(GetR2_I32),
                3 => typeof(GetR3_I32),
                4 => typeof(GetR4_I32),
                5 => typeof(GetR5_I32),
                6 => typeof(GetR6_I32),
                _ => null
            };
        }
    
        public static Type OpGetI64(int n) {
            return n switch {
                0 => typeof(GetR0_I64),
                1 => typeof(GetR1_I64),
                2 => typeof(GetR2_I64),
                3 => typeof(GetR3_I64),
                4 => typeof(GetR4_I64),
                5 => typeof(GetR5_I64),
                6 => typeof(GetR6_I64),
                _ => null
            };
        }
    
        public static Type OpGetF32(int n) {
            return n switch {
                0 => typeof(GetR0_F32),
                1 => typeof(GetR1_F32),
                2 => typeof(GetR2_F32),
                3 => typeof(GetR3_F32),
                4 => typeof(GetR4_F32),
                5 => typeof(GetR5_F32),
                6 => typeof(GetR6_F32),
                _ => null
            };
        }
    
        public static Type OpGetF64(int n) {
            return n switch {
                0 => typeof(GetR0_F64),
                1 => typeof(GetR1_F64),
                2 => typeof(GetR2_F64),
                3 => typeof(GetR3_F64),
                4 => typeof(GetR4_F64),
                5 => typeof(GetR5_F64),
                6 => typeof(GetR6_F64),
                _ => null
            };
        }
    
        public static Type OpSetI32(int n) {
            return n switch {
                0 => typeof(SetR0_I32<>),
                1 => typeof(SetR1_I32<>),
                2 => typeof(SetR2_I32<>),
                3 => typeof(SetR3_I32<>),
                4 => typeof(SetR4_I32<>),
                5 => typeof(SetR5_I32<>),
                6 => typeof(SetR6_I32<>),
                _ => null
            };
        }
    
        public static Type OpSetI64(int n) {
            return n switch {
                0 => typeof(SetR0_I64<>),
                1 => typeof(SetR1_I64<>),
                2 => typeof(SetR2_I64<>),
                3 => typeof(SetR3_I64<>),
                4 => typeof(SetR4_I64<>),
                5 => typeof(SetR5_I64<>),
                6 => typeof(SetR6_I64<>),
                _ => null
            };
        }
    
        public static Type OpSetF32(int n) {
            return n switch {
                0 => typeof(SetR0_F32<>),
                1 => typeof(SetR1_F32<>),
                2 => typeof(SetR2_F32<>),
                3 => typeof(SetR3_F32<>),
                4 => typeof(SetR4_F32<>),
                5 => typeof(SetR5_F32<>),
                6 => typeof(SetR6_F32<>),
                _ => null
            };
        }
    
        public static Type OpSetF64(int n) {
            return n switch {
                0 => typeof(SetR0_F64<>),
                1 => typeof(SetR1_F64<>),
                2 => typeof(SetR2_F64<>),
                3 => typeof(SetR3_F64<>),
                4 => typeof(SetR4_F64<>),
                5 => typeof(SetR5_F64<>),
                6 => typeof(SetR6_F64<>),
                _ => null
            };
        }
    }
    
    
    
    struct GetR0_I32 : Expr<int> {
        [MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
        public int Run( ref Registers reg, Span<long> frame, WasmInstance inst ) => (int) reg.R0 ;
    }
    
    struct GetR0_I64 : Expr<long> {
        [MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
        public long Run( ref Registers reg, Span<long> frame, WasmInstance inst ) =>  reg.R0 ;
    }
    
    struct GetR0_F32 : Expr<float> {
        [MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
        public float Run( ref Registers reg, Span<long> frame, WasmInstance inst ) => BitConverter.Int32BitsToSingle( (int) reg.R0  );
    }
    
    struct GetR0_F64 : Expr<double> {
        [MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
        public double Run( ref Registers reg, Span<long> frame, WasmInstance inst ) => BitConverter.Int64BitsToDouble(  reg.R0  );
    }
    
    
    struct SetR0_I32<VALUE> : Stmt where VALUE : struct, Expr<int> {
        [MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
        public void Run( ref Registers reg, Span<long> frame, WasmInstance inst )
        {
            reg.R0 = (uint) default( VALUE ).Run( ref reg, frame, inst );
        }
    }
    
    struct SetR0_I64<VALUE> : Stmt where VALUE : struct, Expr<long> {
        [MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
        public void Run( ref Registers reg, Span<long> frame, WasmInstance inst )
        {
            reg.R0 =  default( VALUE ).Run( ref reg, frame, inst );
        }
    }
    
    struct SetR0_F32<VALUE> : Stmt where VALUE : struct, Expr<float> {
        [MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
        public void Run( ref Registers reg, Span<long> frame, WasmInstance inst )
        {
            reg.R0 = BitConverter.SingleToUInt32Bits(  default( VALUE ).Run( ref reg, frame, inst ) );
        }
    }
    
    struct SetR0_F64<VALUE> : Stmt where VALUE : struct, Expr<double> {
        [MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
        public void Run( ref Registers reg, Span<long> frame, WasmInstance inst )
        {
            reg.R0 = BitConverter.DoubleToInt64Bits(  default( VALUE ).Run( ref reg, frame, inst ) );
        }
    }
    
    struct GetR1_I32 : Expr<int> {
        [MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
        public int Run( ref Registers reg, Span<long> frame, WasmInstance inst ) => (int) reg.R1 ;
    }
    
    struct GetR1_I64 : Expr<long> {
        [MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
        public long Run( ref Registers reg, Span<long> frame, WasmInstance inst ) =>  reg.R1 ;
    }
    
    struct GetR1_F32 : Expr<float> {
        [MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
        public float Run( ref Registers reg, Span<long> frame, WasmInstance inst ) => BitConverter.Int32BitsToSingle( (int) reg.R1  );
    }
    
    struct GetR1_F64 : Expr<double> {
        [MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
        public double Run( ref Registers reg, Span<long> frame, WasmInstance inst ) => BitConverter.Int64BitsToDouble(  reg.R1  );
    }
    
    
    struct SetR1_I32<VALUE> : Stmt where VALUE : struct, Expr<int> {
        [MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
        public void Run( ref Registers reg, Span<long> frame, WasmInstance inst )
        {
            reg.R1 = (uint) default( VALUE ).Run( ref reg, frame, inst );
        }
    }
    
    struct SetR1_I64<VALUE> : Stmt where VALUE : struct, Expr<long> {
        [MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
        public void Run( ref Registers reg, Span<long> frame, WasmInstance inst )
        {
            reg.R1 =  default( VALUE ).Run( ref reg, frame, inst );
        }
    }
    
    struct SetR1_F32<VALUE> : Stmt where VALUE : struct, Expr<float> {
        [MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
        public void Run( ref Registers reg, Span<long> frame, WasmInstance inst )
        {
            reg.R1 = BitConverter.SingleToUInt32Bits(  default( VALUE ).Run( ref reg, frame, inst ) );
        }
    }
    
    struct SetR1_F64<VALUE> : Stmt where VALUE : struct, Expr<double> {
        [MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
        public void Run( ref Registers reg, Span<long> frame, WasmInstance inst )
        {
            reg.R1 = BitConverter.DoubleToInt64Bits(  default( VALUE ).Run( ref reg, frame, inst ) );
        }
    }
    
    struct GetR2_I32 : Expr<int> {
        [MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
        public int Run( ref Registers reg, Span<long> frame, WasmInstance inst ) => (int) reg.R2 ;
    }
    
    struct GetR2_I64 : Expr<long> {
        [MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
        public long Run( ref Registers reg, Span<long> frame, WasmInstance inst ) =>  reg.R2 ;
    }
    
    struct GetR2_F32 : Expr<float> {
        [MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
        public float Run( ref Registers reg, Span<long> frame, WasmInstance inst ) => BitConverter.Int32BitsToSingle( (int) reg.R2  );
    }
    
    struct GetR2_F64 : Expr<double> {
        [MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
        public double Run( ref Registers reg, Span<long> frame, WasmInstance inst ) => BitConverter.Int64BitsToDouble(  reg.R2  );
    }
    
    
    struct SetR2_I32<VALUE> : Stmt where VALUE : struct, Expr<int> {
        [MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
        public void Run( ref Registers reg, Span<long> frame, WasmInstance inst )
        {
            reg.R2 = (uint) default( VALUE ).Run( ref reg, frame, inst );
        }
    }
    
    struct SetR2_I64<VALUE> : Stmt where VALUE : struct, Expr<long> {
        [MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
        public void Run( ref Registers reg, Span<long> frame, WasmInstance inst )
        {
            reg.R2 =  default( VALUE ).Run( ref reg, frame, inst );
        }
    }
    
    struct SetR2_F32<VALUE> : Stmt where VALUE : struct, Expr<float> {
        [MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
        public void Run( ref Registers reg, Span<long> frame, WasmInstance inst )
        {
            reg.R2 = BitConverter.SingleToUInt32Bits(  default( VALUE ).Run( ref reg, frame, inst ) );
        }
    }
    
    struct SetR2_F64<VALUE> : Stmt where VALUE : struct, Expr<double> {
        [MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
        public void Run( ref Registers reg, Span<long> frame, WasmInstance inst )
        {
            reg.R2 = BitConverter.DoubleToInt64Bits(  default( VALUE ).Run( ref reg, frame, inst ) );
        }
    }
    
    struct GetR3_I32 : Expr<int> {
        [MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
        public int Run( ref Registers reg, Span<long> frame, WasmInstance inst ) => (int) reg.R3 ;
    }
    
    struct GetR3_I64 : Expr<long> {
        [MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
        public long Run( ref Registers reg, Span<long> frame, WasmInstance inst ) =>  reg.R3 ;
    }
    
    struct GetR3_F32 : Expr<float> {
        [MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
        public float Run( ref Registers reg, Span<long> frame, WasmInstance inst ) => BitConverter.Int32BitsToSingle( (int) reg.R3  );
    }
    
    struct GetR3_F64 : Expr<double> {
        [MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
        public double Run( ref Registers reg, Span<long> frame, WasmInstance inst ) => BitConverter.Int64BitsToDouble(  reg.R3  );
    }
    
    
    struct SetR3_I32<VALUE> : Stmt where VALUE : struct, Expr<int> {
        [MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
        public void Run( ref Registers reg, Span<long> frame, WasmInstance inst )
        {
            reg.R3 = (uint) default( VALUE ).Run( ref reg, frame, inst );
        }
    }
    
    struct SetR3_I64<VALUE> : Stmt where VALUE : struct, Expr<long> {
        [MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
        public void Run( ref Registers reg, Span<long> frame, WasmInstance inst )
        {
            reg.R3 =  default( VALUE ).Run( ref reg, frame, inst );
        }
    }
    
    struct SetR3_F32<VALUE> : Stmt where VALUE : struct, Expr<float> {
        [MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
        public void Run( ref Registers reg, Span<long> frame, WasmInstance inst )
        {
            reg.R3 = BitConverter.SingleToUInt32Bits(  default( VALUE ).Run( ref reg, frame, inst ) );
        }
    }
    
    struct SetR3_F64<VALUE> : Stmt where VALUE : struct, Expr<double> {
        [MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
        public void Run( ref Registers reg, Span<long> frame, WasmInstance inst )
        {
            reg.R3 = BitConverter.DoubleToInt64Bits(  default( VALUE ).Run( ref reg, frame, inst ) );
        }
    }
    
    struct GetR4_I32 : Expr<int> {
        [MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
        public int Run( ref Registers reg, Span<long> frame, WasmInstance inst ) => (int) reg.R4 ;
    }
    
    struct GetR4_I64 : Expr<long> {
        [MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
        public long Run( ref Registers reg, Span<long> frame, WasmInstance inst ) =>  reg.R4 ;
    }
    
    struct GetR4_F32 : Expr<float> {
        [MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
        public float Run( ref Registers reg, Span<long> frame, WasmInstance inst ) => BitConverter.Int32BitsToSingle( (int) reg.R4  );
    }
    
    struct GetR4_F64 : Expr<double> {
        [MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
        public double Run( ref Registers reg, Span<long> frame, WasmInstance inst ) => BitConverter.Int64BitsToDouble(  reg.R4  );
    }
    
    
    struct SetR4_I32<VALUE> : Stmt where VALUE : struct, Expr<int> {
        [MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
        public void Run( ref Registers reg, Span<long> frame, WasmInstance inst )
        {
            reg.R4 = (uint) default( VALUE ).Run( ref reg, frame, inst );
        }
    }
    
    struct SetR4_I64<VALUE> : Stmt where VALUE : struct, Expr<long> {
        [MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
        public void Run( ref Registers reg, Span<long> frame, WasmInstance inst )
        {
            reg.R4 =  default( VALUE ).Run( ref reg, frame, inst );
        }
    }
    
    struct SetR4_F32<VALUE> : Stmt where VALUE : struct, Expr<float> {
        [MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
        public void Run( ref Registers reg, Span<long> frame, WasmInstance inst )
        {
            reg.R4 = BitConverter.SingleToUInt32Bits(  default( VALUE ).Run( ref reg, frame, inst ) );
        }
    }
    
    struct SetR4_F64<VALUE> : Stmt where VALUE : struct, Expr<double> {
        [MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
        public void Run( ref Registers reg, Span<long> frame, WasmInstance inst )
        {
            reg.R4 = BitConverter.DoubleToInt64Bits(  default( VALUE ).Run( ref reg, frame, inst ) );
        }
    }
    
    struct GetR5_I32 : Expr<int> {
        [MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
        public int Run( ref Registers reg, Span<long> frame, WasmInstance inst ) => (int) reg.R5 ;
    }
    
    struct GetR5_I64 : Expr<long> {
        [MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
        public long Run( ref Registers reg, Span<long> frame, WasmInstance inst ) =>  reg.R5 ;
    }
    
    struct GetR5_F32 : Expr<float> {
        [MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
        public float Run( ref Registers reg, Span<long> frame, WasmInstance inst ) => BitConverter.Int32BitsToSingle( (int) reg.R5  );
    }
    
    struct GetR5_F64 : Expr<double> {
        [MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
        public double Run( ref Registers reg, Span<long> frame, WasmInstance inst ) => BitConverter.Int64BitsToDouble(  reg.R5  );
    }
    
    
    struct SetR5_I32<VALUE> : Stmt where VALUE : struct, Expr<int> {
        [MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
        public void Run( ref Registers reg, Span<long> frame, WasmInstance inst )
        {
            reg.R5 = (uint) default( VALUE ).Run( ref reg, frame, inst );
        }
    }
    
    struct SetR5_I64<VALUE> : Stmt where VALUE : struct, Expr<long> {
        [MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
        public void Run( ref Registers reg, Span<long> frame, WasmInstance inst )
        {
            reg.R5 =  default( VALUE ).Run( ref reg, frame, inst );
        }
    }
    
    struct SetR5_F32<VALUE> : Stmt where VALUE : struct, Expr<float> {
        [MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
        public void Run( ref Registers reg, Span<long> frame, WasmInstance inst )
        {
            reg.R5 = BitConverter.SingleToUInt32Bits(  default( VALUE ).Run( ref reg, frame, inst ) );
        }
    }
    
    struct SetR5_F64<VALUE> : Stmt where VALUE : struct, Expr<double> {
        [MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
        public void Run( ref Registers reg, Span<long> frame, WasmInstance inst )
        {
            reg.R5 = BitConverter.DoubleToInt64Bits(  default( VALUE ).Run( ref reg, frame, inst ) );
        }
    }
    
    struct GetR6_I32 : Expr<int> {
        [MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
        public int Run( ref Registers reg, Span<long> frame, WasmInstance inst ) => (int) reg.R6 ;
    }
    
    struct GetR6_I64 : Expr<long> {
        [MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
        public long Run( ref Registers reg, Span<long> frame, WasmInstance inst ) =>  reg.R6 ;
    }
    
    struct GetR6_F32 : Expr<float> {
        [MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
        public float Run( ref Registers reg, Span<long> frame, WasmInstance inst ) => BitConverter.Int32BitsToSingle( (int) reg.R6  );
    }
    
    struct GetR6_F64 : Expr<double> {
        [MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
        public double Run( ref Registers reg, Span<long> frame, WasmInstance inst ) => BitConverter.Int64BitsToDouble(  reg.R6  );
    }
    
    
    struct SetR6_I32<VALUE> : Stmt where VALUE : struct, Expr<int> {
        [MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
        public void Run( ref Registers reg, Span<long> frame, WasmInstance inst )
        {
            reg.R6 = (uint) default( VALUE ).Run( ref reg, frame, inst );
        }
    }
    
    struct SetR6_I64<VALUE> : Stmt where VALUE : struct, Expr<long> {
        [MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
        public void Run( ref Registers reg, Span<long> frame, WasmInstance inst )
        {
            reg.R6 =  default( VALUE ).Run( ref reg, frame, inst );
        }
    }
    
    struct SetR6_F32<VALUE> : Stmt where VALUE : struct, Expr<float> {
        [MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
        public void Run( ref Registers reg, Span<long> frame, WasmInstance inst )
        {
            reg.R6 = BitConverter.SingleToUInt32Bits(  default( VALUE ).Run( ref reg, frame, inst ) );
        }
    }
    
    struct SetR6_F64<VALUE> : Stmt where VALUE : struct, Expr<double> {
        [MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
        public void Run( ref Registers reg, Span<long> frame, WasmInstance inst )
        {
            reg.R6 = BitConverter.DoubleToInt64Bits(  default( VALUE ).Run( ref reg, frame, inst ) );
        }
    }

    
    struct GetFrame_I32<INDEX> : Expr<int> where INDEX : struct, Const {
        [MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
        public int Run( ref Registers reg, Span<long> frame, WasmInstance inst ) => (int) frame[(int)default( INDEX ).Run()] ;
    }
    
    struct GetFrame_I64<INDEX> : Expr<long> where INDEX : struct, Const {
        [MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
        public long Run( ref Registers reg, Span<long> frame, WasmInstance inst ) =>  frame[(int)default( INDEX ).Run()] ;
    }
    
    struct GetFrame_F32<INDEX> : Expr<float> where INDEX : struct, Const {
        [MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
        public float Run( ref Registers reg, Span<long> frame, WasmInstance inst ) => BitConverter.Int32BitsToSingle( (int) frame[(int)default( INDEX ).Run()]  );
    }
    
    struct GetFrame_F64<INDEX> : Expr<double> where INDEX : struct, Const {
        [MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
        public double Run( ref Registers reg, Span<long> frame, WasmInstance inst ) => BitConverter.Int64BitsToDouble(  frame[(int)default( INDEX ).Run()]  );
    }

    
    struct SetFrame_I32<INDEX,VALUE> : Stmt where INDEX : struct, Const where VALUE : struct, Expr<int> {
        [MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
        public void Run( ref Registers reg, Span<long> frame, WasmInstance inst )
        {
            frame[(int)default( INDEX ).Run()] = (uint) default( VALUE ).Run( ref reg, frame, inst );
        }
    }
    
    struct SetFrame_I64<INDEX,VALUE> : Stmt where INDEX : struct, Const where VALUE : struct, Expr<long> {
        [MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
        public void Run( ref Registers reg, Span<long> frame, WasmInstance inst )
        {
            frame[(int)default( INDEX ).Run()] =  default( VALUE ).Run( ref reg, frame, inst );
        }
    }
    
    struct SetFrame_F32<INDEX,VALUE> : Stmt where INDEX : struct, Const where VALUE : struct, Expr<float> {
        [MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
        public void Run( ref Registers reg, Span<long> frame, WasmInstance inst )
        {
            frame[(int)default( INDEX ).Run()] = BitConverter.SingleToUInt32Bits(  default( VALUE ).Run( ref reg, frame, inst ) );
        }
    }
    
    struct SetFrame_F64<INDEX,VALUE> : Stmt where INDEX : struct, Const where VALUE : struct, Expr<double> {
        [MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
        public void Run( ref Registers reg, Span<long> frame, WasmInstance inst )
        {
            frame[(int)default( INDEX ).Run()] = BitConverter.DoubleToInt64Bits(  default( VALUE ).Run( ref reg, frame, inst ) );
        }
    }
}
