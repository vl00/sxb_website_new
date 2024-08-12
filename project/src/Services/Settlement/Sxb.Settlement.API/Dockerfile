#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim AS build
WORKDIR /src
COPY ["src/Services/Settlement/Sxb.Settlement.API/", "src/Services/Settlement/Sxb.Settlement.API/"]
COPY ["src/Shared/", "src/Shared/"]
COPY ["src/Tools/", "src/Tools/"]
RUN dotnet restore "src/Services/Settlement/Sxb.Settlement.API/Sxb.Settlement.API.csproj"
WORKDIR "/src/src/Services/Settlement/Sxb.Settlement.API"
RUN dotnet build "Sxb.Settlement.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Sxb.Settlement.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV TZ="Asia/Shanghai"
ENTRYPOINT ["dotnet", "Sxb.Settlement.API.dll"]