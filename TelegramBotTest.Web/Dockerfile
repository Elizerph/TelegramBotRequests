#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0-alpine AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["TelegramBotTest.Web/TelegramBotTest.Web.csproj", "TelegramBotTest.Web/"]
RUN dotnet restore "TelegramBotTest.Web/TelegramBotTest.Web.csproj"
COPY . .
WORKDIR "/src/TelegramBotTest.Web"
RUN dotnet build "TelegramBotTest.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TelegramBotTest.Web.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
VOLUME /app/Data
ENTRYPOINT ["dotnet", "TelegramBotTest.Web.dll"]