@echo off
setlocal enabledelayedexpansion

set ENV=%1
if "%ENV%"=="" set ENV=development
if "%ENV%"=="dev" set ENV=development
set ACTION=%2

if not "%ENV%"=="development" if not "%ENV%"=="staging" if not "%ENV%"=="production" (
    echo Error: Invalid environment "%ENV%". Use: development ^| staging ^| production
    exit /b 1
)

echo ========================================
echo   Basket API Build System (Windows)
echo   Target Environment: %ENV%
echo ========================================

:: Initialize submodules if not present
if not exist "modules\Base-Api\BaseApi\BaseApi.csproj" (
    echo [0/3] Initializing submodules...
    git submodule update --init --recursive
    if %errorlevel% neq 0 exit /b %errorlevel%
)

echo [1/3] Building solution...
dotnet build BasketApi.sln -c Release
if %errorlevel% neq 0 exit /b %errorlevel%

echo [2/3] Building Docker Image (%ENV%)...
docker compose -f docker-compose.yml -f docker-compose.%ENV%.yml build
if %errorlevel% neq 0 exit /b %errorlevel%

if "%ACTION%"=="up" (
    echo [3/3] Starting stack with Docker Compose...
    docker compose -f docker-compose.yml -f docker-compose.%ENV%.yml up -d
) else if "%ACTION%"=="down" (
    echo [3/3] Stopping stack...
    docker compose -f docker-compose.yml -f docker-compose.%ENV%.yml down
) else (
    echo [3/3] Build complete. Use 'build.bat %ENV% up' to start services.
)

echo ========================================
echo   Done.
echo ========================================
