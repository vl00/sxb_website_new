#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0-buster-slim AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim AS build
WORKDIR /src
COPY ["src/Services/Static/Sxb.Static.BackgroundTask/", "src/Services/Static/Sxb.Static.BackgroundTask/"]
COPY ["src/Shared/", "src/Shared/"]
COPY ["src/Tools/", "src/Tools/"]
RUN dotnet restore "src/Services/Static/Sxb.Static.BackgroundTask/Sxb.Static.BackgroundTask.csproj"
WORKDIR "/src/src/Services/Static/Sxb.Static.BackgroundTask/"
RUN dotnet build "Sxb.Static.BackgroundTask.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Sxb.Static.BackgroundTask.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Sxb.Static.BackgroundTask.dll"]