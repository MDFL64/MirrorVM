let fs = require("fs");

let names = ["rand_sort", "hashes", "prospero_compile","prospero_eval"];

async function main() {
    let bytes = fs.readFileSync("rust_bench/target/wasm32-unknown-unknown/release/rust_bench.wasm");
    
    let module = await WebAssembly.instantiate(bytes);

    for (let name of names) {
        let f = module.instance.exports["bench_"+name];

        let min_time = 1/0;

        for (let i=0;i<10;i++) {
            let start = performance.now();
            f();
            let time = performance.now()-start;
            min_time = Math.min(min_time,time);
        }

        console.log(name+","+min_time/1000);
    }
}
main();
