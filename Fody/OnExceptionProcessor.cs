using System;
using System.Collections.Generic;
using System.Linq;
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
        var startNop = Instruction.Create(OpCodes.Nop);
        body.Instructions.Add(startNop);

        
        
        body.Instructions.Add(Instruction.Create(OpCodes.Rethrow));
        return startNop;
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