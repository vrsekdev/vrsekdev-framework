using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Web.Http.Description;

namespace VrsekDev.Blazor.BlazorCommunicationFoundation.Server.ApiExplorer
{
    public class ApiExplorerDecorator<T> : IApiExplorer
        where T : IApiExplorer
    {
        private readonly ServiceApiExplorer serviceApiExplorer;
        private readonly T originalApiExplorer;

        public ApiExplorerDecorator(
            ServiceApiExplorer serviceApiExplorer,
            T originalApiExplorer)
        {
            this.serviceApiExplorer = serviceApiExplorer;
            this.originalApiExplorer = originalApiExplorer;
        }

        public Collection<ApiDescription> ApiDescriptions => new Collection<ApiDescription>(originalApiExplorer.ApiDescriptions.Union(serviceApiExplorer.ApiDescriptions).ToList());
    }
}
