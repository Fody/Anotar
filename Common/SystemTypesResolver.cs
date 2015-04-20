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
        var mscorlib = AssemblyResolver.Resolve("mscorlib");
        var typeType = mscorlib.MainModule.Types.FirstOrDefault(x => x.Name == "Type");
        if (typeType == null)
        {
            var runtime = AssemblyResolver.Resolve("System.Runtime");
            typeType = runtime.MainModule.Types.First(x => x.Name == "Type");
        }
        var funcDefinition = typeType.Module.Types.First(x => x.Name == "Func`1");
        var genericInstanceType = new GenericInstanceType(funcDefinition);
        genericInstanceType.GenericArguments.Add(ModuleDefinition.TypeSystem.String);
        GenericFunc = ModuleDefinition.Import(genericInstanceType);

        var methodReference = new MethodReference("Invoke", funcDefinition.FindMethod("Invoke").ReturnType, genericInstanceType) { HasThis = true };
        FuncInvokeMethod = ModuleDefinition.Import(methodReference);

        GetTypeFromHandle = typeType.Methods
            .First(x => x.Name == "GetTypeFromHandle" &&
                        x.Parameters.Count == 1 &&
                        x.Parameters[0].ParameterType.Name == "RuntimeTypeHandle");
        GetTypeFromHandle = ModuleDefinition.Import(GetTypeFromHandle);


        var stringType = ModuleDefinition.TypeSystem.String.Resolve();
        ConcatMethod = ModuleDefinition.Import(stringType.FindMethod("Concat", "String", "String"));
        FormatMethod = ModuleDefinition.Import(stringType.FindMethod("Format", "String", "Object[]"));
        ObjectArray = new ArrayType(ModuleDefinition.TypeSystem.Object);

        var exceptionType = mscorlib.MainModule.Types.FirstOrDefault(x => x.Name == "Exception");
        if (exceptionType == null)
        {
            var runtime = AssemblyResolver.Resolve("System.Runtime");
            exceptionType = runtime.MainModule.Types.First(x => x.Name == "Exception");
        }
        ExceptionType = ModuleDefinition.Import(exceptionType);

    }

}