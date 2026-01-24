FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy solution and project files (restore cache)
COPY *.sln ./
COPY src/LibraryManagement.API/LibraryManagement.API.csproj src/LibraryManagement.API/
COPY src/LibraryManagement.Application/LibraryManagement.Application.csproj src/LibraryManagement.Application/
COPY src/LibraryManagement.Domain/LibraryManagement.Domain.csproj src/LibraryManagement.Domain/
COPY src/LibraryManagement.Infrastructure/LibraryManagement.Infrastructure.csproj src/LibraryManagement.Infrastructure/

# Restore only the startup project (restores the full graph)
RUN dotnet restore src/LibraryManagement.API/LibraryManagement.API.csproj

# Copy the rest and publish
COPY . .
RUN dotnet publish src/LibraryManagement.API/LibraryManagement.API.csproj -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "LibraryManagement.API.dll"]
