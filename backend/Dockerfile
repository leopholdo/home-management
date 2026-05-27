# =========================
# BUILD STAGE
# =========================
FROM mcr.microsoft.com/dotnet/sdk:10.0-preview AS build

WORKDIR /src

# Copy solution
COPY *.sln .

# Copy csproj files
COPY HouseManagement.API/*.csproj HouseManagement.API/
COPY HouseManagement.Application/*.csproj HouseManagement.Application/
COPY HouseManagement.Domain/*.csproj HouseManagement.Domain/
COPY HouseManagement.Infrastructure/*.csproj HouseManagement.Infrastructure/

# Restore dependencies
RUN dotnet restore HouseManagement.API/HouseManagement.API.csproj

# Copy everything
COPY . .

# Publish
WORKDIR /src/HouseManagement.API

RUN dotnet publish -c Release -o /app/publish

# =========================
# RUNTIME STAGE
# =========================
FROM mcr.microsoft.com/dotnet/aspnet:10.0-preview

WORKDIR /app

COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080

EXPOSE 8080

ENTRYPOINT ["dotnet", "HouseManagement.API.dll"]