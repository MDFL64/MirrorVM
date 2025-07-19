use std::time::Instant;

use wasmi::*;

// In this simple example we are going to compile the below Wasm source,
// instantiate a Wasm module from it and call its exported "hello" function.
fn main() -> Result<(), Box<dyn std::error::Error>> {
    let benchmarks = &["hashes", "image", "json", "prospero_compile", "prospero_eval", "rand_sort", "rapier", "regex", "zip"];

    let wasm = std::fs::read("../rust_bench/target/wasm32-unknown-unknown/release/rust_bench.wasm")?;
    let engine = Engine::default();
    let module = Module::new(&engine, wasm)?;

    type HostState = u32;
    let mut store = Store::new(&engine, 42);

    let mut linker = <Linker<HostState>>::new(&engine);
    let instance = linker
        .instantiate(&mut store, &module)?
        .start(&mut store)?;

    let mut final_stats = Vec::new();

    for bench_name in benchmarks {
        let full_name = format!("bench_{}",bench_name);
        let mut times= Vec::new();
        println!("> {}",bench_name);
        for i in 0..5 {
            let t1 = Instant::now();
        
            instance
                .get_typed_func::<(), (i32)>(&store, &full_name)?
                .call(&mut store, ())?;
        
            let elapsed = t1.elapsed();
            println!("{} t = {:?}",i,elapsed);
            times.push(elapsed);
        }
        times.sort();
        println!("min = {:?}",times.first().unwrap());
        println!("max = {:?}",times.last().unwrap());
        final_stats.push(format!("{},{}",bench_name,times.first().unwrap().as_secs_f64()));
    }

    for fs in final_stats {
        println!("{}",fs);
    }


    Ok(())
}
