using Blazor.BlazorCommunicationFoundation.Sample.Shared;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VrsekDev.Blazor.BlazorCommunicationFoundation.Server.Infrastructure;

namespace Blazor.BlazorCommunicationFoundation.Sample.Server.Services
{
    
    [ContractImplementation]
    public class UserActionService : IUserActionContract
    {
        [Authorize]
        public Task PerformActionAsync()
        {
            return Task.CompletedTask;
        }
    }
}
