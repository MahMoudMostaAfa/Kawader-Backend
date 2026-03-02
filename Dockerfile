# ─── Build Stage ───
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy project files first for better layer caching
COPY src/Kawadar.Api/Kawadar.Api.csproj                          src/Kawadar.Api/
COPY src/Kawadar.Application/Kawadar.Application.csproj           src/Kawadar.Application/
COPY src/Kawadar.Infrastructure/Kawadar.Infrastructure.csproj     src/Kawadar.Infrastructure/
COPY src/Kawadar.Domain/Kawadar.Domain.csproj                     src/Kawadar.Domain/

# Restore dependencies
RUN dotnet restore src/Kawadar.Api/Kawadar.Api.csproj

# Copy all source code
COPY src/ src/

# Build and publish
RUN dotnet publish src/Kawadar.Api/Kawadar.Api.csproj \
    -c Release \
    -o /app/publish \
    --no-restore

# ─── Runtime Stage ───
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

# Create a non-root user
RUN adduser --disabled-password --gecos "" appuser

# Copy published output
COPY --from=build /app/publish .

# Create Logs directory
RUN mkdir -p /app/Logs && chown -R appuser:appuser /app

# Switch to non-root user
USER appuser

# Expose HTTP port
EXPOSE 8080

# Health check
HEALTHCHECK --interval=30s --timeout=5s --start-period=10s --retries=3 \
    CMD curl -f http://localhost:8080/metrics || exit 1

ENTRYPOINT ["dotnet", "Kawadar.Api.dll"]
