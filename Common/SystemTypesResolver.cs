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

        var typeType =
            mscorlib?.MainModule.Types.FirstOrDefault(x => x.Name == "Type")
            ?? runtime?.MainModule.Types.FirstOrDefault(x => x.Name == "Type")
            ?? netstandard?.MainModule.Types.FirstOrDefault(x => x.Name == "Type")
            ?? core?.MainModule.Types.FirstOrDefault(x => x.Name == "Type");

        if (typeType == null)
        {
            throw new WeavingException("Unable to find System.Type");
        }
        Debug.WriteLine("Loaded type {0} from {1}", typeType.FullName, typeType.Module.FileName);

        var funcDefinition =
            mscorlib?.MainModule.Types.FirstOrDefault(x => x.Name == "Func`1")
            ?? runtime?.MainModule.Types.FirstOrDefault(x => x.Name == "Func`1")
            ?? netstandard?.MainModule.Types.FirstOrDefault(x => x.Name == "Func`1")
            ?? core?.MainModule.Types.FirstOrDefault(x => x.Name == "Func`1");
        if (funcDefinition == null)
        {
            throw new WeavingException("Unable to find System.Func`1");
        }
        Debug.WriteLine("Loaded type {0} from {1}", funcDefinition.FullName, funcDefinition.Module.FileName);

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


        var stringType =
            mscorlib?.MainModule.Types.FirstOrDefault(x => x.Name == "String")
            ?? runtime?.MainModule.Types.FirstOrDefault(x => x.Name == "String")
            ?? netstandard?.MainModule.Types.FirstOrDefault(x => x.Name == "String")
            ?? core?.MainModule.Types.FirstOrDefault(x => x.Name == "String");

        if (stringType == null)
        {
            throw new WeavingException("Unable to find System.String");
        }
        Debug.WriteLine("Loaded type {0} from {1}", stringType.FullName, stringType.Module.FileName);

        ConcatMethod = ModuleDefinition.ImportReference(stringType.FindMethod("Concat", "String", "String"));
        FormatMethod = ModuleDefinition.ImportReference(stringType.FindMethod("Format", "String", "Object[]"));
        ObjectArray = new ArrayType(ModuleDefinition.TypeSystem.Object);

        var exceptionType =
            mscorlib?.MainModule.Types.FirstOrDefault(x => x.Name == "Exception")
            ?? runtime?.MainModule.Types.FirstOrDefault(x => x.Name == "Exception")
            ?? netstandard?.MainModule.Types.FirstOrDefault(x => x.Name == "Exception")
            ?? core?.MainModule.Types.FirstOrDefault(x => x.Name == "Exception");
        if (exceptionType == null)
        {
            throw new WeavingException("Unable to find System.Exception");
        }
        Debug.WriteLine("Loaded type {0} from {1}", exceptionType.FullName, exceptionType.Module.FileName);

        ExceptionType = ModuleDefinition.ImportReference(exceptionType);

    }

}