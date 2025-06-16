# dotnet-auth-api-sample

ASP.NET Core Web API を使用した認証機能のサンプルプロジェクトです。
JWT (JSON Web Token) ベースの認証を実装し、ユーザー登録、ログイン、リフレッシュトークン、セキュアなエンドポイントアクセスなどの基本的な機能を提供します。

## ✨ 特徴

- **JWT認証:** セキュアなトークンベース認証。
- **リフレッシュトークン:** アクセストークンの有効期限が切れた後、新しいアクセストークンを安全に取得。
- **パスワードハッシュ化:** BCrypt を使用した安全なパスワード保存。
- **値オブジェクト (VO):** ドメイン駆動設計の原則に基づいた型安全なデータ表現。
- **Entity Framework Core:** SQL Server を使用したデータ永続化。
- **Redis:** リフレッシュトークンの保存など、キャッシュ/セッション管理に利用。
- **Swagger/OpenAPI:** APIドキュメントの自動生成とテストUI。
- **環境変数管理:** `.env` ファイルによる設定管理。
- **HTTPS対応:** 開発用の自己署名証明書を使用したHTTPS通信。

## 🛠 技術スタック

- .NET (ASP.NET Core Web API)
- Entity Framework Core
- SQL Server
- Redis
- JWT (Microsoft.AspNetCore.Authentication.JwtBearer)
- BCrypt.Net-Next
- UUIDNext
- Swashbuckle.AspNetCore (Swagger)
- DotNetEnv

## 🚀 セットアップと実行

### 前提条件
- [.NET SDK](https://dotnet.microsoft.com/download) (バージョンはプロジェクトファイルを参照)
- [Docker](https://www.docker.com/) (SQL Server と Redis の実行に推奨)
- SQL Server (ローカルまたはDocker)
- Redis (ローカルまたはDocker)

### 1. リポジトリのクローン
```bash
git clone https://github.com/your-username/dotnet-auth-api-sample.git
cd dotnet-auth-api-sample
```

### 2. 環境変数の設定
プロジェクトルート（`dotnet-auth-api-sample` ディレクトリ）に `.env` ファイルを作成し、以下の内容を参考に設定します。
`apps/api/.env.example` があればそれをコピーして編集してください。
```env
# JWT Settings
JWT_SECRET="your-super-secret-jwt-key-at-least-32-characters-long" # 32文字以上のランダムな文字列
JWT_ISSUER="your-api-issuer"
JWT_AUDIENCE="your-api-audience"
ACCESS_TOKEN_EXPIRE_SECONDS=900 # 15 minutes
REFRESH_TOKEN_EXPIRE_SECONDS=604800 # 7 days

# Redis
REDIS_CONNECTION_STRING="localhost:6379"

# SSL (開発用 - Docker Compose を使用する場合、パスはコンテナ内を指します)
# SSL_CERT_PATH="/https/localhost-cert.pem"
# SSL_KEY_PATH="/https/localhost-key.pem"
```
**注意:** `JWT_SECRET` は必ず推測困難な32文字以上のランダムな文字列に変更してください。

### 3. 開発用SSL証明書の準備 (オプション)
HTTPSでローカル開発を行う場合、自己署名証明書が必要です。
プロジェクトルートに `.certificate` ディレクトリを作成し、そこに `localhost-cert.pem` と `localhost-key.pem` を配置してください。

OpenSSL を使用して証明書を生成する例:
```bash
mkdir .certificate
openssl req -x509 -newkey rsa:2048 -keyout .certificate/localhost-key.pem -out .certificate/localhost-cert.pem -sha256 -days 365 -nodes -subj "/CN=localhost"
```
`docker-compose.yml` (もしあれば) で、この `.certificate` ディレクトリがコンテナ内の `/https` にマウントされるように設定されています。
環境変数 `SSL_CERT_PATH` と `SSL_KEY_PATH` でこれらのファイルのパスを指定します (Docker未使用時はホストOSの絶対パス、Docker使用時はコンテナ内のパス)。

### 4. データベースのセットアップ
SQL Server インスタンスが実行されていることを確認してください。
`apps/api` ディレクトリに移動し、Entity Framework Core のマイグレーションを実行してデータベーススキーマを作成します。
```bash
cd apps/api
dotnet ef database update
```

### 5. アプリケーションの実行

#### a) dotnet CLI を使用する場合:
`apps/api` ディレクトリで以下を実行します。
```bash
dotnet run
```
APIは `http://localhost:8080` および `https://localhost:8443` (証明書設定時) で利用可能になります。

#### b) Docker Compose を使用する場合 (推奨):
プロジェクトルートに `docker-compose.yml` がある場合、以下を実行します。
```bash
docker-compose up --build
```

## 📄 APIドキュメント (Swagger)

アプリケーション実行後、以下のURLにアクセスすると Swagger UI が表示され、APIの仕様確認とテストが可能です。

**Swagger UI:** https://localhost:8443/swagger/index.html

### OpenAPI 仕様ファイルの生成
ローカルでAPIドキュメントファイル (`openapi.json`) を生成するには、以下のコマンドを実行します。
```bash
cd apps/api
mkdir Docs
curl -k https://localhost:8443/swagger/v1/swagger.json -o Docs/openapi.json
```