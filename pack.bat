@echo off
set VERSION=%1
if "%VERSION%"=="" set VERSION=1.0.0
set OUTPUT=.\nupkgs

echo ========================================
echo   Packaging BasketApi v%VERSION%
echo ========================================

if exist %OUTPUT% (
    echo Cleaning output directory...
    rd /s /q %OUTPUT%
)

mkdir %OUTPUT%

echo Running dotnet pack...
dotnet pack BasketApi/BasketApi.csproj -c Release -o %OUTPUT% /p:PackageVersion=%VERSION%

echo Done. Packages are in %OUTPUT%
echo ========================================
pause
