using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;

class OnExceptionProcessor
{
    public MethodDefinition Method;
    public TypeSystem TypeSystem;
    public Action<bool> FoundUsageInType;
    bool foundDebug;
    bool foundInfo;
    bool foundWarn;
    bool foundError;
    public FieldDefinition FieldDefinition;
    public TypeReference ExceptionReference;
    public IInjector Injector;
    MethodBody body;
    public ModuleDefinition ModuleDefinition;

    public MethodReference FormatMethod;

    public void Process()
    {
        var customAttributes = Method.CustomAttributes;
        var found = false;
        if (customAttributes.ContainsAttribute("LogToDebugOnExceptionAttribute"))
        {
            foundDebug = true;
            found = true;
        }
        if (customAttributes.ContainsAttribute("LogToInfoOnExceptionAttribute"))
        {
            foundInfo = true;
            found = true;
        }
        if (customAttributes.ContainsAttribute("LogToWarnOnExceptionAttribute"))
        {
            foundWarn = true;
            found = true;
        }
        if (customAttributes.ContainsAttribute("LogToErrorOnExceptionAttribute"))
        {
            foundError = true;
            found = true;
        }

        if (!found)
        {
            return;
        }

        FoundUsageInType(true);
        ContinueProcessing();
    }

    void ContinueProcessing()
    {
        body = Method.Body;
        body.SimplifyMacros();


        var ret = Instruction.Create(OpCodes.Ret);
        var leave = Instruction.Create(OpCodes.Leave, ret);

        var write = AddCatch();
        body.Instructions.Add(leave);
        body.Instructions.Add(ret);

        var handler = new ExceptionHandler(ExceptionHandlerType.Catch)
            {
                TryStart = FirstInstructionSkipCtor(),
                TryEnd = write,
                HandlerStart = write,
                HandlerEnd = ret,
                CatchType = ExceptionReference,
            };

        body.ExceptionHandlers.Add(handler);

        body.InitLocals = true;
        body.OptimizeMacros();
    }

    Instruction AddCatch()
    {
        var exceptionVariable = new VariableDefinition(ExceptionReference);
        body.Variables.Add(exceptionVariable);
        var messageVariable = new VariableDefinition(TypeSystem.String);
        body.Variables.Add(messageVariable);
        var paramsArray = new VariableDefinition(new ArrayType(TypeSystem.Object));
        body.Variables.Add(paramsArray);
        var startNop = Instruction.Create(OpCodes.Nop);
        body.Instructions.Add(startNop);

        body.Instructions.Add(Instruction.Create(OpCodes.Stloc, exceptionVariable));

        var stringBuilder = new StringBuilder("Exception occurred in SimpleClass.Method1." + Method.FullName);
        var paramsToLog = GetParamsToLog().ToList();
        for (var index = 0; index < paramsToLog.Count; index++)
        {
            stringBuilder.AppendFormat("param{0} '{{index}}',", index);
        }
        body.Instructions.Add(Instruction.Create(OpCodes.Ldstr, stringBuilder.ToString()));
        body.Instructions.Add(Instruction.Create(OpCodes.Ldc_I4, Method.Parameters.Count));
        body.Instructions.Add(Instruction.Create(OpCodes.Newarr, TypeSystem.Object));
        body.Instructions.Add(Instruction.Create(OpCodes.Stloc, paramsArray));

        for (var index = 0; index < paramsToLog.Count; index++)
        {
            var parameterDefinition = Method.Parameters[index];

            body.Instructions.Add(Instruction.Create(OpCodes.Ldloc, paramsArray));
            body.Instructions.Add(Instruction.Create(OpCodes.Ldc_I4, index));
            body.Instructions.Add(Instruction.Create(OpCodes.Ldarg, parameterDefinition));
            
            if (parameterDefinition.ParameterType.IsValueType)
            {
                body.Instructions.Add(Instruction.Create(OpCodes.Box, parameterDefinition.ParameterType));
            }
            body.Instructions.Add(Instruction.Create(OpCodes.Stelem_Ref));
        }
        
            body.Instructions.Add(Instruction.Create(OpCodes.Ldloc,paramsArray));
        body.Instructions.Add(Instruction.Create(OpCodes.Call, FormatMethod));
        body.Instructions.Add(Instruction.Create(OpCodes.Stloc, messageVariable));

        Instruction sectionNop = Instruction.Create(OpCodes.Nop);
        body.Instructions.Add(Instruction.Create(OpCodes.Ldsfld, FieldDefinition));
        body.Instructions.Add(Instruction.Create(OpCodes.Callvirt, Injector.IsDebugEnabledMethod));
        body.Instructions.Add(Instruction.Create(OpCodes.Brfalse_S, sectionNop));
        body.Instructions.Add(Instruction.Create(OpCodes.Ldsfld, FieldDefinition));
        body.Instructions.Add(Instruction.Create(OpCodes.Ldloc, messageVariable));
        body.Instructions.Add(Instruction.Create(OpCodes.Ldloc, exceptionVariable));
        body.Instructions.Add(Instruction.Create(OpCodes.Callvirt, Injector.DebugExceptionMethod));
        body.Instructions.Add(sectionNop);

        body.Instructions.Add(Instruction.Create(OpCodes.Rethrow));
        return startNop;
    }

    IEnumerable<ParameterDefinition> GetParamsToLog()
    {
        foreach (var parameterDefinition in Method.Parameters)
        {
            var paramMetaData = parameterDefinition.ParameterType.MetadataType;
            if (paramMetaData == MetadataType.UIntPtr ||
                paramMetaData == MetadataType.FunctionPointer ||
                paramMetaData == MetadataType.IntPtr ||
                paramMetaData == MetadataType.Pointer)
            {
                // We don't want to log values of these parameters, so skip
                // this iteration
                continue;
            }
            yield return  parameterDefinition;
        }
    }

    Instruction FirstInstructionSkipCtor()
    {
        if (Method.IsConstructor && !Method.IsStatic)
        {
            return body.Instructions.Skip(2).First();
        }
        return body.Instructions.First();
    }

    
}