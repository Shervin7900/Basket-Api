# Build Stage
FROM mcr.microsoft.com/dotnet/sdk:10.0-alpine AS build
WORKDIR /src

# Copy solution and all project files for restore caching
COPY ["BasketApi.sln", "."]
COPY ["BasketApi/BasketApi.csproj", "BasketApi/"]
COPY ["modules/Base-Api/BaseApi/BaseApi.csproj", "modules/Base-Api/BaseApi/"]

# Restore all projects together
RUN dotnet restore "BasketApi.sln"

# Copy full source
COPY . .

# Build and publish
WORKDIR "/src/BasketApi"
RUN dotnet publish "BasketApi.csproj" -c Release -o /app/publish /p:UseAppHost=false --no-restore

# Runtime Stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0-alpine AS final
WORKDIR /app

# Use built-in non-root user for security
USER app

# Copy artifacts
COPY --from=build /app/publish .

# Environment variables
ENV ASPNETCORE_URLS=http://+:8080
# ENV ASPNETCORE_ENVIRONMENT=Production # Removed to allow runtime override

EXPOSE 8080

ENTRYPOINT ["dotnet", "BasketApi.dll"]
