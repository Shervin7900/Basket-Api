@echo off
echo Packaging BasketApi...
dotnet pack BasketApi/BasketApi.csproj -c Release -o ./nupkgs
echo Done.
pause
