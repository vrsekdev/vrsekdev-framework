using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using VrsekDev.Blazor.BlazorCommunicationFoundation.Abstractions;
using VrsekDev.Blazor.BlazorCommunicationFoundation.Abstractions.Binding;
using VrsekDev.Blazor.BlazorCommunicationFoundation.Server.ApiExplorer.Types;

namespace VrsekDev.Blazor.BlazorCommunicationFoundation.Server.ApiExplorer
{
    public class ServiceApiDescriptionProvider : IApiDescriptionProvider
    {
        private readonly IEnumerable<IInvocationSerializer> invocationSerializers;
        private readonly IContractBinder contractBinder;

        public ServiceApiDescriptionProvider(
            IEnumerable<IInvocationSerializer> invocationSerializers,
            IContractBinder contractBinder)
        {
            this.invocationSerializers = invocationSerializers;
            this.contractBinder = contractBinder;
        }

        public int Order => 10;

        public void OnProvidersExecuting(ApiDescriptionProviderContext context)
        {
            foreach (var binding in contractBinder.GetBindings())
            {
                Type contractType = binding.Value.ContractType;
                MethodInfo methodInfo = binding.Value.ContractMethodInfo;

                ApiDescription apiDescription = new ApiDescription
                {
                    HttpMethod = "POST",
                    RelativePath = binding.Key,
                };
                PutParameterDescriptions(methodInfo, apiDescription.ParameterDescriptions);
                PutSupportedRequestFormats(apiDescription.SupportedRequestFormats);
                PutSupportedResponseType(methodInfo, apiDescription.SupportedResponseTypes);

                apiDescription.ActionDescriptor = GetActionDescriptor(contractType, methodInfo, apiDescription.ParameterDescriptions.Select(x => x.ParameterDescriptor).ToList());

                context.Results.Add(apiDescription);
            }
        }

        public void OnProvidersExecuted(ApiDescriptionProviderContext context)
        {

        }

        private ActionDescriptor GetActionDescriptor(Type contractType, MethodInfo methodInfo, IList<ParameterDescriptor> parameterDescriptors)
        {
            ActionDescriptor actionDescriptor = new ActionDescriptor
            {
                DisplayName = methodInfo.Name,
                Parameters = parameterDescriptors
            };
            actionDescriptor.RouteValues.Add("controller", contractType.Name);
            actionDescriptor.RouteValues.Add("action", methodInfo.Name);

            return actionDescriptor;
        }

        private void PutParameterDescriptions(MethodInfo methodInfo, IList<ApiParameterDescription> apiParameters)
        {
            if (!methodInfo.GetParameters().Any())
            {
                return;
            }

            Type argumentType = new FakeType(methodInfo);

            apiParameters.Add(new ApiParameterDescription
            {
                Name = "Arguments",
                Type = argumentType,
                ModelMetadata = new RequestTypeMetadata(argumentType, methodInfo.GetParameters().Select(x => new ParameterMetadata(x))),
                Source = BindingSource.Body
            });
        }

        private void PutSupportedRequestFormats(IList<ApiRequestFormat> supportedRequestFormats)
        {
            foreach (IInvocationSerializer serializer in invocationSerializers.Reverse())
            {
                supportedRequestFormats.Add(new ApiRequestFormat
                {
                    MediaType = serializer.MediaType
                });
            }
        }

        private void PutSupportedResponseType(MethodInfo methodInfo, IList<ApiResponseType> supportedResponseTypes)
        {
            if (methodInfo.ReturnType == typeof(Task))
            {
                supportedResponseTypes.Add(new ApiResponseType
                {
                    StatusCode = 204,
                });
                return;
            }

            Type returnType = methodInfo.ReturnType.GetGenericArguments()[0];
            ApiResponseType apiResponseType = new ApiResponseType
            {
                Type = returnType,
                StatusCode = 200,
                ModelMetadata = new ResponseTypeMetadata(returnType)
            };
            foreach (IInvocationSerializer serializer in invocationSerializers.Reverse())
            {
                apiResponseType.ApiResponseFormats.Add(new ApiResponseFormat
                {
                    MediaType = serializer.MediaType
                });
            }

            supportedResponseTypes.Add(apiResponseType);
        }
    }
}
