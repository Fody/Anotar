﻿using System.Linq;
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
            //skip for abstract and delegates
            if (!method.HasBody)
            {
                continue;
            }
            var onExceptionProcessor = new OnExceptionProcessor
                {
                    Method = method,
                    FieldDefinition = fieldDefinition,
                    TypeSystem = ModuleDefinition.TypeSystem,
                    FoundUsageInType = x => foundUsage = x,
                    Injector = injector,
                    ExceptionReference = exceptionType,ModuleDefinition = ModuleDefinition
                };
            onExceptionProcessor.Process();
            var logForwardingProcessor = new LogForwardingProcessor
                {
                    FoundUsageInType = x => foundUsage = x,
                    LogWarning = LogWarning,
                    Method = method,
                    ConcatMethod = concatMethod,
                    ExceptionType = exceptionType,
                    FieldDefinition = fieldDefinition,
                    ObjectArray = objectArray,
                    StringType = ModuleDefinition.TypeSystem.String,
                    Injector = injector,
                    FormatMethod = formatMethod
                };
            logForwardingProcessor.ProcessMethod();
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