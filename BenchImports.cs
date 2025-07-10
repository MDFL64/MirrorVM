
using System.Text;

class BenchImports : ImportProvider {
    public override ICallable ImportFunction(string module, string name, FunctionType sig)
    {
        if (module == "env" && name == "write_error")
        {
            return new WriteError();
        }
        Console.WriteLine("import " + module + " " + name);
        return null;
    }
}

class WriteError : ICallable
{
    public void Call(Span<long> frame, WasmInstance inst)
    {
        int str_base = (int)frame[0];
        int str_len = (int)frame[1];

        var slice = inst.Memory[str_base..(str_base + str_len)];
        var text = Encoding.UTF8.GetString(slice);

        Console.WriteLine("ERROR: "+text);
    }

    public void SetBody(object body, string name)
    {
        throw new NotImplementedException();
    }
}
