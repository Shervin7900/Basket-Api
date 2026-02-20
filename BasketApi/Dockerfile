# Build Stage
FROM mcr.microsoft.com/dotnet/sdk:10.0-alpine AS build
WORKDIR /src

# Copy project files for caching
COPY ["BasketApi/BasketApi.csproj", "BasketApi/"]
COPY ["modules/Base-Api/BaseApi/BaseApi.csproj", "modules/Base-Api/BaseApi/"]

# Restore dependencies
RUN dotnet restore "BasketApi/BasketApi.csproj"

# Copy source and build
COPY . .
WORKDIR "/src/BasketApi"
RUN dotnet publish "BasketApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime Stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0-alpine AS final
WORKDIR /app

# Non-root user for security
RUN addgroup -S app && adduser -S app -G app
USER app

# Copy artifacts
COPY --from=build /app/publish .

# Environment variables
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

EXPOSE 8080

ENTRYPOINT ["dotnet", "BasketApi.dll"]
