using System;
using System.Diagnostics;
using System.Linq;
using Mono.Cecil;

public partial class ModuleWeaver
{

    public MethodReference GetTypeFromHandle;
    public TypeReference ExceptionType;
    public ArrayType ObjectArray;
    public MethodReference ConcatMethod;
    public MethodReference FormatMethod;
    public MethodReference FuncInvokeMethod;
    public TypeReference GenericFunc;

    public void LoadSystemTypes()
    {
        var mscorlib = AssemblyResolver.Resolve(new AssemblyNameReference("mscorlib", null));
        var netstandard = AssemblyResolver.Resolve(new AssemblyNameReference("netstandard", null));
        var runtime = AssemblyResolver.Resolve(new AssemblyNameReference("System.Runtime", null));
        var core = AssemblyResolver.Resolve(new AssemblyNameReference("System.Core", null));

        var typeType = LoadTypeDefinition("System.Type", mscorlib, netstandard, runtime, core);

        var funcDefinition = LoadTypeDefinition("System.Func`1", mscorlib, netstandard, runtime, core);

        var genericInstanceType = new GenericInstanceType(funcDefinition);
        genericInstanceType.GenericArguments.Add(ModuleDefinition.TypeSystem.String);
        GenericFunc = ModuleDefinition.ImportReference(genericInstanceType);

        var methodReference = new MethodReference("Invoke", funcDefinition.FindMethod("Invoke").ReturnType, genericInstanceType) { HasThis = true };
        FuncInvokeMethod = ModuleDefinition.ImportReference(methodReference);

        GetTypeFromHandle = typeType.Methods
            .First(x => x.Name == "GetTypeFromHandle" &&
                        x.Parameters.Count == 1 &&
                        x.Parameters[0].ParameterType.Name == "RuntimeTypeHandle");
        GetTypeFromHandle = ModuleDefinition.ImportReference(GetTypeFromHandle);


        var stringType = LoadTypeDefinition("System.String", mscorlib, netstandard, runtime, core);

        ConcatMethod = ModuleDefinition.ImportReference(stringType.FindMethod("Concat", "String", "String"));
        FormatMethod = ModuleDefinition.ImportReference(stringType.FindMethod("Format", "String", "Object[]"));
        ObjectArray = new ArrayType(ModuleDefinition.TypeSystem.Object);

        var exceptionType = LoadTypeDefinition("System.Exception", mscorlib, netstandard, runtime, core);
        ExceptionType = ModuleDefinition.ImportReference(exceptionType);

    }

    private static TypeDefinition LoadTypeDefinition(
        string typeFullName,
        params AssemblyDefinition[] canidateAssemblies)
    {
        foreach (var candidateAssembly in canidateAssemblies)
        {
            var typeDef = candidateAssembly?.MainModule.Types.FirstOrDefault(x => x.FullName == typeFullName);
            if (typeDef != null)
            {
                Debug.WriteLine("Loaded type {0} from {1}", typeDef.FullName, typeDef.Module.FileName);
                return typeDef;
            }
        }
        throw new WeavingException($"Unable to find {typeFullName} among [{String.Join(", ", canidateAssemblies.OfType<AssemblyDefinition>())}]");
    }

}