FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0.403-alpine3.20 AS build
ARG BUILD_CONFIGURATION=Release
ARG TARGETOS
ARG TARGETARCH

WORKDIR /src
COPY ["Pyro.Api/Pyro/Pyro.csproj", "Pyro/"]
COPY ["Pyro.Api/Pyro.ApiTests/Pyro.ApiTests.csproj", "Pyro.ApiTests/"]
COPY ["Pyro.Api/Pyro.Contracts/Pyro.Contracts.csproj", "Pyro.Contracts/"]
COPY ["Pyro.Api/Pyro.Domain/Pyro.Domain.csproj", "Pyro.Domain/"]
COPY ["Pyro.Api/Pyro.Domain.UnitTests/Pyro.Domain.UnitTests.csproj", "Pyro.Domain.UnitTests/"]
COPY ["Pyro.Api/Pyro.Infrastructure/Pyro.Infrastructure.csproj", "Pyro.Infrastructure/"]

COPY ["Pyro.Api/Pyro.Domain.Shared/Pyro.Domain.Shared.csproj", "Pyro.Domain.Shared/"]
COPY ["Pyro.Api/Pyro.Infrastructure.Shared/Pyro.Infrastructure.Shared.csproj", "Pyro.Infrastructure.Shared/"]

COPY ["Pyro.Api/Pyro.Domain.Identity/Pyro.Domain.Identity.csproj", "Pyro.Domain.Identity/"]
COPY ["Pyro.Api/Pyro.Domain.Identity.UnitTests/Pyro.Domain.Identity.UnitTests.csproj", "Pyro.Domain.Identity.UnitTests/"]
COPY ["Pyro.Api/Pyro.Infrastructure.Identity/Pyro.Infrastructure.Identity.csproj", "Pyro.Infrastructure.Identity/"]

COPY ["Pyro.Api/Pyro.Domain.Issues/Pyro.Domain.Issues.csproj", "Pyro.Domain.Issues/"]
COPY ["Pyro.Api/Pyro.Domain.Issues.UnitTests/Pyro.Domain.Issues.UnitTests.csproj", "Pyro.Domain.Issues.UnitTests/"]
COPY ["Pyro.Api/Pyro.Infrastructure.Issues/Pyro.Infrastructure.Issues.csproj", "Pyro.Infrastructure.Issues/"]

COPY ["Pyro.Api/Pyro.sln", "./"]
COPY ["Pyro.Api/Directory.Build.targets", "./"]
COPY ["Pyro.Api/Directory.Packages.props", "./"]
RUN dotnet restore "Pyro.sln" --runtime $TARGETOS-$TARGETARCH

COPY ./Pyro.Api/ .
RUN dotnet build "Pyro.sln" --nologo --no-restore -c $BUILD_CONFIGURATION

FROM build AS test
ENV BUILD_CONFIGURATION=$BUILD_CONFIGURATION
ENTRYPOINT ["sh", "-c", "dotnet test Pyro.sln --nologo --no-restore --no-build -c $BUILD_CONFIGURATION --logger \"GitHubActions;summary.includePassedTests=true;summary.includeSkippedTests=true\" -- RunConfiguration.CollectSourceInformation=true"]

FROM build AS publish
RUN dotnet publish "Pyro/Pyro.csproj" \
    --nologo --no-restore --no-build -c $BUILD_CONFIGURATION \
    -o /app/publish /p:UseAppHost=false

FROM --platform=$BUILDPLATFORM node:22.9.0-alpine AS node
WORKDIR /src
COPY ["Pyro.UI/package.json", "Pyro.UI/package-lock.json", "./"]
RUN npm ci
COPY Pyro.UI .
RUN npm run build

FROM mcr.microsoft.com/dotnet/aspnet:8.0.10-alpine3.20 AS final
EXPOSE 80
HEALTHCHECK --interval=5s --timeout=5s CMD wget http://localhost/health -q -O - > /dev/null 2>&1

ENV ASPNETCORE_ENVIRONMENT="Production"
ENV ASPNETCORE_ConnectionStrings__DefaultConnection="Data Source=/data/pyro.db"
ENV ASPNETCORE_Git__BasePath="/data"
ENV ASPNETCORE_URLS="http://+:80"

RUN addgroup -S pyro && adduser -S pyro -G pyro

VOLUME /data
RUN mkdir /data && chown pyro:pyro /data

USER pyro
WORKDIR /app
COPY --from=publish /app/publish .
COPY --from=node /src/dist/browser ./wwwroot
ENTRYPOINT ["dotnet", "Pyro.dll"]
