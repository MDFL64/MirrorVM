let fs = require("fs");


let entries = {};

{
    let lines = fs.readFileSync("dump.txt").toString().split("\n");
    let index = 0;
    while (index < lines.length) {
        let line = lines[index];
        if (line.startsWith("; Assembly listing for method ")) {
            let name = line.substring(30).split(" ")[0];
            let calls = new Set();
            //console.log(">>>",name);
            for (;;) {
                index++;
                let line = lines[index].trim();
                if (line.startsWith("call")) {
                    let fn_name = line.substring(9);
                    if (fn_name.startsWith("[")) {
                        calls.add( fn_name.substring(1,fn_name.length-1) );
                    }
                } else if (line.startsWith("; Total bytes of code")) {
                    // todo
                    let size = +line.substring(22);
                    //throw size;
                    entries[name] = {calls,size,index};
                    break;
                }
            }
        }
        index++;
    }
}

let functions = {};

{
    let lines = fs.readFileSync("compile_log.txt").toString().split("\n");
    for (let line of lines) {
        let parts = line.split(" :: ");
        if (parts.length == 2) {
            functions[ parts[0] ] = parts[1];
        }
    }
}

function size_ty(ty_name) {
    let first = ty_name.split(":")[0];

    const re = /[A-Za-z][A-Za-z_0-9]*/g
    return ((first || '').match(re) || []).length;
    //return first.length;
}

function dump_ty(ty_name,depth) {
    const max_len = 100;
    let count = 1;

    let print_line = "    ".repeat(depth) + size_ty(ty_name) + " " + ty_name;

    if (print_line.length > max_len) {
        print_line = print_line.substring(0,max_len)+"...";
    }
    console.log(print_line);

    let entry = entries[ty_name];
    if (entry != null) {
        for (let x of entry.calls) {
            count += dump_ty(x,depth+1);
        }
    }
    return count;
}

function dump_function(name) {
    console.log("function "+name);
    console.log("--------------------");
    let ty = functions[name] + ":Call(System.Span`1[long],WasmInstance):this";
    let n = dump_ty(ty,0);
    console.log("count = "+n);
}

dump_function("memset");

//console.log(functions);

//console.log(entries);

/*let output = "digraph {\n";

let stack = [];

for (let key in entries) {
    if (key.length >= 1000) {
        entries[key].closed = true;
        stack.push(key);
    }
}

let count = 0;
while (stack.length > 0) {
    let key = stack.pop();
    count++;
    let entry = entries[key];
    let trimmed = key.substring(0,20);
    let size = Math.sqrt(entry.size/80);
    if (size < 1) {
        size = 1;
    }
    let w = size*2;
    let h = size/2;
    output += entry.index + " [ shape=box label=\""+trimmed+"\" height="+h+" width="+w+" fixedsize=true ]\n";
    //console.log(entry);
    for (let call_key of entry.calls) {
        let call_entry = entries[call_key];
        if (call_entry != null) {
            output += entry.index + " -> " + call_entry.index+"\n";

            if (!call_entry.closed) {
                call_entry.closed = true;
                stack.push(call_key);
            }
        }
    }
}

output += "}";
fs.writeFileSync("inlines.dot",output);

console.log("split count = "+count);*/
