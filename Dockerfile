# ── Build stage ────────────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

# Copy solution and project files first (layer cache for restore)
COPY DevicesAPI.slnx ./
COPY src/DevicesAPI.Domain/DevicesAPI.Domain.csproj             src/DevicesAPI.Domain/
COPY src/DevicesAPI.Application/DevicesAPI.Application.csproj   src/DevicesAPI.Application/
COPY src/DevicesAPI.Infrastructure/DevicesAPI.Infrastructure.csproj src/DevicesAPI.Infrastructure/
COPY src/DevicesAPI.API/DevicesAPI.API.csproj                   src/DevicesAPI.API/

RUN dotnet restore src/DevicesAPI.API/DevicesAPI.API.csproj

# Copy remaining source and publish
COPY src/ src/
RUN dotnet publish src/DevicesAPI.API/DevicesAPI.API.csproj \
    -c Release \
    -o /app/publish \
    --no-restore

# ── Runtime stage ──────────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "DevicesAPI.API.dll"]