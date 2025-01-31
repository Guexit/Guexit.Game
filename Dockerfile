FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /publish
COPY . .
RUN dotnet publish src/Guexit.Game.WebApi/Guexit.Game.WebApi.csproj -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build-env /publish/out .

USER $APP_UID
ENTRYPOINT ["dotnet", "Guexit.Game.WebApi.dll"]