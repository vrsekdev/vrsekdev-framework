using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using VrsekDev.Blazor.BlazorCommunicationFoundation.Server.Binding;

namespace VrsekDev.Blazor.BlazorCommunicationFoundation.Server.ApiExplorer
{
    public class ServiceApiDescriptionProvider : IApiDescriptionProvider
    {
        private readonly IContractBinder contractBinder;

        public ServiceApiDescriptionProvider(IContractBinder contractBinder)
        {
            this.contractBinder = contractBinder;
        }

        public int Order => 10;

        public void OnProvidersExecuting(ApiDescriptionProviderContext context)
        {
            foreach (var binding in contractBinder.GetBindings())
            {
                string identifier = binding.Key;
                Type contractType = binding.Value.ContractType;
                MethodInfo methodInfo = binding.Value.ContractMethodInfo;

                context.Results.Add(new ApiDescription
                {
                    //GroupName = contractType.Name,
                    HttpMethod = "POST",
                    ActionDescriptor = GetActionDescriptor(methodInfo),
                    RelativePath = $"bcf/invoke?bindingIdentifier={identifier}"
                });
            }
        }

        public void OnProvidersExecuted(ApiDescriptionProviderContext context)
        {

        }

        private ActionDescriptor GetActionDescriptor(MethodInfo methodInfo)
        {
            List<ParameterDescriptor> parameters = methodInfo.GetParameters().Select(GetParameterDescriptor).ToList();

            ActionDescriptor actionDescriptor = new ActionDescriptor
            {
                DisplayName = methodInfo.Name,
                Parameters = parameters
            };
            actionDescriptor.RouteValues.Add("controller", "bcf");

            return actionDescriptor;
        }

        private ParameterDescriptor GetParameterDescriptor(ParameterInfo parameterInfo)
        {
            return new ParameterDescriptor
            {
                Name = parameterInfo.Name,
                ParameterType = parameterInfo.ParameterType
            };
        }
    }
}
