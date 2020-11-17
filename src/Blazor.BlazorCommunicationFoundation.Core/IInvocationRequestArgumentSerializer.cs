using System.Reflection;

namespace VrsekDev.Blazor.BlazorCommunicationFoundation
{
    public interface IInvocationRequestArgumentSerializer
    {
        InvocationRequestArgument[] SerializeArguments(ParameterInfo[] parameters, object[] arguments);

        object[] DeserializeArguments(ParameterInfo[] parameters, InvocationRequestArgument[] arguments);
    }
}