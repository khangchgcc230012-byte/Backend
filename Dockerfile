# Build Stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /source

# Copy project files and restore
COPY *.csproj .
RUN dotnet restore

# Copy all source code
COPY . .

# Publish the application
RUN dotnet publish -c Release -o /app --no-restore

# Runtime Stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app .

# Port configuration for Render
ENV ASPNETCORE_URLS=http://+:10000
EXPOSE 10000

# Ensure WebApplication1 matches your actual assembly name
ENTRYPOINT ["dotnet", "WebApplication1.dll"]