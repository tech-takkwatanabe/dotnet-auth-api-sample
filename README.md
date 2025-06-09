# dotnet-auth-api-sample

## 🚧 under construction 🚧

```bash
cd apps
dotnet new webapi -n Api -o api/
cd api
dotnet new gitignore

dotnet add package Microsoft.EntityFrameworkCore.SqlServer
dotnet add package Microsoft.EntityFrameworkCore.Design
dotnet add package Microsoft.EntityFrameworkCore.Tools

dotnet add package DotNetEnv
dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer

dotnet add package BCrypt.Net-Next
dotnet add package UUIDNext

dotnet add package StackExchange.Redis
```