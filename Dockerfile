## Dockerfile for Taxi Server Container Build
## Author : Ozeco-Mmem

# Build Stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS Builder
WORKDIR /src

# Restore Dependency : Import csproj first to Caching projects
COPY ["Server/ServerCore/ServerCore.csproj", "ServerCore/"]
COPY ["Server/GameLogic/GameLogic.csproj", "GameLogic/"]
COPY ["Server/Shared/Shared.csproj", "Shared/"]

# Restore NuGet
RUN dotnet restore "ServerCore/ServerCore.csproj"

# get SourceCode
COPY . .

# build
WORKDIR "/src/ServerCore"
RUN dotnet publish "ServerCore.csproj" -c Debug -o /app/publish /p:UseAppHost=false

# Runtime Stage
FROM mcr.microsoft.com/dotnet/runtime:10.0 AS Runtime
WORKDIR /app

# Copy build result
COPY --from=Builder /app/publish .

# Server Port Open
EXPOSE 7777

#Entrypoint
ENTRYPOINT ["dotnet", "ServerCore.dll"]
