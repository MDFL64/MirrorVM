public class WasmInstance {
    public byte[] Memory;

    public WasmInstance(WasmModule module, bool full_init = true) {
        if (full_init) {
            Memory = module.GetInitialMemory();
        } else {
            Memory = [];
        }
    }
}
