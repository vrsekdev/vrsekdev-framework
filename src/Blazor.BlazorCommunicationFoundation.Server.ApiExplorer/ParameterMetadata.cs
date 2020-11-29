using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Text;

namespace VrsekDev.Blazor.BlazorCommunicationFoundation.Server.ApiExplorer
{
    public class ParameterMetadata : ServiceApiModelMetadata
    {
        private readonly ParameterInfo parameterInfo;

        public ParameterMetadata(ParameterInfo parameterInfo) : base(ModelMetadataIdentity.ForParameter(parameterInfo))
        {
            this.parameterInfo = parameterInfo;
        }

        public override string DisplayName => parameterInfo.Name;

        public override BindingSource BindingSource => BindingSource.Body;
    }
}
