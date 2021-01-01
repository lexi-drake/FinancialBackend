# Nuget restore
FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /src
COPY *.sln .
COPY Tests/*.csproj Tests/
COPY WebService/*.csproj WebService/
RUN dotnet restore
COPY . .

# Test
FROM build AS testing
WORKDIR /src/WebService
RUN dotnet build
WORKDIR /src/Tests
RUN dotnet test

# Publish
FROM build AS publish
WORKDIR /src/WebService
RUN dotnet publish -c Release /src/publish

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS runtime
WORKDIR /app
COPY --from=publish /src/publish
CMD ASPNETCORE_URLS=http://*:$PORT dotnet WebService.dll