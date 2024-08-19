# Use .NET SDK to build the project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy the project file and restore dependencies
COPY ["MLAPI/MLAPI.csproj", "MLAPI/"]
RUN dotnet restore "./MLAPI/MLAPI.csproj"

# Copy the rest of the source code and build the project
COPY . .
WORKDIR "/src/MLAPI"
RUN dotnet build "MLAPI.csproj" -c Release -o /app/build

# Publish the project
RUN dotnet publish "MLAPI.csproj" -c Release -o /app/publish

# Use ASP.NET Core runtime to run the API
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

# Copy the published files from the build stage
COPY --from=build /app/publish .

# Set the entry point to run the application
ENTRYPOINT ["dotnet", "MLAPI.dll"]