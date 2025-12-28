# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution file and restore dependencies
COPY FigureProj.sln ./
COPY FigureProj.Common/FigureProj.Common.csproj FigureProj.Common/
COPY FigureProj.Infrastructure/FigureProj.Infrastructure.csproj FigureProj.Infrastructure/
COPY FigureProj.REST/FigureProj.REST.csproj FigureProj.REST/

# Restore NuGet packages
RUN dotnet restore

# Copy all source files
COPY FigureProj.Common/ FigureProj.Common/
COPY FigureProj.Infrastructure/ FigureProj.Infrastructure/
COPY FigureProj.REST/ FigureProj.REST/

# Build the project
WORKDIR /src/FigureProj.REST
RUN dotnet build -c Release -o /app/build --no-restore

# Publish stage
FROM build AS publish
RUN dotnet publish -c Release -o /app/publish --no-restore

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Copy published files
COPY --from=publish /app/publish .

# Set environment variables
ENV ASPNETCORE_URLS=http://0.0.0.0:${PORT:-5000}
ENV ASPNETCORE_ENVIRONMENT=Production

# Expose port (Render will set PORT env variable)
EXPOSE 5000

# Run the application
ENTRYPOINT ["dotnet", "FigureProj.REST.dll"]

