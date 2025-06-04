FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy csproj and restore dependencies
COPY ["Api.csproj", "./"]
RUN dotnet restore "Api.csproj"

# Copy everything else and build
COPY . .
RUN dotnet build "Api.csproj" -c Debug -o /app/build

# dotnet-efツールをインストール
RUN dotnet tool install --global dotnet-ef
ENV PATH="$PATH:/root/.dotnet/tools"

FROM build AS publish
RUN dotnet publish "Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# 最終イメージをSDKに変更
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS final
WORKDIR /app

# SQL Serverクライアントツールをインストール
RUN apt-get update && apt-get install -y curl gnupg2 && \
    curl https://packages.microsoft.com/keys/microsoft.asc | apt-key add - && \
    curl https://packages.microsoft.com/config/debian/11/prod.list > /etc/apt/sources.list.d/mssql-release.list && \
    apt-get update && \
    ACCEPT_EULA=Y apt-get install -y mssql-tools18 unixodbc-dev && \
    rm -rf /var/lib/apt/lists/*

ENV PATH="$PATH:/opt/mssql-tools18/bin"

# dotnet-efツールをインストール
RUN dotnet tool install --global dotnet-ef
ENV PATH="$PATH:/root/.dotnet/tools"

COPY --from=publish /app/publish .
COPY --from=build /src .

ENTRYPOINT ["dotnet", "run", "--project", "Api.csproj"]