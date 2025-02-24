rm dump.txt
$env:DOTNET_JitDisasm=""
dotnet build --configuration Release
$env:DOTNET_JitDisasm="*"
$env:DOTNET_JitStdOutFile="dump.txt"
$env:DOTNET_JitDisasmOnlyOptimized=1

#$env:DOTNET_JitInlinePrintStats=1
#$env:DOTNET_JitPrintInlinedMethods="*"
#$env:DOTNET_JitInlineDumpData=1
#$env:DOTNET_JitInlineDumpXml=1
#$env:DOTNET_JitInlineDumpXmlFile="poo.xml"

./bin/release/net9.0/brainfart.exe
