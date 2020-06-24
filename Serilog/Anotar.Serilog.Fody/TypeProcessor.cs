using System;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;

public partial class ModuleWeaver
{
    void ProcessType(TypeDefinition type)
    {
        var fieldDefinition = type.Fields.FirstOrDefault(x => x.IsStatic && x.FieldType.FullName == LazyDefinition.FullName);
        Action foundAction;
        if (fieldDefinition == null)
        {
            fieldDefinition = new FieldDefinition("LazyAnotarLogger", FieldAttributes.Static | FieldAttributes.Private, LazyDefinition)
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

    void InjectField(TypeDefinition type, FieldDefinition lazyFieldDefinition)
    {
        var staticConstructor = this.GetStaticConstructor(type);
        staticConstructor.Body.SimplifyMacros();
        var genericInstanceMethod = new GenericInstanceMethod(forContextDefinition);
        genericInstanceMethod.GenericArguments.Add(type.GetNonCompilerGeneratedType().GetGeneric());
        var instructions = staticConstructor.Body.Instructions;
        type.Fields.Add(lazyFieldDefinition);

        instructions.Insert(0, Instruction.Create(OpCodes.Ldnull));
        instructions.Insert(1, Instruction.Create(OpCodes.Ldftn, genericInstanceMethod));
        instructions.Insert(2, Instruction.Create(OpCodes.Newobj, FuncCtor));
        instructions.Insert(3, Instruction.Create(OpCodes.Ldc_I4, PublicationOnly));
        instructions.Insert(4, Instruction.Create(OpCodes.Newobj, LazyCtor));
        instructions.Insert(5, Instruction.Create(OpCodes.Stsfld, lazyFieldDefinition.GetGeneric()));

        staticConstructor.Body.OptimizeMacros();
    }
}