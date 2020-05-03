FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-alpine AS base

RUN apk add icu-libs
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT false

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-alpine AS build
WORKDIR /src

# Copy csproj and restore as distinct layers
# https://andrewlock.net/optimising-asp-net-core-apps-in-docker-avoiding-manually-copying-csproj-files-part-2/
COPY */*.csproj ./
RUN for file in $(ls *.csproj); do mkdir -p ${file%.*}/ && mv $file ${file%.*}/; done

WORKDIR /src/CloudMall.Services.Storage
RUN dotnet restore

# copy everything and build
COPY . .
RUN dotnet publish -c Release -o out

# build runtime image
FROM base AS final

LABEL Maintainer="WeihanLi"
WORKDIR /app
COPY --from=build /src/CloudMall.Services.Storage/out .

EXPOSE 80
ENTRYPOINT ["dotnet", "CloudMall.Services.Storage.dll"]
