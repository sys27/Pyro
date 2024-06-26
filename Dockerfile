﻿FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Pyro.Api/Pyro/Pyro.csproj", "Pyro/"]
COPY ["Pyro.Api/Pyro.Domain/Pyro.Domain.csproj", "Pyro.Domain/"]
COPY ["Pyro.Api/Pyro.Domain.Core/Pyro.Domain.Core.csproj", "Pyro.Domain.Core/"]
COPY ["Pyro.Api/Pyro.Domain.Identity/Pyro.Domain.Identity.csproj", "Pyro.Domain.Identity/"]
COPY ["Pyro.Api/Pyro.Infrastructure/Pyro.Infrastructure.csproj", "Pyro.Infrastructure/"]
COPY ["Pyro.Api/Pyro.sln", "./"]
RUN dotnet restore "Pyro.sln"
COPY ./Pyro.Api/ .
RUN dotnet build "Pyro.sln" -c $BUILD_CONFIGURATION -o /app/build
RUN dotnet publish "Pyro.sln" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM node:lts-alpine AS node
WORKDIR /src
COPY ["Pyro.UI/package.json", "Pyro.UI/package-lock.json", "./"]
RUN npm ci
COPY Pyro.UI .
RUN npm run build

FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS final
RUN addgroup -S pyro && adduser -S pyro -G pyro
USER pyro
EXPOSE 8080
WORKDIR /app
COPY --from=build /app/publish .
COPY --from=node /src/dist/browser ./wwwroot
ENTRYPOINT ["dotnet", "Pyro.dll"]