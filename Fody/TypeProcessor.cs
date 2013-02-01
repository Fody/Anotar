using System;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;

public partial class ModuleWeaver
{
    void ProcessType(TypeDefinition type)
    {
        var fieldDefinition = type.Fields.FirstOrDefault(x => x.IsStatic && x.FieldType.FullName == injector.LoggerType.FullName);
        Action foundAction;
        if (fieldDefinition == null)
        {
            fieldDefinition = new FieldDefinition("AnotarLogger", FieldAttributes.Static | FieldAttributes.Private, injector.LoggerType)
                {
                    DeclaringType = type
                };
            foundAction = () => InjectField(type, fieldDefinition);
        }
        else
        {
            foundAction = () => { };
        }
        var fieldReference = fieldDefinition.GetGeneric();
        var foundUsage = false;
        foreach (var method in type.Methods)
        {
            //skip for abstract and delegates
            if (!method.HasBody)
            {
                continue;
            }

            var onExceptionProcessor = new OnExceptionProcessor
                {
                    Method = method,
                    Field = fieldReference,
                    FormatMethod = formatMethod,
                    TypeSystem = ModuleDefinition.TypeSystem,
                    FoundUsageInType = x => foundUsage = x,
                    Injector = injector,
                    ExceptionReference = exceptionType,
                    ModuleWeaver = this
                };
            onExceptionProcessor.Process();

            var logForwardingProcessor = new LogForwardingProcessor
                {
                    FoundUsageInType = x => foundUsage = x,
                    Method = method,
                    ConcatMethod = concatMethod,
                    ExceptionType = exceptionType,
                    LogMinimalMessage = logMinimalMessage,
                    Field = fieldReference,
                    ObjectArray = objectArray,
                    StringType = ModuleDefinition.TypeSystem.String,
                    Injector = injector,
                    FormatMethod = formatMethod
                };
            logForwardingProcessor.ProcessMethod();

        }
        if (foundUsage)
        {
            foundAction();
        }
    }

    void InjectField(TypeDefinition type, FieldDefinition fieldDefinition)
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
        injector.AddField(type, staticConstructor, fieldDefinition.GetGeneric());
        type.Fields.Add(fieldDefinition);
    }
}