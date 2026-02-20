#!/bin/bash
set -e

VERSION=${1:-1.0.0}
OUTPUT="./nupkgs"

echo "========================================"
echo "  Packaging BasketApi v$VERSION"
echo "========================================"

if [ -d "$OUTPUT" ]; then
    echo "Cleaning output directory..."
    rm -rf "$OUTPUT"
fi

mkdir -p "$OUTPUT"

echo "Running dotnet pack..."
dotnet pack BasketApi/BasketApi.csproj -c Release -o "$OUTPUT" /p:PackageVersion=$VERSION

echo "Done. Packages are in $OUTPUT"
echo "========================================"
