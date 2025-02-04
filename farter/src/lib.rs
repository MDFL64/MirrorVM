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
pub extern "C" fn test2(x: i32, y: i32) -> i32 {
    let mut a = 0;
    for i in 0..200_000_000 {
        if i % 7 != 0 {
            a += i * x + i * y + i * 7;
        } else {
            a += 1000;
        }
    }
    a
}

#[no_mangle]
pub extern "C" fn test_i32_compare(x: i32, y: i32) -> i32 {
    if x == 0 || x == 100 {
        return 1;
    }
    if x < 50 {
        return 2;
    }
    if x <= 60 {
        return 3;
    }

    0
}
