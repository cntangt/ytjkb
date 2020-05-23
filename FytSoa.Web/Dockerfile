#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["FytSoa.Web/FytSoa.Web.csproj", "FytSoa.Web/"]
COPY ["FytSoa.Service/FytSoa.Service.csproj", "FytSoa.Service/"]
COPY ["FytSoa.Common/FytSoa.Common.csproj", "FytSoa.Common/"]
COPY ["FytSoa.Core/FytSoa.Core.csproj", "FytSoa.Core/"]
COPY ["FytSoa.Extensions/FytSoa.Extensions.csproj", "FytSoa.Extensions/"]
COPY ["FytSoa.Tasks/FytSoa.Tasks.csproj", "FytSoa.Tasks/"]
RUN dotnet restore "FytSoa.Web/FytSoa.Web.csproj"
COPY . .
WORKDIR "/src/FytSoa.Web"
RUN dotnet build "FytSoa.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "FytSoa.Web.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FytSoa.Web.dll"]