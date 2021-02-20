#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0.100-preview.1-alpine AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0.100-preview.1-alpine AS build
WORKDIR /src
COPY ["pixit/Server/pixit.Server.csproj", "pixit/Server/"]
COPY ["pixit/Shared/pixit.Shared.csproj", "pixit/Shared/"]
COPY ["pixit/Client/pixit.Client.csproj", "pixit/Client/"]
RUN dotnet restore "pixit/Server/pixit.Server.csproj"
COPY . .
WORKDIR "/src/pixit/Server"
RUN dotnet build "pixit.Server.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "pixit.Server.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "pixit.Server.dll"]