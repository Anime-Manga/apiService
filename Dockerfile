#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src

COPY ["src/Cesxhin.AnimeManga/Cesxhin.AnimeManga.Api/", "./Cesxhin.AnimeManga.Api/"]
COPY ["src/Cesxhin.AnimeManga/Cesxhin.AnimeManga.Domain/", "./Cesxhin.AnimeManga.Domain/"]
COPY ["src/Cesxhin.AnimeManga/Cesxhin.AnimeManga.Persistence/", "./Cesxhin.AnimeManga.Persistence/"]
COPY ["src/Cesxhin.AnimeManga/Cesxhin.AnimeManga.Application/", "./Cesxhin.AnimeManga.Application/"]

RUN dotnet restore "./Cesxhin.AnimeManga.Api/Cesxhin.AnimeManga.Api.csproj"

COPY . .
WORKDIR "./Cesxhin.AnimeManga.Api"

RUN dotnet build "Cesxhin.AnimeManga.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Cesxhin.AnimeManga.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Cesxhin.AnimeManga.Api.dll"]
