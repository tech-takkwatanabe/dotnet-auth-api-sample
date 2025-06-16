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
git clone https://github.com/tech-takkwatanabe/dotnet-auth-api-sample.git
cd dotnet-auth-api-sample
```

### 2. 環境変数の設定
`apps/api/.env.example` をコピーして`apps/api/.env`を作成し、内容を編集してください。
`USER_SECRET_ID`には以下のコマンドで生成した値をコピーしてください。

```bash
cd apps/api
dotnet user-secrets init
```

**注意:** `JWT_SECRET` は必ず推測困難な32文字以上のランダムな文字列に変更してください。

### 3. 開発用SSL証明書の準備
HTTPSでローカル開発を行う場合、自己署名証明書が必要です。

まだローカル認証局を作成していない場合
```bash
mkcert -install
```

プロジェクトルートに `.certificate` ディレクトリを作成し、そこに `localhost-cert.pem` と `localhost-key.pem` を配置してください。

```bash
mkdir .certificate
mkcert -key-file .certificate/localhost-key.pem -cert-file .certificate/localhost-cert.pem localhost 127.0.0.1 ::1
```
`docker-compose.yml`で、この `.certificate` ディレクトリがコンテナ内の `/https` にマウントされるように設定されています。
環境変数 `SSL_CERT_PATH` と `SSL_KEY_PATH` でこれらのファイルのパスを指定します (Docker未使用時はホストOSの絶対パス、Docker使用時はコンテナ内のパス)。

### 4. コンテナ初期化
```bash
cd apps/api
make init
```

### 5. データベースのセットアップ
SQL Server インスタンスが実行されていることを確認してください。
`apps/api` ディレクトリに移動し、Entity Framework Core のマイグレーションを実行してデータベーススキーマを作成します。
```bash
make migrate
```

## 📄 APIドキュメント (Swagger)

アプリケーション実行後、以下のURLにアクセスすると Swagger UI が表示され、APIの仕様確認とテストが可能です。

**Swagger UI:** https://localhost:8443/swagger/index.html
