#!/bin/sh

publish() {
    echo "[*] Building app for $1..."
    dotnet publish $1.csproj
    echo "[*] Done!"
    echo "[*] Moving files..."
    rm "./build/$1" -r
    mkdir "./build/$1"
    mv "./build/Debug/$2/publish" "./build/$1" -v
    rm "./build/Debug/$2" -r -v
    echo "[*] Done!"
    echo
}

publish "Linux" "linux-x64"
publish "Windows" "win-x64"
# publish "macOS" "osx-x64"

exit 0
