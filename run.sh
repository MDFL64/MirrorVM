rm dump.txt
export DOTNET_JitDisasm=""
dotnet build --configuration Release
set -f
export DOTNET_JitDisasm='*'
export DOTNET_JitStdOutFile="./dump.txt"
export DOTNET_JitDisasmOnlyOptimized=1
./bin/Release/net10.0/MirrorVM
set +f
