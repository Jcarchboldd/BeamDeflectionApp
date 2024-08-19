FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["BeamDeflectionBlazorApp/BeamDeflectionBlazorApp.csproj", "BeamDeflectionBlazorApp/"]
RUN dotnet restore "BeamDeflectionBlazorApp/BeamDeflectionBlazorApp.csproj"

COPY BeamDeflectionBlazorApp/. ./BeamDeflectionBlazorApp/
WORKDIR "/src/BeamDeflectionBlazorApp"
RUN dotnet publish "BeamDeflectionBlazorApp.csproj" -c Release -o /app/publish

FROM nginx:alpine AS final
WORKDIR /usr/share/nginx/html
COPY --from=build /app/publish/wwwroot .