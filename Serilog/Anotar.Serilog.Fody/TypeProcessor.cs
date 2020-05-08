using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;
using Mono.Collections.Generic;

public partial class ModuleWeaver
{
    void ProcessType(TypeDefinition type)
    {
        var fieldDefinition = type.Fields.FirstOrDefault(x => x.IsStatic && x.FieldType.FullName == loggerType.FullName);
        Action foundAction;
        if (fieldDefinition == null)
        {
            fieldDefinition = new FieldDefinition("AnotarLogger", FieldAttributes.Static | FieldAttributes.Private, loggerType)
            {
                DeclaringType = type
            };
            var lazyFieldDefinition = new FieldDefinition("LazyAnotarLogger", FieldAttributes.Static | FieldAttributes.Private, LazyDefinition)
            {
                DeclaringType = type
            };
            foundAction = () => InjectField(type, fieldDefinition,lazyFieldDefinition);
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

    void InjectField(TypeDefinition type, FieldDefinition fieldDefinition, FieldDefinition lazyFieldDefinition)
    {
        var staticConstructor = this.GetStaticConstructor(type);
        staticConstructor.Body.SimplifyMacros();
        var genericInstanceMethod = new GenericInstanceMethod(forContextDefinition);
        genericInstanceMethod.GenericArguments.Add(type.GetNonCompilerGeneratedType().GetGeneric());
        var instructions = staticConstructor.Body.Instructions;
        type.Fields.Add(fieldDefinition);
        type.Fields.Add(lazyFieldDefinition);

        var returns = instructions.Where(_ => _.OpCode == OpCodes.Ret)
            .ToList();

        var loggerSets = GetLoggerSets(instructions);

        var ilProcessor = staticConstructor.Body.GetILProcessor();

        instructions.Insert(0, Instruction.Create(OpCodes.Ldnull));
        instructions.Insert(1, Instruction.Create(OpCodes.Ldftn, genericInstanceMethod));
        instructions.Insert(2, Instruction.Create(OpCodes.Newobj, FuncCtor));
        instructions.Insert(3, Instruction.Create(OpCodes.Ldc_I4, PublicationOnly));
        instructions.Insert(4, Instruction.Create(OpCodes.Newobj, LazyCtor));
        instructions.Insert(5, Instruction.Create(OpCodes.Stsfld, lazyFieldDefinition));
        //IL_0000: ldnull
        //IL_0001: ldftn class [Serilog]Serilog.ILogger [Serilog]Serilog.Log::ForContext<class ClassWithLogging>()
        //IL_0007: newobj instance void class [System.Runtime]System.Func`1<class [Serilog]Serilog.ILogger>::.ctor(object, native int)
        //IL_000c: ldc.i4.2
        //IL_000d: newobj instance void class [System.Runtime]System.Lazy`1<class [Serilog]Serilog.ILogger>::.ctor(class [System.Runtime]System.Func`1<!0>, valuetype [System.Runtime]System.Threading.LazyThreadSafetyMode)
        //IL_0012: stsfld class [System.Runtime]System.Lazy`1<class [Serilog]Serilog.ILogger> Template::anotarLogger
        //IL_0017: ret

        var fieldReference = fieldDefinition.GetGeneric();
        foreach (var loggerSet in loggerSets)
        {
            ilProcessor.InsertAfter(loggerSet, Instruction.Create(OpCodes.Stsfld, fieldReference));
            ilProcessor.InsertAfter(loggerSet, Instruction.Create(OpCodes.Call, genericInstanceMethod));
        }

        foreach (var returnInstruction in returns)
        {
            var newReturn = Instruction.Create(OpCodes.Ret);
            ilProcessor.InsertAfter(returnInstruction, newReturn);
            ilProcessor.InsertBefore(newReturn, Instruction.Create(OpCodes.Call, genericInstanceMethod));
            ilProcessor.InsertBefore(newReturn, Instruction.Create(OpCodes.Stsfld, fieldReference));
            returnInstruction.OpCode = OpCodes.Nop;
        }

        staticConstructor.Body.OptimizeMacros();
    }

    static IReadOnlyList<Instruction> GetLoggerSets(Collection<Instruction> instructions)
    {
        return instructions.Where(
            _ =>
            {
                if (_.OpCode != OpCodes.Call)
                {
                    return false;
                }

                if (!(_.Operand is MethodReference reference))
                {
                    return false;
                }

                return reference.Name == "set_Logger" &&
                       reference.DeclaringType?.FullName == "Serilog.Log";
            })
            .ToList();
    }
}