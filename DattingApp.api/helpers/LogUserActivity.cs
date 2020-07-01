using System;
using System.Security.Claims;
using System.Threading.Tasks;
using DattingApp.api.Data;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace DattingApp.api.helpers
{
    public class LogUserActivity : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context,
                                                 ActionExecutionDelegate next)
        {
            // delegué pointant sur le context d'execution à travers ActionExecutionDelegate
            var resultContext = await next();
            // recuperation userid avec le token
            var userId = int.Parse(resultContext.HttpContext.User
                        .FindFirst(ClaimTypes.NameIdentifier).Value);
            // injection de dépendances dans startup ??
            // nécessite le using Micorsoft.Extensions.DependencyInjection
            var repo = resultContext.HttpContext.RequestServices.GetService<IDattingRepository>();
            // modif de la date car action et enregistrement
            var user = await repo.GetUser(userId);
            user.LastActive = DateTime.Now;
            await repo.SaveAll();
        }
    }
}