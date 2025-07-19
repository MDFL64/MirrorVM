cd rust_bench
$env:RUSTFLAGS='--cfg getrandom_backend="custom" --emit=llvm-ir'
cargo clean
cargo build --release --target wasm32-unknown-unknown -Z build-std=std,panic_abort
cd ..
