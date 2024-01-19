@echo off

CALL :PUBLISH "Windows" "win-x64"
CALL :PUBLISH "Linux" "linux-x64"
@REM CALL :PUBLISH "macOS" "osx-x64"

EXIT /B 0

:PUBLISH
    echo [*] Building app for %~1...
    dotnet publish %~1.csproj
    echo [*] Done!
    echo [*] Moving files...
    mkdir "build\%~1"
    del   "build\%~1\*" /s /q /f
    move  "build\Debug\%~2\publish\*" "build\%~1\"
    rmdir "build\Debug\%~2\" /s /q
    echo [*] Done!
    echo.
