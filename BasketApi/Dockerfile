FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy projects and restore
COPY ["BasketApi/BasketApi.csproj", "BasketApi/"]
COPY ["modules/Base-Api/BaseApi/BaseApi.csproj", "modules/Base-Api/BaseApi/"]
RUN dotnet restore "BasketApi/BasketApi.csproj"

# Copy everything else and build
COPY . .
WORKDIR "/src/BasketApi"
RUN dotnet build "BasketApi.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "BasketApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BasketApi.dll"]
