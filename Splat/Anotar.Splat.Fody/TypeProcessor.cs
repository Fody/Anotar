using System;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;

public partial class ModuleWeaver
{
    void ProcessType(TypeDefinition type)
    {
        var fieldDefinition =
            type.Fields.FirstOrDefault(x => x.IsStatic && x.FieldType.FullName == FullLoggerType.FullName);
        Action foundAction;
        if (fieldDefinition == null)
        {
            fieldDefinition = new FieldDefinition("AnotarLogger", FieldAttributes.Static | FieldAttributes.Private,
                FullLoggerType)
            {
                DeclaringType = type,
                IsStatic = true,
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
                LoggerField = fieldReference,
                FoundUsageInType = () => foundUsage = true,
                ModuleWeaver = this
            };
            onExceptionProcessor.Process();

            var logForwardingProcessor = new LogForwardingProcessor
            {
                FoundUsageInType = () => foundUsage = true,
                Method = method,
                ModuleWeaver = this,
                LoggerField = fieldReference,
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
        var staticConstructor = this.GetStaticConstructor(type);
        var instructions = staticConstructor.Body.Instructions;

        var targetType = type.GetNonCompilerGeneratedType();
        var logManagerVariable = new VariableDefinition(LogManagerType);
        staticConstructor.Body.Variables.Add(logManagerVariable);


        instructions.Insert(0, Instruction.Create(OpCodes.Call, GetLocatorMethod));
        instructions.Insert(1, Instruction.Create(OpCodes.Ldtoken, LogManagerType));
        instructions.Insert(2, Instruction.Create(OpCodes.Call, GetTypeFromHandle));
        instructions.Insert(3, Instruction.Create(OpCodes.Ldnull));
        instructions.Insert(4, Instruction.Create(OpCodes.Callvirt, GetServiceMethod));
        instructions.Insert(5, Instruction.Create(OpCodes.Castclass, LogManagerType));
        instructions.Insert(6, Instruction.Create(OpCodes.Stloc, logManagerVariable));
        instructions.Insert(7, Instruction.Create(OpCodes.Ldloc, logManagerVariable));
        instructions.Insert(8, Instruction.Create(OpCodes.Ldtoken, targetType.GetGeneric()));
        instructions.Insert(9, Instruction.Create(OpCodes.Call, GetTypeFromHandle));
        instructions.Insert(10, Instruction.Create(OpCodes.Callvirt, GetLoggerMethod));
        instructions.Insert(11, Instruction.Create(OpCodes.Stsfld, fieldDefinition.GetGeneric()));

        type.Fields.Add(fieldDefinition);
    }
}