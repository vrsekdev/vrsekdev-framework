using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace VrsekDev.Blazor.BlazorCommunicationFoundation.Server.Security
{
    public class AuthorizationContext
    {
        public AuthorizationPolicy Policy { get; set; }
    }
}
