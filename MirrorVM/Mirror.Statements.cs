using System;
using System.Runtime.CompilerServices;

namespace MirrorVM
{
    // statements, terminators, and miscellaneous ops

    struct Select<COND, A, B, T> : Expr<T>
        where COND : struct, Expr<int>
        where A : struct, Expr<T>
        where B : struct, Expr<T>
    {
        [MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
        public T Run( ref Registers reg, Span<long> frame, WasmInstance inst )
        {
            if ( default( COND ).Run( ref reg, frame, inst ) != 0 )
            {
                return default( A ).Run( ref reg, frame, inst );
            }
            else
            {
                return default( B ).Run( ref reg, frame, inst );
            }
        }
    }

    struct End : Stmt {
        [MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
        public void Run( ref Registers reg, Span<long> frame, WasmInstance inst )
        { }
    }

    struct ExprStmt<VALUE, T> : Stmt where VALUE : struct, Expr<T> {
        [MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
        public void Run( ref Registers reg, Span<long> frame, WasmInstance inst )
        {
            default( VALUE ).Run( ref reg, frame, inst );
        }
    }

    struct ClearFrame<START,END> : Stmt
        where START : struct, Const
        where END : struct, Const
    {
        [MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
        public void Run( ref Registers reg, Span<long> frame, WasmInstance inst )
        {
            int start = (int)default( START ).Run();
            int end = (int)default( END ).Run();
            //Console.WriteLine("clear frame " + start + " " + end);
            for (int i = start; i < end; i++)
            {
                frame[i] = 0;
            }
        }
    }

    struct NoInline<A> : Stmt
        where A : struct, Stmt
    {
        [MethodImpl( MethodImplOptions.NoInlining | MethodImplOptions.AggressiveOptimization )]
        public void Run( ref Registers reg, Span<long> frame, WasmInstance inst )
        {
            //Registers r2 = reg;
            default( A ).Run( ref reg, frame, inst );
            //reg = r2;
        }
    }

    struct TermReturn<BODY> : Terminator
        where BODY : struct, Stmt
    {
        [MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
        public void Run( ref Registers reg, Span<long> frame, WasmInstance inst )
        {
            default( BODY ).Run( ref reg, frame, inst );
            reg.NextBlock = -1;
        }
    }

    struct StmtTrap : Stmt
    {
        [MethodImpl( MethodImplOptions.NoInlining | MethodImplOptions.AggressiveOptimization )]
        public void Run( ref Registers reg, Span<long> frame, WasmInstance inst )
        {
            throw new Exception( "trap (stmt) @ " + reg.NextBlock );
        }
    }

    struct TermTrap<BODY> : Terminator
        where BODY : struct, Stmt
    {
        [MethodImpl( MethodImplOptions.NoInlining | MethodImplOptions.AggressiveOptimization )]
        public void Run( ref Registers reg, Span<long> frame, WasmInstance inst )
        {
            default( BODY ).Run( ref reg, frame, inst );
            throw new Exception( "trap @ " + reg.NextBlock );
        }
    }

    struct TermVoid : Terminator
    {
        [MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
        public void Run( ref Registers reg, Span<long> frame, WasmInstance inst )
        {
            //throw new Exception("entered void block");
        }
    }

    
    struct Stmts1<A, B, C, D> : Stmt
        where A : struct, Stmt
        where B : struct, Stmt
        where C : struct, Stmt
        where D : struct, Stmt
    {
        [MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
        public void Run( ref Registers reg, Span<long> frame, WasmInstance inst )
        {
            default( A ).Run( ref reg, frame, inst );
            default( B ).Run( ref reg, frame, inst );
            default( C ).Run( ref reg, frame, inst );
            default( D ).Run( ref reg, frame, inst );
        }
    }
    
    struct Stmts2<A, B, C, D> : Stmt
        where A : struct, Stmt
        where B : struct, Stmt
        where C : struct, Stmt
        where D : struct, Stmt
    {
        [MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
        public void Run( ref Registers reg, Span<long> frame, WasmInstance inst )
        {
            default( A ).Run( ref reg, frame, inst );
            default( B ).Run( ref reg, frame, inst );
            default( C ).Run( ref reg, frame, inst );
            default( D ).Run( ref reg, frame, inst );
        }
    }
    
    struct Stmts3<A, B, C, D> : Stmt
        where A : struct, Stmt
        where B : struct, Stmt
        where C : struct, Stmt
        where D : struct, Stmt
    {
        [MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
        public void Run( ref Registers reg, Span<long> frame, WasmInstance inst )
        {
            default( A ).Run( ref reg, frame, inst );
            default( B ).Run( ref reg, frame, inst );
            default( C ).Run( ref reg, frame, inst );
            default( D ).Run( ref reg, frame, inst );
        }
    }
    
    struct Stmts4<A, B, C, D> : Stmt
        where A : struct, Stmt
        where B : struct, Stmt
        where C : struct, Stmt
        where D : struct, Stmt
    {
        [MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
        public void Run( ref Registers reg, Span<long> frame, WasmInstance inst )
        {
            default( A ).Run( ref reg, frame, inst );
            default( B ).Run( ref reg, frame, inst );
            default( C ).Run( ref reg, frame, inst );
            default( D ).Run( ref reg, frame, inst );
        }
    }
    
    struct Stmts5<A, B, C, D> : Stmt
        where A : struct, Stmt
        where B : struct, Stmt
        where C : struct, Stmt
        where D : struct, Stmt
    {
        [MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
        public void Run( ref Registers reg, Span<long> frame, WasmInstance inst )
        {
            default( A ).Run( ref reg, frame, inst );
            default( B ).Run( ref reg, frame, inst );
            default( C ).Run( ref reg, frame, inst );
            default( D ).Run( ref reg, frame, inst );
        }
    }
    
    struct Stmts6<A, B, C, D> : Stmt
        where A : struct, Stmt
        where B : struct, Stmt
        where C : struct, Stmt
        where D : struct, Stmt
    {
        [MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
        public void Run( ref Registers reg, Span<long> frame, WasmInstance inst )
        {
            default( A ).Run( ref reg, frame, inst );
            default( B ).Run( ref reg, frame, inst );
            default( C ).Run( ref reg, frame, inst );
            default( D ).Run( ref reg, frame, inst );
        }
    }
    
    struct Stmts7<A, B, C, D> : Stmt
        where A : struct, Stmt
        where B : struct, Stmt
        where C : struct, Stmt
        where D : struct, Stmt
    {
        [MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
        public void Run( ref Registers reg, Span<long> frame, WasmInstance inst )
        {
            default( A ).Run( ref reg, frame, inst );
            default( B ).Run( ref reg, frame, inst );
            default( C ).Run( ref reg, frame, inst );
            default( D ).Run( ref reg, frame, inst );
        }
    }
    
    struct Stmts8<A, B, C, D> : Stmt
        where A : struct, Stmt
        where B : struct, Stmt
        where C : struct, Stmt
        where D : struct, Stmt
    {
        [MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
        public void Run( ref Registers reg, Span<long> frame, WasmInstance inst )
        {
            default( A ).Run( ref reg, frame, inst );
            default( B ).Run( ref reg, frame, inst );
            default( C ).Run( ref reg, frame, inst );
            default( D ).Run( ref reg, frame, inst );
        }
    }
}
