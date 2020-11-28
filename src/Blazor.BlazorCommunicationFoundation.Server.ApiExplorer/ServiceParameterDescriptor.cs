using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Web.Http.Controllers;
using System.Web.Http.Description;

namespace VrsekDev.Blazor.BlazorCommunicationFoundation.Server.ApiExplorer
{
    public class ServiceParameterDescriptor : HttpParameterDescriptor
    {
        private readonly ParameterInfo parameterInfo;

        public ServiceParameterDescriptor(ParameterInfo parameterInfo)
        {
            this.parameterInfo = parameterInfo;
        }

        public override string ParameterName => parameterInfo.Name;

        public override Type ParameterType => parameterInfo.ParameterType;
    }
}
