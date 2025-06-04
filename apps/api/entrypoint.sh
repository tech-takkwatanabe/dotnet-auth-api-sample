#!/bin/sh
set -e

echo "DBの接続を確認中..."
until /opt/mssql-tools18/bin/sqlcmd -S db -U sa -P "${MSSQL_SA_PASSWORD}" -Q "SELECT 1" &> /dev/null; do
  echo "DBの起動待ち..."
  sleep 3
done

echo "マイグレーションを実行中..."
cd /src/apps/api && dotnet ef database update

echo "アプリケーションを起動中..."
cd /app && exec dotnet Api.dll 