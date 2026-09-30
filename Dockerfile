# Build stage: restore and publish with the full SDK image
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /source

# Copy only the project file first so the restore layer is cached
# and reused when only the source code changes.
COPY src/Exo.WebApi/Exo.WebApi.csproj src/Exo.WebApi/
RUN dotnet restore src/Exo.WebApi/Exo.WebApi.csproj

COPY src/ src/
RUN dotnet publish src/Exo.WebApi/Exo.WebApi.csproj -c Release -o /app --no-restore

# Runtime stage: smaller image with only the ASP.NET runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app .

# Run as the non-root user that ships with the .NET 8 images
USER $APP_UID

ENV ASPNETCORE_HTTP_PORTS=8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "Exo.WebApi.dll"]
