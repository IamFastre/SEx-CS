@echo off

IF "%*"=="" GOTO :ALL

:START
    IF "%1"=="-w" CALL :PUBLISH "Windows" "win-x64"
    IF "%1"=="-l" CALL :PUBLISH "Linux"   "linux-x64"
    IF "%1"=="-m" CALL :PUBLISH "macOS"   "osx-x64"
    IF "%1"=="-h" GOTO :HELP

    IF "%1"=="--windows" CALL :PUBLISH "Windows" "win-x64"
    IF "%1"=="--linux"   CALL :PUBLISH "Linux"   "linux-x64"
    IF "%1"=="--macos"   CALL :PUBLISH "macOS"   "osx-x64"
    IF "%1"=="--help"    GOTO :HELP

    SHIFT
    IF [%1]==[]   GOTO :EOF
    GOTO :START

:ALL
    CALL :PUBLISH "Windows" "win-x64"
    CALL :PUBLISH "Linux"   "linux-x64"
    CALL :PUBLISH "macOS"   "osx-x64"
    GOTO :EOF

:PUBLISH
    echo [*] Building app for %~1...
    dotnet publish %~dp0\%~1.csproj
    echo [*] Done!

    echo [*] Moving files...
    mkdir "%~dp0\build\%~1"
    del   "%~dp0\build\%~1\*" /s /q /f
    move  "%~dp0\build\Debug\%~2\publish\*" "%~dp0\build\%~1\"
    rmdir "%~dp0\build\Debug\%~2\" /s /q
    echo [*] Done!

    echo.
    EXIT /B 0

:HELP
    echo.
    echo [*] Publishes the app for all platforms unless specified.
    echo.
    echo [*] Usage: %~n0
    echo [*] Usage: %~n0 [options]
    echo.
    echo [*] Options:
    echo       -h, --help      Show this menu
    echo       -w, --windows   Publish app for Windows
    echo       -l, --linux     Publish app for Linux
    echo       -m, --macos     Publish app for macOS/OSX
    echo.
    EXIT /B 0
