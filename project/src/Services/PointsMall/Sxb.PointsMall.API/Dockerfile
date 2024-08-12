#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim AS build
WORKDIR /src
COPY ["src/Services/PointsMall/Sxb.PointsMall.API/Sxb.PointsMall.API.csproj", "src/Services/PointsMall/Sxb.PointsMall.API/"]
RUN dotnet restore "src/Services/PointsMall/Sxb.PointsMall.API/Sxb.PointsMall.API.csproj"
COPY . .
WORKDIR "/src/src/Services/PointsMall/Sxb.PointsMall.API"
RUN dotnet build "Sxb.PointsMall.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Sxb.PointsMall.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Sxb.PointsMall.API.dll"]