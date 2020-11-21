using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using VrsekDev.Blazor.BlazorCommunicationFoundation.Infrastructure;

namespace Blazor.BlazorCommunicationFoundation.Sample.Shared
{
    [Contract(Area = "WithAuth")]
    public interface IUserActionContract
    {
        Task PerformActionAsync();
    }
}
