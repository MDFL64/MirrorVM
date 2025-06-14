let fs = require("fs");

let keys = [
    "abide","barrel","candy","displace","eggbeater",
    "fedora","gangway","harbor","illusive","jovial"
];

let result = [];

function makeObject() {
    let obj = {};

    for (let i=0;i<5;i++) {
        let key = keys[Math.floor(Math.random()*10)];
        let value;
        if (Math.random()<0.5) {
            value = Math.random() * 20 - 10;
        } else if (Math.random()<0.5) {
            value = "";
            let limit = Math.random()*10;
            for (let j=0;j<limit;j++) {
                value += "@";
            }
        } else {
            value = Math.random()<0.5;
        }

        obj[key] = value;
    }
    return obj;
}

for (let i=0;i<1000;i++) {
    result.push(makeObject());
}

fs.writeFileSync("data.json",JSON.stringify(result));
