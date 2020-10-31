using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace VrsekDev.Blazor.BlazorCommunicationFoundation.Server.Security
{
    public interface IAuthorizationContextProvider
    {
        Task<AuthorizationContext> GetAuthorizationContextAsync(MethodInfo methodInfo);
    }
}
