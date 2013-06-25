using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Collections.Generic;

public static class CecilExtensions
{
    public static void Replace(this Collection<Instruction> collection, Instruction instruction, IEnumerable<Instruction> instructions)
    {
        var indexOf = collection.IndexOf(instruction);
        collection.RemoveAt(indexOf);
        foreach (var instruction1 in instructions)
        {
            collection.Insert(indexOf, instruction1);
            indexOf++;
        }
    }

    public static void CheckForDynamicUsagesOf(this MethodDefinition methodDefinition, string methodNameToCheckFor)
    {
        foreach (var instruction in methodDefinition.Body.Instructions)
        {
            if (instruction.OpCode == OpCodes.Ldtoken)
            {
                var memberReference = instruction.Operand as MemberReference;
                if (memberReference != null)
                {
                    if (memberReference.FullName == methodNameToCheckFor)
                    {
                        //TODO: sequence point
                        throw new WeavingException(string.Format("'typeof' usages and passing `dynamic' params is not supported. '{0}'.", methodDefinition.FullName));
                    }
                }
            }
        }
    }

    public static MethodDefinition GetStaticConstructor(this TypeDefinition type)
	{
		var staticConstructor = type.Methods.FirstOrDefault(x => x.IsConstructor && x.IsStatic);
		if (staticConstructor == null)
		{
			const MethodAttributes attributes = MethodAttributes.Static
												| MethodAttributes.SpecialName
												| MethodAttributes.RTSpecialName
												| MethodAttributes.HideBySig
												| MethodAttributes.Private;
			staticConstructor = new MethodDefinition(".cctor", attributes, type.Module.TypeSystem.Void);

			staticConstructor.Body.Instructions.Add(Instruction.Create(OpCodes.Ret));
			type.Methods.Add(staticConstructor);
		}
		return staticConstructor;
	}
	public static void InsertBefore(this ILProcessor processor, Instruction target, IEnumerable<Instruction> instructions)
	{
		foreach (var instruction in instructions)
			processor.InsertBefore(target, instruction);
	}
    public static void InsertAfter(this Collection<Instruction> collection, Instruction target, params Instruction[] instructions)
	{
        var indexOf = collection.IndexOf(target);
        indexOf++;
        for (var index = 0; index < instructions.Length; index++)
        {
            var instruction = instructions[index];
            collection.Insert(index+indexOf, instruction);
        }
	}
	public static SequencePoint GetPreviousSequencePoint(this Instruction instruction)
	{
		while (true)
		{

			if (instruction.SequencePoint != null)
			{
				return instruction.SequencePoint;
			}

			instruction = instruction.Previous;
			if (instruction == null)
			{
				return null;
			}
		}
	}

    public static bool ContainsAttribute(this Collection<CustomAttribute> attributes, string attributeName)
    {
        var containsAttribute = attributes.FirstOrDefault(x => x.AttributeType.FullName == attributeName);
        if (containsAttribute != null)
        {
            attributes.Remove(containsAttribute);
        }
        return containsAttribute != null;
    }

    public static MethodDefinition FindMethod(this TypeDefinition typeDefinition, string method, params string[] paramTypes)
    {
        var firstOrDefault = typeDefinition.Methods
            .FirstOrDefault(x => 
                !x.HasGenericParameters && 
                x.Name == method && 
                x.IsMatch(paramTypes));
        if (firstOrDefault == null)
        {
            var parameterNames = string.Join(", ", paramTypes);
            throw new WeavingException(string.Format("Expected to find method '{0}({1})' on type '{2}'.", method, parameterNames, typeDefinition.FullName));
        }
        return firstOrDefault;
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
	public static TypeReference GetGeneric(this TypeDefinition definition)
    {
		if (definition.HasGenericParameters)
        {
            var genericInstanceType = new GenericInstanceType(definition);
            foreach (var parameter in definition.GenericParameters)
            {
                genericInstanceType.GenericArguments.Add(parameter);
            }
            return  genericInstanceType;
        }

        return definition;
    }
}