# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["FundingApi/PDS.ViewYourFunding.Data.Api.csproj", "FundingApi/"]
COPY ["Core/PDS.ViewYourFunding.Data.Core.csproj", "Core/"]
COPY ["Interfaces/PDS.ViewYourFunding.Data.Interfaces.csproj", "Interfaces/"]
COPY ["PDS.VYF.Data.Services/PDS.VYF.Data.Services.csproj", "PDS.VYF.Data.Services/"]
COPY ["Services/PDS.ViewYourFunding.Data.Services.csproj", "Services/"]

ARG PDS_FEED_URL="https://sfa-gov-uk.pkgs.visualstudio.com/9fd46a3f-eada-41dc-86e9-36332aeb8ef1/_packaging/PDS/nuget/v3/index.json"
ARG PDS_FEED_USERNAME="localuser"
ARG PDS_FEED_PAT

RUN dotnet nuget add source ${PDS_FEED_URL} \
	--name "Private Source" \
	--username ${PDS_FEED_USERNAME} \
	--password ${PDS_FEED_PAT} \
	--store-password-in-clear-text

RUN dotnet restore "./FundingApi/PDS.ViewYourFunding.Data.Api.csproj"
COPY . .
WORKDIR "/src/FundingApi"
RUN dotnet build "./PDS.ViewYourFunding.Data.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./PDS.ViewYourFunding.Data.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PDS.ViewYourFunding.Data.Api.dll"]
