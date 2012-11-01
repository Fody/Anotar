using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;


public class ModuleWeaver
{
    IInjector injector;
    public Action<string> LogInfo { get; set; }
    public IAssemblyResolver AssemblyResolver { get; set; }
    public Action<string> LogWarning { get; set; }
    public ModuleDefinition ModuleDefinition { get; set; }
    MethodReference concatMethod;
    MethodReference formatMethod;
    TypeReference exceptionType;
    ArrayType objectArray;

    public ModuleWeaver()
    {
        LogInfo = s => { };
        LogWarning = s => { };
    }

    public void Execute()
    {
        injector = GetInjector();
        var stringType = ModuleDefinition.TypeSystem.String.Resolve();
        concatMethod = ModuleDefinition.Import(stringType.FindMethod("Concat", "String", "String"));
        formatMethod = ModuleDefinition.Import(stringType.FindMethod("Format", "String", "Object[]"));
        objectArray = new ArrayType(ModuleDefinition.TypeSystem.Object);

        var msCoreLibDefinition = AssemblyResolver.Resolve("mscorlib");
        exceptionType = ModuleDefinition.Import(msCoreLibDefinition.MainModule.Types.First(x => x.Name == "Exception"));
        foreach (var type in ModuleDefinition
            .GetTypes()
            .Where(x => x.BaseType != null))
        {
            ProcessType(type);
        }
    }

    IInjector GetInjector()
    {
        var injectors = new List<IInjector>
            {
                new NLogInjector(),
                new Log4NetInjector()
            };

        foreach (var injector1 in injectors)
        {
            var exsitingReference = ModuleDefinition.AssemblyReferences.FirstOrDefault(x => string.Equals(x.Name, injector1.ReferenceName, StringComparison.OrdinalIgnoreCase));

            if (exsitingReference != null)
            {
                var reference = AssemblyResolver.Resolve(exsitingReference);
                injector1.Init(reference, ModuleDefinition);
                return injector1;
            }
        }

        foreach (var injector1 in injectors)
        {
            var reference = AssemblyResolver.Resolve(injector1.ReferenceName);
            if (reference != null)
            {
                injector1.Init(reference, ModuleDefinition);
                return injector1;
            }
        }
        throw new Exception("Could not resolve a logging framework");
    }


    void ProcessType(TypeDefinition type)
    {
        var fieldDefinition = new FieldDefinition("AnotarLogger", FieldAttributes.Static | FieldAttributes.Private, injector.LoggerType);

        var foundUsage = false;
        foreach (var method in type.Methods)
        {
            var methodProcessor = new MethodProcessor
                {
                    FoundUsageInType = x => foundUsage = x,
                    LogWarning = LogWarning,
                    method = method,
                    concatMethod = concatMethod,
                    exceptionType = exceptionType,
                    fieldDefinition = fieldDefinition,
                    objectArray = objectArray,
                    stringType = ModuleDefinition.TypeSystem.String,
                    injector = injector,
                    formatMethod = formatMethod
                };
            methodProcessor.ProcessMethod();
        }
        if (foundUsage)
        {
            var staticConstructor = type.Methods.FirstOrDefault(x => x.IsConstructor && x.IsStatic);
            if (staticConstructor == null)
            {
                const MethodAttributes attributes = MethodAttributes.Static
                                                    | MethodAttributes.SpecialName
                                                    | MethodAttributes.RTSpecialName
                                                    | MethodAttributes.HideBySig
                                                    | MethodAttributes.Private;
                staticConstructor = new MethodDefinition(".cctor", attributes, ModuleDefinition.TypeSystem.Void);

                staticConstructor.Body.Instructions.Add(Instruction.Create(OpCodes.Ret));
                type.Methods.Add(staticConstructor);
            }
            injector.AddField(type, staticConstructor, fieldDefinition);
            type.Fields.Add(fieldDefinition);
        }
    }
}