﻿FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["LevelUp.Microservice.Auth/LevelUp.Microservice.Auth.csproj", "LevelUp.Microservice.Auth/"]
RUN dotnet restore "LevelUp.Microservice.Auth/LevelUp.Microservice.Auth.csproj"
COPY . .
WORKDIR "/src/LevelUp.Microservice.Auth"
RUN dotnet build "LevelUp.Microservice.Auth.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "LevelUp.Microservice.Auth.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "LevelUp.Microservice.Auth.dll"]
