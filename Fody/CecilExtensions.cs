using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Collections.Generic;

public static class CecilExtensions
{
    public static void BeforeLast(this Collection<Instruction> collection, params Instruction[] instructions)
    {
        var index = collection.Count - 1;
        foreach (var instruction in instructions)
        {
            collection.Insert(index, instruction);
            index++;
        }
    }

    public static bool ContainsAttribute(this Collection<CustomAttribute> attributes, string attributeName)
    {
        var containsAttribute = attributes.FirstOrDefault(x => x.Constructor.DeclaringType.Name == attributeName);
        if (containsAttribute != null)
        {
            attributes.Remove(containsAttribute);
        }
        return containsAttribute != null;
    }

    public static MethodDefinition FindMethod(this TypeDefinition typeDefinition, string method, params string[] paramTypes)
    {
        return typeDefinition.Methods.First(x => x.Name == method && x.IsMatch(paramTypes));
    }
    
    public static bool IsMatch(this MethodReference methodReference, params string[] paramTypes)
    {
        if (methodReference.Parameters.Count != paramTypes.Length)
        {
            return false;
        }
        for (var index = 0; index < methodReference.Parameters.Count; index++)
        {
            var parameterDefinition = methodReference.Parameters[index];
            var paramType = paramTypes[index];
            if (parameterDefinition.ParameterType.Name != paramType)
            {
                return false;
            }
        }
        return true;
    }

    public static FieldReference GetGeneric(this FieldDefinition definition)
    {
        if (definition.DeclaringType.HasGenericParameters)
        {
            var declaringType = new GenericInstanceType(definition.DeclaringType);
            foreach (var parameter in definition.DeclaringType.GenericParameters)
            {
                declaringType.GenericArguments.Add(parameter);
            }
            return new FieldReference(definition.Name, definition.FieldType, declaringType);
        }

        return definition;
    }
}