#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["NuGet.config", "."]
COPY ["src/Services/SignActicity/Sxb.SignActivity.API/Sxb.SignActivity.API.csproj", "src/Services/SignActicity/Sxb.SignActivity.API/"]
RUN dotnet restore "src/Services/SignActicity/Sxb.SignActivity.API/Sxb.SignActivity.API.csproj"
COPY . .
WORKDIR "/src/src/Services/SignActicity/Sxb.SignActivity.API"
RUN dotnet build "Sxb.SignActivity.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Sxb.SignActivity.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Sxb.SignActivity.API.dll"]