setlocal

:: Change to this script's directory
cd /d %~dp0

set NuGet=%~dp0.nuget\nuget.exe

call Pack.cmd

cd NuGetPackages

forfiles /m *.nupkg /c "cmd /c %NuGet% push @file"
