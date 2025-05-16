# Base image (.NET 9 Runtime)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80
ENV ASPNETCORE_URLS=http://+:80

# Build image (.NET 9 SDK)
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy project files
COPY ["NutvaCms.API/NutvaCms.API.csproj", "NutvaCms.API/"]
COPY ["NutvaCms.Application/NutvaCms.Application.csproj", "NutvaCms.Application/"]
COPY ["NutvaCms.Domain/NutvaCms.Domain.csproj", "NutvaCms.Domain/"]
COPY ["NutvaCms.Infrastructure/NutvaCms.Infrastructure.csproj", "NutvaCms.Infrastructure/"]
COPY ["NutvaCms.Persistence/NutvaCms.Persistence.csproj", "NutvaCms.Persistence/"]

# Restore
RUN dotnet restore "NutvaCms.API/NutvaCms.API.csproj"

# Copy source
COPY . .

# Ensure configs and static files are included
COPY ["NutvaCms.API/appsettings.json", "NutvaCms.API/"]
COPY ["NutvaCms.API/wwwroot", "NutvaCms.API/wwwroot"]

# Publish
WORKDIR "/src/NutvaCms.API"
RUN dotnet publish "NutvaCms.API.csproj" -c Release -o /app/publish

# Final runtime image
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "NutvaCms.API.dll"]
