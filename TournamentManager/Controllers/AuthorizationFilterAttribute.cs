using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using System.Linq;

namespace TournamentManager.Controllers
{
    public class Security
    {
        public string[] ApiKeys { get; set; }
    }

    public class AuthorizationFilterAttribute : Attribute, IAuthorizationFilter
    {
        
        public IConfiguration Configuration { get; set; }

        public AuthorizationFilterAttribute(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var security = Configuration.GetSection("Security").Get<Security>();
            StringValues apiKeys = context.HttpContext.Request.Headers["Authorization"];
            bool found = false;

            if (security.ApiKeys == null)
            {
                found = true;
            }
            else
            {
                if (security.ApiKeys.Length > 0)
                {
                    if (apiKeys.Any())
                    {
                        foreach (var apikey in apiKeys)
                        {
                            if (security.ApiKeys.Contains(apikey))
                            {
                                found = true;
                                break;
                            }
                        }
                    }
                }
                else
                    found = true;
            }

            if (!found)
                context.Result = new UnauthorizedResult();
        }
    }
}
