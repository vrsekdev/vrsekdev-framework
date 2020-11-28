using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Controllers;

namespace VrsekDev.Blazor.BlazorCommunicationFoundation.Server.ApiExplorer
{
    public class ServiceMethodDescriptor : HttpActionDescriptor
    {
        private readonly MethodInfo methodInfo;

        public ServiceMethodDescriptor(MethodInfo methodInfo)
        {
            this.methodInfo = methodInfo;
        }

        public override string ActionName => methodInfo.Name;

        public override Type ReturnType => methodInfo.ReturnType;

        public override Task<object> ExecuteAsync(HttpControllerContext controllerContext, IDictionary<string, object> arguments, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public override Collection<HttpParameterDescriptor> GetParameters()
        {
            return new Collection<HttpParameterDescriptor>(methodInfo.GetParameters().Select(x => new ServiceParameterDescriptor(x)).ToList<HttpParameterDescriptor>());
        }
    }
}
