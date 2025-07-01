# Étape de build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

COPY *.sln ./
COPY wacdoprojet/*.csproj ./wacdoprojet/
RUN dotnet restore

COPY . ./
RUN dotnet publish -c Release -o out

# Étape runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/out .

ENTRYPOINT ["dotnet", "wacdoprojet.dll"]
