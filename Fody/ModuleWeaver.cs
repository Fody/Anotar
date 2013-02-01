using System;
using System.Linq;
using Mono.Cecil;

public partial class ModuleWeaver
{
    IInjector injector;
    public Action<string> LogInfo { get; set; }
    public IAssemblyResolver AssemblyResolver { get; set; }
    public Action<string> LogWarning { get; set; }
    public Action<string> LogError { get; set; }
    public ModuleDefinition ModuleDefinition { get; set; }
    MethodReference concatMethod;
    MethodReference formatMethod;
    bool logMinimalMessage;
    TypeReference exceptionType;
    ArrayType objectArray;

    public ModuleWeaver()
    {
        LogInfo = s => { };
        LogWarning = s => { };
        LogError = s => { };
    }

    public void Execute()
    {
        var assemblyContainsAttribute = ModuleDefinition.Assembly.CustomAttributes.ContainsAttribute("LogMinimalMessageAttribute");
        var moduleContainsAttribute = ModuleDefinition.CustomAttributes.ContainsAttribute("LogMinimalMessageAttribute");
        if (assemblyContainsAttribute || moduleContainsAttribute)
        {
            logMinimalMessage = true;
        }
        injector = GetInjector();
        var stringType = ModuleDefinition.TypeSystem.String.Resolve();
        concatMethod = ModuleDefinition.Import(stringType.FindMethod("Concat", "String", "String"));
        formatMethod = ModuleDefinition.Import(stringType.FindMethod("Format", "String", "Object[]"));
        objectArray = new ArrayType(ModuleDefinition.TypeSystem.Object);

        var msCoreLibDefinition = AssemblyResolver.Resolve("mscorlib");
        exceptionType = ModuleDefinition.Import(msCoreLibDefinition.MainModule.Types.First(x => x.Name == "Exception"));
        foreach (var type in ModuleDefinition
            .GetTypes()
            .Where(x => (x.BaseType != null) && !x.IsEnum && !x.IsInterface))
        {
            ProcessType(type);
        }

        //TODO: ensure attributes dont exist on interfaces
        RemoveReference();
    }

}