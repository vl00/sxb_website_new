#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim AS build
WORKDIR /src
COPY ["src/Services/Recommend/Sxb.Recommend.API/Sxb.Recommend.API.csproj", "src/Services/Recommend/Sxb.Recommend.API/"]
COPY ["src/Services/Recommend/Sxb.Recommend.Infrastructure/Sxb.Recommend.Infrastructure.csproj", "src/Services/Recommend/Sxb.Recommend.Infrastructure/"]
COPY ["src/Shared/Sxb.Infrastructure.Core/Sxb.Infrastructure.Core.csproj", "src/Shared/Sxb.Infrastructure.Core/"]
COPY ["src/Shared/Sxb.Domain.Abstractions/Sxb.Domain.Abstractions.csproj", "src/Shared/Sxb.Domain.Abstractions/"]
COPY ["src/Services/Recommend/Sxb.Recommend.Domain/Sxb.Recommend.Domain.csproj", "src/Services/Recommend/Sxb.Recommend.Domain/"]
COPY ["src/Tools/Sxb.Framework.AspNetCoreHelper/Sxb.Framework.AspNetCoreHelper.csproj", "src/Tools/Sxb.Framework.AspNetCoreHelper/"]
COPY ["src/Tools/Sxb.Framework.Foundation/Sxb.Framework.Foundation.csproj", "src/Tools/Sxb.Framework.Foundation/"]
COPY ["src/Tools/Sxb.Framework.Cache.Redis/src/Sxb.Framework.Cache.Redis.csproj", "src/Tools/Sxb.Framework.Cache.Redis/src/"]
COPY ["src/Services/Recommend/Sxb.Recommend.Application/Sxb.Recommend.Application.csproj", "src/Services/Recommend/Sxb.Recommend.Application/"]
RUN dotnet restore "src/Services/Recommend/Sxb.Recommend.API/Sxb.Recommend.API.csproj"
COPY . .
WORKDIR "/src/src/Services/Recommend/Sxb.Recommend.API"
RUN dotnet build "Sxb.Recommend.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Sxb.Recommend.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Sxb.Recommend.API.dll"]