FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["LAB1_WEB_API/LAB1_WEB_API.csproj", "LAB1_WEB_API/"]
RUN dotnet restore "LAB1_WEB_API/LAB1_WEB_API.csproj"
COPY . .
WORKDIR "/src/LAB1_WEB_API"
RUN dotnet build "LAB1_WEB_API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "LAB1_WEB_API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "LAB1_WEB_API.dll"]
