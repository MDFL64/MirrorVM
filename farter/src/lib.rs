#[no_mangle]
pub extern "C" fn add(x: i32, y: i32) -> i32 {
    if x == -1 {
        return 12;
    }
    let mut a = 0;
    for i in 0..1_000_000 {
        a += a / x + a - y;
    }
    a
}
