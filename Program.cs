using System.ComponentModel.Design.Serialization;
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;

var cmds = JsonSerializer.Deserialize<TestCommands>(File.ReadAllText("tests/i32.json"));
cmds.Run();
