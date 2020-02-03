using System.Linq;
using Fody;
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
        var typeType = FindTypeDefinition("System.Type");

        var funcDefinition = FindTypeDefinition("System.Func`1");

        var genericInstanceType = new GenericInstanceType(funcDefinition);
        genericInstanceType.GenericArguments.Add(TypeSystem.StringReference);
        GenericFunc = ModuleDefinition.ImportReference(genericInstanceType);

        var returnType = funcDefinition.FindMethod("Invoke").ReturnType;
        var methodReference = new MethodReference("Invoke", returnType, genericInstanceType)
        {
            HasThis = true
        };
        FuncInvokeMethod = ModuleDefinition.ImportReference(methodReference);

        GetTypeFromHandle = typeType.Methods
            .First(x => x.Name == "GetTypeFromHandle" &&
                        x.Parameters.Count == 1 &&
                        x.Parameters[0].ParameterType.Name == "RuntimeTypeHandle");
        GetTypeFromHandle = ModuleDefinition.ImportReference(GetTypeFromHandle);

        var stringType = FindTypeDefinition("System.String");

        ConcatMethod = ModuleDefinition.ImportReference(stringType.FindMethod("Concat", "String", "String"));
        FormatMethod = ModuleDefinition.ImportReference(stringType.FindMethod("Format", "String", "Object[]"));
        ObjectArray = new ArrayType(TypeSystem.ObjectReference);

        var exceptionType = FindTypeDefinition("System.Exception");
        ExceptionType = ModuleDefinition.ImportReference(exceptionType);
    }

    TypeDefinition LoadTypeDefinition(string typeFullName, params AssemblyDefinition[] candidateAssemblies)
    {
        foreach (var candidateAssembly in candidateAssemblies)
        {
            if (candidateAssembly == null)
            {
                continue;
            }
            foreach (var assemblyModule in candidateAssembly.Modules)
            {
                var typeDef = assemblyModule.Types.FirstOrDefault(x => x.FullName == typeFullName);
                if (typeDef != null)
                {
                    WriteInfo($"[SystemTypeResolver] Loaded type {typeDef.FullName} from {typeDef.Module.FileName}");
                    return typeDef;
                }
                var exportedType = assemblyModule.ExportedTypes.FirstOrDefault(x => x.FullName == typeFullName);
                var exportedTypeDef = exportedType?.Resolve();
                if (exportedTypeDef != null)
                {
                    WriteInfo($"[SystemTypeResolver] Loaded type (type-forwarded) {exportedTypeDef.FullName} from {exportedTypeDef.Module.FileName}");
                    return exportedTypeDef;
                }
            }
        }
        throw new WeavingException($"[SystemTypeResolver] Unable to find {typeFullName} among [{string.Join(", ", candidateAssemblies.OfType<AssemblyDefinition>())}]");
    }
}