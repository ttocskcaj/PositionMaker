FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["PositionMakerCli/PositionMakerCli.csproj", "PositionMakerCli/"]
RUN dotnet restore "PositionMakerCli/PositionMakerCli.csproj"
COPY . .
WORKDIR "/src/PositionMakerCli"
RUN dotnet build "PositionMakerCli.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PositionMakerCli.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PositionMakerCli.dll"]
