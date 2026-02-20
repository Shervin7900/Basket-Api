#!/bin/bash
set -e

ENV=${1:-development}
ACTION=${2:-none}

# Normalize ENV
if [ "$ENV" == "dev" ]; then ENV="development"; fi

# Validate ENV
if [[ "$ENV" != "development" && "$ENV" != "staging" && "$ENV" != "production" ]]; then
    echo "Error: Invalid environment '$ENV'. Use: development | staging | production"
    exit 1
fi

echo "========================================"
echo "  Basket API Build System"
echo "  Target Environment: $ENV"
echo "========================================"

# Initialize submodules if not present
if [ ! -f "modules/Base-Api/BaseApi/BaseApi.csproj" ]; then
    echo "[0/3] Initializing submodules..."
    git submodule update --init --recursive
fi

echo "[1/3] Building solution..."
dotnet build BasketApi.sln -c Release

echo "[2/3] Building Docker Image ($ENV)..."
docker compose -f docker-compose.yml -f docker-compose.$ENV.yml build

if [ "$ACTION" == "up" ]; then
    echo "[3/3] Starting stack with Docker Compose..."
    docker compose -f docker-compose.yml -f docker-compose.$ENV.yml up -d
elif [ "$ACTION" == "down" ]; then
    echo "[3/3] Stopping stack..."
    docker compose -f docker-compose.yml -f docker-compose.$ENV.yml down
else
    echo "[3/3] Build complete. Use './builder.sh $ENV up' to start services."
fi

echo "========================================"
echo "  Done."
echo "========================================"
