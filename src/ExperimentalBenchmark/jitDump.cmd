
set COMPlus_JitDump=Main
dotnet publish -c Release
robocopy /e c:\source\runtime\artifacts\bin\coreclr\Windows_NT.x64.Release .\bin\Release\net5.0\win-x64\publish
copy /y c:\source\runtime\artifacts\bin\coreclr\Windows_NT.x64.Debug\clrjit.dll .\bin\Release\net5.0\win-x64\publish
