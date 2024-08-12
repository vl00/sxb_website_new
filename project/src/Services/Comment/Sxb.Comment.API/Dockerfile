#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["NuGet.config", "."]
COPY ["src/Services/Comment/Sxb.Comment.API/Sxb.Comment.API.csproj", "src/Services/Comment/Sxb.Comment.API/"]
COPY ["src/Services/Comment/Sxb.Comment.Query.SQL/Sxb.Comment.Query.SQL.csproj", "src/Services/Comment/Sxb.Comment.Query.SQL/"]
COPY ["src/Services/Comment/Sxb.Comment.Common/Sxb.Comment.Common.csproj", "src/Services/Comment/Sxb.Comment.Common/"]
COPY ["src/Tools/Sxb.Framework.Foundation/Sxb.Framework.Foundation.csproj", "src/Tools/Sxb.Framework.Foundation/"]
COPY ["src/Tools/Sxb.Framework.Cache.Redis/src/Sxb.Framework.Cache.Redis.csproj", "src/Tools/Sxb.Framework.Cache.Redis/src/"]
COPY ["src/Tools/Sxb.Framework.AspNetCoreHelper/Sxb.Framework.AspNetCoreHelper.csproj", "src/Tools/Sxb.Framework.AspNetCoreHelper/"]
RUN dotnet restore "src/Services/Comment/Sxb.Comment.API/Sxb.Comment.API.csproj" --configfile "NuGet.config"
COPY . .
WORKDIR "/src/src/Services/Comment/Sxb.Comment.API"
RUN dotnet build "Sxb.Comment.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Sxb.Comment.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Sxb.Comment.API.dll"]