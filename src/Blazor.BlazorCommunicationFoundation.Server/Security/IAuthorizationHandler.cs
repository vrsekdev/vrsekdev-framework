using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace VrsekDev.Blazor.BlazorCommunicationFoundation.Server.Security
{
    public interface IAuthorizationHandler
    {
        Task<bool> AuthorizeAsync(HttpContext httpContext, AuthorizationContext authorizationContext);
    }
}
