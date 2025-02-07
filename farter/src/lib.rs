#[no_mangle]
pub extern "C" fn test1(x: i32, y: i32) -> i32 {
    if x <= 0 {
        return -1;
    }
    let mut a = 0;
    for i in 0..200_000_000 {
        a += a / x + a - y;
    }
    a
}

#[no_mangle]
pub extern "C" fn test2(x: i32, y: i32) -> f32 {
    let x = x as f32;
    let y = y as f32;
    let mut a = 0.0;
    for i in 0..200_000_000 {
        a += a / x + a - y;
    }
    a
}

#[no_mangle]
pub extern "C" fn test3(x: i32, y: i32) -> i32 {
    let mut res = 0;
    for i in 0..200_000_000 {
        res += i % x;
        res += i % y;
    }
    return res;
}
