# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution and projects
COPY *.sln ./
COPY E-Commerce.API/*.csproj E-Commerce.API/
COPY E-Commerce.Application/*.csproj E-Commerce.Application/
COPY E-Commerce.Core/*.csproj E-Commerce.Core/
COPY E-Commerce.Infrastructure/*.csproj E-Commerce.Infrastructure/

# Restore
RUN dotnet restore

# Copy the rest of the code
COPY . .

# Publish
WORKDIR /src/E-Commerce.API
RUN dotnet publish -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "E-Commerce.API.dll"]
