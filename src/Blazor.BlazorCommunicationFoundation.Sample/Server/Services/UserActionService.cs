using Blazor.BlazorCommunicationFoundation.Sample.Shared;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blazor.BlazorCommunicationFoundation.Sample.Server.Services
{
    
    public class UserActionService : IUserActionContract
    {
        [Authorize]
        public Task PerformActionAsync()
        {
            return Task.CompletedTask;
        }
    }
}
