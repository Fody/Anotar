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
        var typeType = FindType("System.Type");

        var funcDefinition = FindType("System.Func`1");

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

        var stringType = FindType("System.String");

        ConcatMethod = ModuleDefinition.ImportReference(stringType.FindMethod("Concat", "String", "String"));
        FormatMethod = ModuleDefinition.ImportReference(stringType.FindMethod("Format", "String", "Object[]"));
        ObjectArray = new ArrayType(ModuleDefinition.TypeSystem.Object);

        var exceptionType = FindType("System.Exception");
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
                    LogInfo?.Invoke($"[SystemTypeResolver] Loaded type {typeDef.FullName} from {typeDef.Module.FileName}");
                    return typeDef;
                }
                var exportedType = assemblyModule.ExportedTypes.FirstOrDefault(x => x.FullName == typeFullName);
                var exportedTypeDef = exportedType?.Resolve();
                if (exportedTypeDef != null)
                {
                    LogInfo?.Invoke($"[SystemTypeResolver] Loaded type (type-forwarded) {exportedTypeDef.FullName} from {exportedTypeDef.Module.FileName}");
                    return exportedTypeDef;
                }
            }
        }
        throw new WeavingException($"[SystemTypeResolver] Unable to find {typeFullName} among [{string.Join(", ", candidateAssemblies.OfType<AssemblyDefinition>())}]");
    }
}