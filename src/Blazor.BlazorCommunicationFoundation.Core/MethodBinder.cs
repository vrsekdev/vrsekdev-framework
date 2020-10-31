using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using VrsekDev.Blazor.BlazorCommunicationFoundation.Core;

namespace VrsekDev.Blazor.BlazorCommunicationFoundation.Core
{
    public class MethodBinder : IMethodBinder
    {
        public MethodInfo BindMethod(RequestBindingInfo requestBindingInfo, ArgumentBindingInfo[] argumentsBindingInfo)
        {
            Type contractType = Type.GetType(requestBindingInfo.TypeName);
            Type[] argumentTypes = argumentsBindingInfo.Select(x => Type.GetType(x.TypeName)).ToArray();
            if (argumentTypes.Length == 0)
            {
                argumentTypes = Type.EmptyTypes;
            }

            return contractType.GetMethod(requestBindingInfo.MethodName, argumentTypes);
        }

        public MethodInfo BindMethod(Type contractType, string methodName, object[] args)
        {
            IEnumerable<MethodInfo> methods = contractType.GetMethods();

            methods = methods.Where(x => x.Name == methodName && x.GetParameters().Length == args.Length);
            if (methods.Count() == 1)
            {
                return methods.Single();
            }

            if (args.Any(x => x == null))
            {
                // TODO: 
                throw new AmbiguousMatchException("BCF does not support null values when there is more than one method with the same name and parameter count, yet.");
            }

            return contractType.GetMethod(methodName, args.Select(x => x.GetType()).ToArray());
        }
    }
}
