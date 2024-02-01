@echo off

IF "%*"=="" GOTO :ALL

:START
    IF "%1"=="-w" CALL :PUBLISH "Windows" "win-x64"
    IF "%1"=="-l" CALL :PUBLISH "Linux"   "linux-x64"
    IF "%1"=="-m" CALL :PUBLISH "macOS"   "osx-x64"
    IF "%1"=="-h" GOTO :HELP
    IF "%1"=="--help" GOTO :HELP
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
    dotnet publish %~1.csproj
    echo [*] Done!

    echo [*] Moving files...
    mkdir "build\%~1"
    del   "build\%~1\*" /s /q /f
    move  "build\Debug\%~2\publish\*" "build\%~1\"
    rmdir "build\Debug\%~2\" /s /q
    echo [*] Done!

    echo.

:HELP
    echo.
    echo [*] Publishes the app for all platforms unless specified.
    echo.
    echo [*] Usage: %~nx0
    echo [*] Usage: %~nx0 [options]
    echo.
    echo [*] Options:
    echo       -h, --help      Show this menu
    echo       -w              Publish app for Windows
    echo       -l              Publish app for Linux
    echo       -m              Publish app for macOS/OSX
    EXIT /B 0
