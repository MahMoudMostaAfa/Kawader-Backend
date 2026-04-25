# Build stage: restore and publish the API.
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy project files first to maximize Docker layer caching for restore.
COPY src/Kawadar.Api/Kawadar.Api.csproj                           src/Kawadar.Api/
COPY src/Kawadar.Application/Kawadar.Application.csproj           src/Kawadar.Application/
COPY src/Kawadar.Infrastructure/Kawadar.Infrastructure.csproj     src/Kawadar.Infrastructure/
COPY src/Kawadar.Domain/Kawadar.Domain.csproj                     src/Kawadar.Domain/

RUN dotnet restore src/Kawadar.Api/Kawadar.Api.csproj

COPY src/ src/

# Publish a framework-dependent build.
RUN dotnet publish src/Kawadar.Api/Kawadar.Api.csproj \
    -c Release \
    -o /app/publish \
    --no-restore \
    /p:UseAppHost=false

# Runtime stage: copy only published output for a smaller final image.
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

RUN apt-get update \
    && apt-get install -y --no-install-recommends curl \
    && rm -rf /var/lib/apt/lists/*

# Define a dedicated non-root runtime user.
RUN groupadd --system appgroup \
    && useradd --system --gid appgroup --create-home --home-dir /home/appuser appuser

# Copy only published artifacts with explicit ownership.
COPY --from=build --chown=appuser:appgroup /app/publish .

USER appuser

EXPOSE 8080

ENV ASPNETCORE_URLS=http://+:8080 \
    DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false

HEALTHCHECK --interval=30s --timeout=5s --start-period=20s --retries=5 \
    CMD curl --fail http://127.0.0.1:8080/openapi/v1.json || exit 1

ENTRYPOINT ["dotnet", "Kawadar.Api.dll"]
