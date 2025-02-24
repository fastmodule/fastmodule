# Use ASP.NET Core runtime as base
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8082

# Use .NET SDK for building
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy project file and restore dependencies
COPY ./src/FastModule.Host.Api/FastModule.Host.Api.csproj /src/FastModule.Host.Api/
RUN dotnet restore /src/FastModule.Host.Api/FastModule.Host.Api.csproj

# Copy the entire source code
COPY ./src /src/

# Debugging: Check if project file exists
RUN ls -l /src/FastModule.Host.Api

# Set working directory for building
WORKDIR /src/FastModule.Host.Api
RUN dotnet build FastModule.Host.Api.csproj -c $BUILD_CONFIGURATION -o /app/build

# Publish the app
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish FastModule.Host.Api.csproj -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Final image setup
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FastModule.Host.Api.dll"]
