# Stage 1: Base runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
# Default ASP.NET Core port for .NET 8. 
# Render will automatically detect this port or you can configure it.
ENV ASPNETCORE_HTTP_PORTS=8080
EXPOSE 8080

# Stage 2: SDK image for compiling the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy project files first to leverage Docker layer caching
COPY ["ListoWeb.API/ListoAPI.API.csproj", "ListoWeb.API/"]
COPY ["ListoAPI.Aplication/ListoAPI.Aplication.csproj", "ListoAPI.Aplication/"]
COPY ["ListoAPI.DTO/ListoAPI.DTO.csproj", "ListoAPI.DTO/"]

# Restore dependencies
RUN dotnet restore "ListoWeb.API/ListoAPI.API.csproj"

# Copy the rest of the source code
COPY . .

# Build the project
WORKDIR "/src/ListoWeb.API"
RUN dotnet build "ListoAPI.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Stage 3: Publish stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "ListoAPI.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Stage 4: Final stage
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Run the app. We use a shell wrapper to bind to Render's dynamic PORT environment variable if provided, falling back to 8080.
ENTRYPOINT ["sh", "-c", "ASPNETCORE_HTTP_PORTS=${PORT:-8080} dotnet ListoAPI.API.dll"]
