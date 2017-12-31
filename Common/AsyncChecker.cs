using System.Linq;
using Fody;
using Mono.Cecil;

public static class AsyncChecker
{
    public static void ThrowIfIsAsync(this MethodDefinition method)
    {
        if (method.CustomAttributes.Any(_ => _.AttributeType.Name == "AsyncStateMachineAttribute"))
        {
            var message =
                $"Could not log exceptions for '{method.FullName}'. async methods are not currently supported. Feel free to submit a pull request if you want this feature.";
            throw new WeavingException(message);
        }
    }
}