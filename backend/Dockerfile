FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Kanban.WebApi/Kanban.WebApi.csproj", "Kanban.WebApi/"]
COPY ["Kanban.Application/Kanban.Application.csproj", "Kanban.Application/"]
COPY ["Kanban.Domain/Kanban.Domain.csproj", "Kanban.Domain/"]
COPY ["Kanban.Infrastructure/Kanban.Infrastructure.csproj", "Kanban.Infrastructure/"]
RUN dotnet restore "./Kanban.WebApi/Kanban.WebApi.csproj"
COPY . .
WORKDIR "/src/Kanban.WebApi"
RUN dotnet build "./Kanban.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Kanban.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Kanban.WebApi.dll"]
