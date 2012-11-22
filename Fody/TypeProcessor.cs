using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;

public partial class ModuleWeaver
{
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