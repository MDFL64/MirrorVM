use std::collections::HashMap;
use std::sync::Mutex;

/*fn main() {

    let mut bytecode = compile(&std::fs::read_to_string("prospero.vm").unwrap());
    //bytecode.truncate(0x8 + 1);
    println!("{:?}",bytecode.last());
    let mut values = vec![0f32; bytecode.len()];

    let mut image = GrayImage::new(SIZE, SIZE);
    let mut stats = Stats{
        min: f32::INFINITY,
        max: -f32::INFINITY,
        is_nan: false,
        sqrt_min: f32::INFINITY,
        sqrt_max: -f32::INFINITY,
    };

    for y in 0..SIZE {
        //println!("> {}",y);
        for x in 0..SIZE {
            let fx = x as f32 / (SIZE / 2) as f32 - 1.0;
            let fy = -(y as f32 / (SIZE / 2) as f32 - 1.0);
            //println!("{} {}", fx, fy);
            eval(fx, fy, &bytecode, &mut values, &mut stats);

            if *values.last().unwrap() < 0.0 {
                image.put_pixel(x, y, Luma([0xFF]));
            }
        }
    }

    image.save("out.png");
    println!("{:?}",stats);
}*/

static BYTECODE: Mutex<Vec<Expr>> = Mutex::new(vec!());

#[no_mangle]
pub extern "C" fn bench_prospero_compile() -> i32 {
    let file = include_str!("prospero.vm");
    let mut bc = BYTECODE.lock().unwrap();
    for i in 0..40 {
        *bc = compile(file);
    }
    assert_eq!(bc.len(), 7866);
    0
}

#[no_mangle]
pub extern "C" fn bench_prospero_eval() -> i32 {
    let bc = BYTECODE.lock().unwrap();
    let mut values = vec![0f32; bc.len()];

    const SIZE: i32 = 65;
    let mut sum = 0.0;

    for y in 0..SIZE {
        for x in 0..SIZE {
            let x_f = x as f32 / SIZE as f32 * 2.0 - 1.0;
            let y_f = -y as f32 / SIZE as f32 * 2.0 + 1.0;

            eval(x_f, y_f, &bc, &mut values);
            sum += values.last().unwrap().abs();
        }
    }

    (sum * 1_000_000.0) as i32
}

#[no_mangle]
pub extern "C" fn prospero_eval(x: f32, y: f32) -> f32 {
    let bc = BYTECODE.lock().unwrap();
    let mut values = vec![0f32; bc.len()];

    eval(x, y, &bc, &mut values);

    *values.last().unwrap()
}

fn eval(x: f32, y: f32, bytecode: &[Expr], values: &mut [f32]) {
    
    for (i, e) in bytecode.iter().enumerate() {
        let mut val = match e {
            Expr::Const(c) => *c,
            Expr::VarX => x,
            Expr::VarY => y,

            Expr::Add(lhs, rhs) => values[*lhs as usize] + values[*rhs as usize],
            Expr::Sub(lhs, rhs) => values[*lhs as usize] - values[*rhs as usize],
            Expr::Mul(lhs, rhs) => values[*lhs as usize] * values[*rhs as usize],

            Expr::Max(lhs, rhs) => values[*lhs as usize].max( values[*rhs as usize] ),
            Expr::Min(lhs, rhs) => values[*lhs as usize].min( values[*rhs as usize] ),

            Expr::Neg(arg) => -values[*arg as usize],
            Expr::Square(arg) => values[*arg as usize].powi(2),
            Expr::Sqrt(arg) => {
                let a = values[*arg as usize];
                a.sqrt()
            }

            _ => panic!("todo {:?}", e),
        };

        values[i] = val;
    }
}

#[derive(Debug)]
enum Expr {
    VarX,
    VarY,
    Const(f32),

    Add(u32, u32),
    Sub(u32, u32),
    Mul(u32, u32),

    Max(u32, u32),
    Min(u32, u32),

    Neg(u32),
    Square(u32),
    Sqrt(u32),
}

fn compile(source: &str) -> Vec<Expr> {
    let lines = source
        .split("\n")
        .filter(|line| line.len() > 0 && !line.starts_with("#"));

    let mut lookup: HashMap<&str, u32> = HashMap::new();
    let mut exprs: Vec<Expr> = Vec::new();

    for line in lines {
        let mut tokens = line.split(" ");
        let name = tokens.next().unwrap();
        let op = tokens.next().unwrap();

        let e = match op {
            "var-x" => Expr::VarX,
            "var-y" => Expr::VarY,
            "const" => {
                let value: f32 = tokens.next().unwrap().parse().unwrap();
                Expr::Const(value)
            }
            "add" | "sub" | "mul" | "div" | "max" | "min" => {
                let lhs = lookup[tokens.next().unwrap()];
                let rhs = lookup[tokens.next().unwrap()];
                let f = match op {
                    "add" => Expr::Add,
                    "sub" => Expr::Sub,
                    "mul" => Expr::Mul,

                    "max" => Expr::Max,
                    "min" => Expr::Min,
                    _ => panic!("todo binary {}", op),
                };
                f(lhs, rhs)
            }
            "neg" | "square" | "sqrt" => {
                let arg = lookup[tokens.next().unwrap()];
                let f = match op {
                    "neg" => Expr::Neg,
                    "square" => Expr::Square,
                    "sqrt" => Expr::Sqrt,
                    _ => panic!("todo unary {}", op),
                };
                f(arg)
            }
            _ => panic!("todo op {}", op),
        };

        let index: u32 = exprs.len().try_into().unwrap();
        exprs.push(e);
        lookup.insert(name, index);
    }

    exprs
}
