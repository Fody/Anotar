using System.Linq;
using Mono.Cecil;

public static class AsyncChecker
{

	public static void ThrowIfIsAsync(this MethodDefinition method)
	{
		if (method.CustomAttributes.Any(_ => _.AttributeType.Name == "AsyncStateMachineAttribute"))
		{
			var message = string.Format("Could not log exceptions for '{0}'. async methods are not currently supported. Feel free to submit a pull request if you want this feature.", method.FullName);
			throw new WeavingException(message);
		}
	}
}