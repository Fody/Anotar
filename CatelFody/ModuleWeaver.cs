using System;
using System.Linq;
using Mono.Cecil;

public partial class ModuleWeaver
{
    public Action<string> LogInfo { get; set; }
    public IAssemblyResolver AssemblyResolver { get; set; }
    public Action<string> LogWarning { get; set; }
    public Action<string> LogError { get; set; }
    public ModuleDefinition ModuleDefinition { get; set; }
    public MethodReference ConcatMethod;
	public MethodReference FormatMethod;
    public bool LogMinimalMessage;
    public TypeReference ExceptionType;
    public ArrayType ObjectArray;
    MethodReference getTypeFromHandle;

    public ModuleWeaver()
    {
        LogInfo = s => { };
        LogWarning = s => { };
        LogError = s => { };
    }

    public void Execute()
    {
        var assemblyContainsAttribute = ModuleDefinition.Assembly.CustomAttributes.ContainsAttribute("Anotar.Catel.LogMinimalMessageAttribute");
        var moduleContainsAttribute = ModuleDefinition.CustomAttributes.ContainsAttribute("Anotar.Catel.LogMinimalMessageAttribute");
        if (assemblyContainsAttribute || moduleContainsAttribute)
        {
            LogMinimalMessage = true;
        }
        FindReference();
        Init();
        var stringType = ModuleDefinition.TypeSystem.String.Resolve();
        ConcatMethod = ModuleDefinition.Import(stringType.FindMethod("Concat", "String", "String"));
        FormatMethod = ModuleDefinition.Import(stringType.FindMethod("Format", "String", "Object[]"));
        ObjectArray = new ArrayType(ModuleDefinition.TypeSystem.Object);

        var mscorlib = AssemblyResolver.Resolve("mscorlib");
        var mscorlibTypes = mscorlib.MainModule.Types;
        var typeType = mscorlibTypes.First(x => x.Name == "Type");
        getTypeFromHandle = typeType.Methods
                                    .First(x => x.Name == "GetTypeFromHandle" &&
                                                x.Parameters.Count == 1 &&
                                                x.Parameters[0].ParameterType.Name == "RuntimeTypeHandle");
        getTypeFromHandle = ModuleDefinition.Import(getTypeFromHandle);


        var msCoreLibDefinition = AssemblyResolver.Resolve("mscorlib");
        ExceptionType = ModuleDefinition.Import(msCoreLibDefinition.MainModule.Types.First(x => x.Name == "Exception"));
        foreach (var type in ModuleDefinition
            .GetTypes()
            .Where(x => (x.BaseType != null) && !x.IsEnum && !x.IsInterface))
        {
            ProcessType(type);
        }

        RemoveReference();
    }

}