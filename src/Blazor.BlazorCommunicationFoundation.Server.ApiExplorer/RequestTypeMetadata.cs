using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using System;
using System.Collections.Generic;
using System.Text;
using VrsekDev.Blazor.BlazorCommunicationFoundation.Abstractions;

namespace VrsekDev.Blazor.BlazorCommunicationFoundation.Server.ApiExplorer
{
    public class RequestTypeMetadata : ServiceApiModelMetadata
    {
        private readonly IEnumerable<ParameterMetadata> parameters;

        public RequestTypeMetadata(Type type, IEnumerable<ParameterMetadata> parameters) : base(ModelMetadataIdentity.ForType(type))
        {
            this.parameters = parameters;
        }

        public override ModelPropertyCollection Properties => new ModelPropertyCollection(parameters);

        public override BindingSource BindingSource => BindingSource.Body;

        public override string DisplayName => "Arguments_";
    }
}
