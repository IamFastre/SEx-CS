#!/bin/sh

help() {
  echo
  echo "[*] Publishes the app for all platforms unless specified."
  echo
  echo "[*] Usage: $(basename $0)"
  echo "[*] Usage: $(basename $0) [options]"
  echo
  echo "[*] Options:"
  echo "      -h, --help      Show this menu"
  echo "      -w              Publish app for windows"
  echo "      -l              Publish app for linux"
  echo "      -m              Publish app for macOS/OSX"
  exit 0
}

publish() {
  echo  "[*] Building app for $1..."
  dotnet publish $1.csproj
  echo  "[*] Done!"

  echo  "[*] Moving files..."
  rm    "./build/$1" -r
  mkdir "./build/$1"
  mv    "./build/Debug/$2/publish/" "./build/$1" -v
  rm    "./build/Debug/$2" -r -v
  echo  "[*] Done!"

  echo
}

all() {
  publish "Linux"   "linux-x64"
  publish "macOS"   "osx-x64"
  publish "Windows" "win-x64"
}

if [ "$*" = "" ];
  then all
else
  for i; do
    if   [ "$i" = "-l" ];
      then publish "Linux"   "linux-x64"
    elif [ "$i" = "-m" ];
      then publish "macOS"   "osx-x64"
    elif [ "$i" = "-w" ];
      then publish "Windows" "win-x64"
    elif [ "$i" = "-h" ];
      then help
    elif [ "$i" = "--help" ];
      then help
    fi
  done
fi
