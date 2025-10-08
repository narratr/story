@echo off
echo Building Story Examples...
echo.

REM Build the solution
msbuild Story.sln /p:Configuration=Debug /p:Platform="Any CPU" /verbosity:minimal

if %ERRORLEVEL% neq 0 (
    echo Build failed!
    pause
    exit /b 1
)

echo.
echo Build successful! Running examples...
echo.

REM Run the examples
cd Playground
bin\Debug\Story.Examples.exe

echo.
echo Examples completed!
pause
