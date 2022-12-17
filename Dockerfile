#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src

COPY ["src/Cesxhin.AnimeManga.Api/", "./Cesxhin.AnimeManga.Api/"]
COPY ["src/references/Cesxhin.AnimeManga.Domain/", "./references/Cesxhin.AnimeManga.Domain/"]
COPY ["src/references/Cesxhin.AnimeManga.Persistence/", "./references/Cesxhin.AnimeManga.Persistence/"]
COPY ["src/references/Cesxhin.AnimeManga.Application/", "./references/Cesxhin.AnimeManga.Application/"]

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
