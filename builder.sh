#!/bin/bash
set -e

ENV=${1:-development}
ACTION=${2:-none}

# Normalize ENV
if [ "$ENV" == "dev" ]; then ENV="development"; fi

echo "========================================"
echo "  Basket API Build System"
echo "  Target Environment: $ENV"
echo "========================================"

# Check for submodules
if [ ! -f "modules/Base-Api/BaseApi/BaseApi.csproj" ]; then
    echo "Error: Base-Api submodule not found. Run 'git submodule update --init --recursive'."
    exit 1
fi

echo "[1/3] Building solution..."
dotnet build BasketApi.sln -c Release

echo "[2/3] Building Docker Image ($ENV)..."
docker compose -f docker-compose.yml -f docker-compose.$ENV.yml build

if [ "$ACTION" == "up" ]; then
    echo "[3/3] Starting stack with Docker Compose..."
    docker compose -f docker-compose.yml -f docker-compose.$ENV.yml up -d
else
    echo "[3/3] Build complete. Use './builder.sh $ENV up' to start services."
fi

echo "========================================"
echo "  Done."
echo "========================================"
