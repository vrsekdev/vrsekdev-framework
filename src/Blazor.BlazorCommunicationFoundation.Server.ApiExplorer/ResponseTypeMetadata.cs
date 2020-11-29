using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using System;
using System.Collections.Generic;
using System.Text;

namespace VrsekDev.Blazor.BlazorCommunicationFoundation.Server.ApiExplorer
{
    public class ResponseTypeMetadata : ServiceApiModelMetadata
    {
        private readonly Type responseType;

        public ResponseTypeMetadata(Type responseType) : base(ModelMetadataIdentity.ForType(responseType))
        {
            this.responseType = responseType;
        }

        public override string DisplayName => responseType.Name;

        public override BindingSource BindingSource => BindingSource.Body;
    }
}
