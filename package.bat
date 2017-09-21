REM This MUST be run from the VS Developer Command Prompt
REM Packages in Release mode and drops in the current folder.
msbuild /t:pack /p:Configuration=Release,PackageOutputPath=..\..\. src\Stringly.Typed\Stringly.Typed.csproj