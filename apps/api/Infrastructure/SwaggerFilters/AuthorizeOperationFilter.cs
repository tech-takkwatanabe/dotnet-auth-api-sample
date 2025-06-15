using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Api.Infrastructure.SwaggerFilters
{
  public class AuthorizeOperationFilter : IOperationFilter
  {
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
      // アクションメソッドまたはコントローラーに [Authorize] 属性があるか確認
      var hasAuthorizeAttribute = context.MethodInfo.GetCustomAttributes(true).OfType<AuthorizeAttribute>().Any() ||
                                  context.MethodInfo.DeclaringType.GetCustomAttributes(true).OfType<AuthorizeAttribute>().Any();

      if (hasAuthorizeAttribute)
      {
        operation.Security = new List<OpenApiSecurityRequirement>
          {
            new OpenApiSecurityRequirement
            {
              {
                new OpenApiSecurityScheme { Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" } },
                new string[] { }
              }
            }
          };
      }
    }
  }
}