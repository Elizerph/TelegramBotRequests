#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:6.0-alpine AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["TelegramBotTest/TelegramBotTest.csproj", "TelegramBotTest/"]
RUN dotnet restore "TelegramBotTest/TelegramBotTest.csproj"
COPY . .
WORKDIR "/src/TelegramBotTest"
RUN dotnet build "TelegramBotTest.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TelegramBotTest.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
VOLUME /app/Logs
ENTRYPOINT ["dotnet", "TelegramBotTest.dll"]