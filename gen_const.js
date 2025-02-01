const fs = require("fs");

let res = "interface Const { int Run(); }\n\n";
let cases = "";

for(let i=0;i<256;i++) {
    let n = i.toString(16).padStart(2,0).toUpperCase();
    res += `struct B${n} : Const { public int Run() => 0x${n}; }\n`;
    cases += `            case 0x${n}: return typeof(B${n});\n`;
}
res += "\n";
cases += "\n";

for(let i=1;i<256;i++) {
    let n = i.toString(16).padStart(2,0).toUpperCase();
    res += `struct BN${n} : Const { public int Run() => -0x${n}; }\n`;
    cases += `            case -0x${n}: return typeof(BN${n});\n`;
}

res += `
public class ConstBuilder {
    public static Type BuildInt(int n) {
        switch (n) {
${cases}
            default:
                throw new Exception("const out of range "+n);
        }
    }
}
`;

fs.writeFileSync("Constants.cs",res);
