using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web.Http.Description;
using VrsekDev.Blazor.BlazorCommunicationFoundation.Server.Binding;

namespace VrsekDev.Blazor.BlazorCommunicationFoundation.Server.ApiExplorer
{
    public class ServiceApiExplorer : IApiExplorer
    {
        private readonly Collection<ApiDescription> apiDescriptions;

        public ServiceApiExplorer(IContractBinder contractBinder)
        {
            IReadOnlyDictionary<string, ContractMethodBinding> bindings = contractBinder.GetBindings();
            apiDescriptions = new Collection<ApiDescription>(bindings.Select(x => new ApiDescription
            {
                ActionDescriptor = new ServiceMethodDescriptor(x.Value.ContractMethodInfo),
                HttpMethod = HttpMethod.Post,
                RelativePath = $"/bcf/invoke?bindingIdentifier={x.Key}"
            }).ToList());
        }

        public Collection<ApiDescription> ApiDescriptions => apiDescriptions;
    }
}
