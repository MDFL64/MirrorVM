import { readFileSync, writeFileSync } from "fs";

const METHOD_IMPL = "[MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]\n";

const TYPE_NAMES = {
    int: "I32",
    long: "I64",
    float: "F32",
    double: "F64"
}

const MACROS = {
    STMT_RUN: METHOD_IMPL + "public void Run( ref Registers reg, Span<long> frame, WasmInstance inst )",
    
    EXPR_RUN: function(ty) {
        return METHOD_IMPL + `public ${ty} Run( ref Registers reg, Span<long> frame, WasmInstance inst )`;
    },

    CAST_TO: function(ty,expr) {
        switch (ty) {
            case "int":
                return "(int)"+expr;
            case "long":
                return expr;
            case "float":
                return `BitConverter.Int32BitsToSingle( (int)${expr} )`;
            case "double":
                return `BitConverter.Int64BitsToDouble( ${expr} )`;
        }
        console.log({ty,expr});
        throw "bad cast to";
    },

    CAST_FROM: function(ty, expr) {
        switch (ty) {
            case "int":
            case "long":
                return expr;
            case "float":
                return `BitConverter.SingleToUInt32Bits( ${expr} )`;
            case "double":
                return `BitConverter.DoubleToInt64Bits( ${expr} )`;
        }
        console.log({ty,expr});
        throw "bad cast from";
    },

    CALL_EXPR: function(param_name) {
        return `default( ${param_name} ).Run( ref reg, frame, inst )`;
    },

    //EXPR_RUN: METHOD_IMPL + "public void Run( ref Registers reg, Span<long> frame, WasmInstance inst )",

    REG_GET: (num,ty) => `
struct GetR${num}_${TYPE_NAMES[ty]} : Expr<${ty}> {
    $EXPR_RUN(${ty}) => $CAST_TO(${ty}; reg.R${num} );
}`,
    FRAME_GET: (ty) => `
struct GetFrame_${TYPE_NAMES[ty]}<INDEX> : Expr<${ty}> where INDEX : struct, Const {
    $EXPR_RUN(${ty}) => $CAST_TO(${ty}; frame[(int)default( INDEX ).Run()] );
}`,


    REG_SET: (num,ty) => `
struct SetR${num}_${TYPE_NAMES[ty]}<VALUE> : Stmt where VALUE : struct, Expr<${ty}> {
    $STMT_RUN
    {
        reg.R${num} = $CAST_FROM(${ty}; $CALL_EXPR(VALUE));
    }
}`,
    FRAME_SET: (ty) => `
struct SetFrame_${TYPE_NAMES[ty]}<INDEX,VALUE> : Stmt where INDEX : struct, Const where VALUE : struct, Expr<${ty}> {
    $STMT_RUN
    {
        frame[(int)default( INDEX ).Run()] = $CAST_FROM(${ty}; $CALL_EXPR(VALUE));
    }
}`
};

function parseArgs(line) {
    if (line[0] != "(") {
        throw "bad args";
    }

    let depth = 0;
    let i = 0;

    for (;i<line.length;i++) {
        let c = line[i];
        if (c == "(") {
            depth++;
        } else if (c == ")") {
            depth--;
            if (depth == 0) {
                break;
            }
        }
    }

    let args = line.substring(1,i).split(";");
    let rest = line.substring(i+1);

    return {args, rest};
}

function processLine(line) {
    
    let whitespace = line.match(/^[ \t]*/)[0];

    let edited = false;

    line = line.replace(/\$(\w+)(\(.*)?/,(_,name,args)=>{

        if (args != null) {
            args = parseArgs(args);
        }
        console.log("["+line+"]",name,args);

        let macro = MACROS[name];
        if (typeof macro == "function") {
            macro = macro.apply(null,args?.args);
        }
        if (typeof macro != "string") {
            throw "bad macro";
        }

        edited = true;
        macro = macro.replace(/\n/g,"\n"+whitespace);

        let rest = args?.rest ?? "";

        return macro + rest;
    });

    if (edited) {
        line = processText(line);
    }
    
    return line;
}

function processText(text) {
    let lines = text.split("\n");

    for (let i=0;i<lines.length;i++) {
        lines[i] = processLine(lines[i]);
    }

    return lines.join("\n");
}

function generate(filename) {
    let lines = readFileSync(`template/${filename}.template`).toString().split("\n");

    for (let i=0;i<lines.length;i++) {
        lines[i] = processLine(lines[i]);
    }

    writeFileSync(`MirrorVM/${filename}.cs`, lines.join("\n"));
}

generate("Mirror.Call");
generate("Mirror.Locals");
