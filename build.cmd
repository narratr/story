@echo off
setlocal

:: Ensure you’re in the solution root
echo Installing NuGet packages...
nuget install Story.Core\packages.config -o packages
if %errorlevel% neq 0 exit /b %errorlevel%

:: Build Story.Core
pushd Story.Core
echo Building Story.Core...
msbuild Story.Core.csproj /p:Configuration=Release
if %errorlevel% neq 0 exit /b %errorlevel%
popd

:: Build Story.Ext
pushd Story.Ext
echo Building Story.Ext...
msbuild Story.Ext.csproj /p:Configuration=Release
if %errorlevel% neq 0 exit /b %errorlevel%
popd

:: Pack NuGet packages
echo Packing NuGet packages...
nuget pack Story.Core\Story.Core.csproj -IncludeReferencedProjects -Prop Configuration=Release
if %errorlevel% neq 0 exit /b %errorlevel%
nuget pack Story.Ext\Story.Ext.csproj -IncludeReferencedProjects -Prop Configuration=Release
if %errorlevel% neq 0 exit /b %errorlevel%

echo Done!
endlocal
