#$env:DOTNET_JitDisasm="*"
$env:DOTNET_JitStdOutFile="dump.txt"
#$env:DOTNET_JitDisasmOnlyOptimized=1
$env:DOTNET_JitDisasmSummary=1
dotnet run --configuration Release
