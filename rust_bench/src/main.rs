use core::f64;
use std::time::Instant;

use prospero::{bench_prospero_compile, bench_prospero_eval};
use physics::bench_rapier;

pub fn main() {
    // "hashes", "image", "json", "prospero_compile", "prospero_eval", "rand_sort", "rapier", "regex", "zip"
    let bench_fns = [
        bench_hashes, bench_image, bench_json, bench_prospero_compile, bench_prospero_eval,
        bench_rand_sort, bench_rapier, bench_regex, bench_zip
    ];

    for func in bench_fns {
        let mut min_time = f64::INFINITY;
        for _ in 0..10 {
            let start = Instant::now();
            func();
            min_time = min_time.min(start.elapsed().as_secs_f64());
        }
        println!("{}",min_time);
    }
}

// yuckers
include!("lib.rs");
