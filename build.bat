@echo off
setlocal enabledelayedexpansion

set ENV=%1
if "%ENV%"=="" set ENV=development
if "%ENV%"=="dev" set ENV=development
set ACTION=%2

echo ========================================
echo   Basket API Build System (Windows)
echo   Target Environment: %ENV%
echo ========================================

echo [1/3] Restoring and Building solution...
dotnet restore BasketApi.sln
if %errorlevel% neq 0 exit /b %errorlevel%

dotnet build BasketApi.sln -c Release --no-restore
if %errorlevel% neq 0 exit /b %errorlevel%

echo [2/3] Building Docker Image (%ENV%)...
docker compose -f docker-compose.yml -f docker-compose.%ENV%.yml build
if %errorlevel% neq 0 exit /b %errorlevel%

if "%ACTION%"=="up" (
    echo [3/3] Starting stack with Docker Compose...
    docker compose -f docker-compose.yml -f docker-compose.%ENV%.yml up -d
) else (
    echo [3/3] Build complete. Use 'build.bat %ENV% up' to start services.
)

echo ========================================
echo   Done.
echo ========================================
