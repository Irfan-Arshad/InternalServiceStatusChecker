# ===== Build Stage =====
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# copy solution and restore
COPY InternalServiceStatusChecker.sln ./
COPY InternalServiceStatusChecker ./InternalServiceStatusChecker/

RUN dotnet restore

# build and publish
RUN dotnet publish ./InternalServiceStatusChecker/InternalServiceStatusChecker.csproj -c Release -o /app/publish

# ===== Runtime Stage =====
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

# expose port (change if your API runs on a different port)
EXPOSE 8080

ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "InternalServiceStatusChecker.dll"]
