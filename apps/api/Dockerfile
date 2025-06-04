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

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS final
WORKDIR /app

COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "watch", "run", "--project", "./Api.csproj", "--urls", "http://*:8080;https://*:8443"]
