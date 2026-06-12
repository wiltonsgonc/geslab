# Multi-stage build for LabScheduler.Api

# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy all .csproj files so the solution restore works
COPY *.slnx ./
COPY NuGet.config ./
COPY src/LabScheduler.Domain/*.csproj src/LabScheduler.Domain/
COPY src/LabScheduler.Infrastructure/*.csproj src/LabScheduler.Infrastructure/
COPY src/LabScheduler.Api/*.csproj src/LabScheduler.Api/
COPY src/LabScheduler.Web/LabScheduler.Web/*.csproj src/LabScheduler.Web/LabScheduler.Web/
COPY src/LabScheduler.Web/LabScheduler.Web.Client/*.csproj src/LabScheduler.Web/LabScheduler.Web.Client/
RUN dotnet restore

# Copy everything and publish only the API
COPY . .
RUN dotnet publish src/LabScheduler.Api/LabScheduler.Api.csproj -c Release -o /app

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
EXPOSE 5000
EXPOSE 5001

RUN apt-get update && apt-get upgrade -y && apt-get install -y --no-install-recommends curl && rm -rf /var/lib/apt/lists/*

HEALTHCHECK --interval=30s --timeout=10s --start-period=5s --retries=3 \
  CMD curl --silent --fail http://localhost:5000/ || exit 1

COPY --from=build /app .
USER app
ENTRYPOINT ["dotnet", "LabScheduler.Api.dll"]
