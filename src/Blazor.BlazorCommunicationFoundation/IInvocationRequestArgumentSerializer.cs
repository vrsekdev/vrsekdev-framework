namespace VrsekDev.Blazor.BlazorCommunicationFoundation.Core
{
    public interface IInvocationRequestArgumentSerializer
    {
        InvocationRequestArgument[] SerializeArguments(object[] arguments);

        object[] DeserializeArguments(InvocationRequestArgument[] arguments);
    }
}