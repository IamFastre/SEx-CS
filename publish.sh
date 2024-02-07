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
  echo "      -w, --windows   Publish app for Windows"
  echo "      -l, --linux     Publish app for Linux"
  echo "      -m, --macos     Publish app for macOS/OSX"
  echo
  exit 0
}

publish() {
  echo  "[*] Building app for $1..."
  dotnet publish $(dirname $0)/$1.csproj
  echo  "[*] Done!"

  echo  "[*] Moving files..."
  rm    $(dirname $0)/build/$1 -r
  mkdir $(dirname $0)/build/$1
  mv    $(dirname $0)/build/Debug/$2/publish/* $(dirname $0)/build/$1 -v
  rm    $(dirname $0)/build/Debug/$2 -r -v
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

    elif [ "$i" = "--linux" ];
      then publish "Linux"   "linux-x64"
    elif [ "$i" = "--macos" ];
      then publish "macOS"   "osx-x64"
    elif [ "$i" = "--windows" ];
      then publish "Windows" "win-x64"
    elif [ "$i" = "--help" ];
      then help
    fi
  done
fi
