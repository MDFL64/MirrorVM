var fs = require("fs");

let name = "hash_sha1";

async function main() {
    var bytes = fs.readFileSync("farter/target/wasm32-unknown-unknown/release/farter.wasm");
    
    var module = await WebAssembly.instantiate(bytes);
    var _ = module.instance.exports[name](100_000_000,456);

    let start = performance.now();
    var res = module.instance.exports[name](100_000_000,456);
    //module.in
    console.log(res);
    console.log(performance.now()-start);
}
main();
