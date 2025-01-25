const fs = require("fs");

const lines = fs.readFileSync("dump.txt").toString().split("\n");

let matcher = /^(Loop|UpdateCell|UpdatePointer)/;

let lut = {};
let lut_indices = {};
let lut_marks = {};
let links = [];

function assert(x) {
    if (!x) {
        throw new Error("assert failed");
    }
}

function decompose(str) {
    if (lut[str] != null) {
        return lut[str];
    }

    let index = str.indexOf("`");
    if (index == -1) {
        lut[str] = str;
        return str;
    } else {
        let id = str.substr(0,index);
        {
            let n = lut_indices[id] ?? 1;
            lut_indices[id] = n+1;
            id += "_"+n;
            lut[str] = id;
        }
        index++;
        while (Number.isFinite(parseInt(str[index]))) {
            index++;
        }
        assert(str[index] == "[");
        let done = false;
        while (!done) {
            index++;
            let child_start = index;
            let depth = 0;
            for (;;) {
                if (str[index] == "[") {
                    depth++;
                }
                if (str[index] == "]") {
                    depth--;
                    if (depth < 0) {
                        done = true;
                        break;
                    }
                }
                if (depth == 0 && str[index] == ",") {
                    break;
                }
                index++;
            }
            
            let child = str.substring(child_start,index);
            let child_id = decompose(child);
            links.push(`${id} -> ${child_id}`);
        }
        return id;
    }
}

let count = 300;

for( let line of lines) {
    let parts = line.split("JIT compiled").map(x => x.trim());
    if (parts.length == 2) {
        let main = parts[1];
        if (main.includes("Tier1") && main.match(matcher)) {
            main = main.split(":")[0];
            let marked = decompose(main);
            lut_marks[marked] = true;
            //console.log("==>",main);
            //console.log(links);
            count--;
            if (count <= 0) {
                break; // stop for now
            }
        }
    }
}
// a [fillcolor="red" style=filled];
let marks_str = Object.keys(lut_marks).map(x => x+` [fillcolor="lime" style=filled]`);

fs.writeFileSync("graph.dot",`
digraph mygraph {
    ${marks_str.join("\n\t")}

    ${links.join("\n\t")}
}
`);
//console.log(lut_marks);
