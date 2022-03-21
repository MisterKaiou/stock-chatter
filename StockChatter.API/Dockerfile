FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 as build
COPY . /src
WORKDIR /src/StockChatter.API
RUN dotnet restore
RUN dotnet build -c Release -o /app/build --no-restore

FROM build as publish
RUN dotnet publish -c Release -o /app/publish --no-restore

FROM base as final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "StockChatter.API.dll"]
