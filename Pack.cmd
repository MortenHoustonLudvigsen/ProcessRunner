setlocal

:: Change to this script's directory
cd /d %~dp0

set NuGet=%~dp0.nuget\nuget.exe

rmdir /Q /S NuGetPackages
mkdir NuGetPackages
%NuGet% pack ProcessRunner\ProcessRunner.csproj -Build -Properties "Configuration=Release" -Symbols -OutputDirectory NuGetPackages
