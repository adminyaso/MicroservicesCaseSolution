# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:9.0-alpine AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

USER root
RUN apk add --no-cache icu-libs
# .NET için Invariant mode'u kapatmak
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=0
# Uygulamanızın çalıştırılacağı kullanıcıya geri dönüyoruz
USER $APP_UID

# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:9.0-alpine AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Services/LogService/LogService.API/LogService.API.csproj", "src/Services/LogService/LogService.API/"]
COPY ["src/Shared/Logging/Logging.csproj", "src/Shared/Logging/"]
COPY ["src/Services/LogService/LogService.Application/LogService.Application.csproj", "src/Services/LogService/LogService.Application/"]
COPY ["src/Services/LogService/LogService.Domain/LogService.Domain.csproj", "src/Services/LogService/LogService.Domain/"]
COPY ["src/Services/LogService/LogService.Infrastructure/LogService.Infrastructure.csproj", "src/Services/LogService/LogService.Infrastructure/"]
RUN dotnet restore "src/Services/LogService/LogService.API/LogService.API.csproj"
COPY . .
WORKDIR "/src/src/Services/LogService/LogService.API"
RUN dotnet build "./LogService.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./LogService.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "LogService.API.dll"]