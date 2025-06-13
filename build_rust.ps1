cd rust_bench
$env:RUSTFLAGS='--cfg getrandom_backend="custom"'
cargo build --release --target wasm32-unknown-unknown
cd ..
