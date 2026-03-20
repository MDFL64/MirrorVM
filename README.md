# MirrorVM

A reflection-driven WebAssembly compiler.

See [here](https://sbox.game/churchofmiku/mirrorvm/news/mirrorvm-compiling-webassembly-using-reflection-e90bd343#new-section) for more information.

## Optimization Notes
- Control Flow Optimization is somewhat helpful.
- Usage of registers is somewhat helpful.
- Increasing ref-register count up to 63 dramatically helps hash benchmark, but other benchmarks start to degrade. 31 may be a good middle-ground.
- MethodImplOptions.AggressiveInlining is somewhat helpful.
- MethodImplOptions.AggressiveOptimization is somewhat neutral.
- Attempts to stack allocate a frame are still counterproductive.
