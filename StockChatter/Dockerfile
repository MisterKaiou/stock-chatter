FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
COPY . /src
WORKDIR /src/StockChatter
RUN dotnet restore "StockChatter.csproj"
RUN dotnet build "StockChatter.csproj" -c Release -o /app/build --no-restore

FROM build AS publish
RUN dotnet publish "StockChatter.csproj" -c Release -o /app/publish --no-restore

FROM nginx:alpine as final
WORKDIR /usr/local/nginx/html
COPY --from=publish /app/publish/wwwroot .
COPY StockChatter/nginx.conf /etc/nginx/nginx.conf
