use std::time::Instant;

use wasmi::*;

// In this simple example we are going to compile the below Wasm source,
// instantiate a Wasm module from it and call its exported "hello" function.
fn main() -> Result<(), Box<dyn std::error::Error>> {
    let wasm = std::fs::read("../rust_bench/target/wasm32-unknown-unknown/release/rust_bench.wasm")?;
    let engine = Engine::default();
    let module = Module::new(&engine, wasm)?;

    type HostState = u32;
    let mut store = Store::new(&engine, 42);

    let mut linker = <Linker<HostState>>::new(&engine);
    let instance = linker
        .instantiate(&mut store, &module)?
        .start(&mut store)?;

    for i in 0..3 {
        let t1 = Instant::now();
    
        instance
            .get_typed_func::<(), (i32)>(&store, "bench_hashes")?
            .call(&mut store, ())?;
    
        println!("t = {:?}",t1.elapsed());
    }

    Ok(())
}
