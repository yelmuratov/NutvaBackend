# Base image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

# Build image
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["NutvaCms.API/NutvaCms.API.csproj", "NutvaCms.API/"]
COPY ["NutvaCms.Application/NutvaCms.Application.csproj", "NutvaCms.Application/"]
COPY ["NutvaCms.Domain/NutvaCms.Domain.csproj", "NutvaCms.Domain/"]
COPY ["NutvaCms.Infrastructure/NutvaCms.Infrastructure.csproj", "NutvaCms.Infrastructure/"]
COPY ["NutvaCms.Persistence/NutvaCms.Persistence.csproj", "NutvaCms.Persistence/"]

RUN dotnet restore "NutvaCms.API/NutvaCms.API.csproj"

COPY . .
WORKDIR "/src/NutvaCms.API"
RUN dotnet publish "NutvaCms.API.csproj" -c Release -o /app/publish

# Final image
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "NutvaCms.API.dll"]
