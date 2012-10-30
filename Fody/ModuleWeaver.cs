using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Rocks;
using Mono.Cecil.Cil;


public class ModuleWeaver
{
    IInjector injector;
    public Action<string> LogInfo { get; set; }
    public IAssemblyResolver AssemblyResolver { get; set; }
    public Action<string> LogWarning { get; set; }
    public ModuleDefinition ModuleDefinition { get; set; }

    public ModuleWeaver()
    {
        LogInfo = s => { };
        LogWarning = s => { };
    }

    public void Execute()
    {
        var nLogFactory = new NLogInjector
            {
                AssemblyResolver = AssemblyResolver
            };
        if (nLogFactory.IsCompat(ModuleDefinition))
        {
            injector = nLogFactory;
        }

        if (injector == null)
        {
            //TODO:Log
            return;
        }

        var msCoreReferenceFinder = new MsCoreReferenceFinder(this, ModuleDefinition.AssemblyResolver);
        msCoreReferenceFinder.Execute();
        foreach (var type in ModuleDefinition
            .GetTypes()
            .Where(x => x.BaseType != null))
        {
            ProcessType(type);
        }
    }


    void ProcessType(TypeDefinition type)
    {
        var fieldDefinition = new FieldDefinition("AnotarLogger", FieldAttributes.Static | FieldAttributes.Private, injector.LoggerType);

        var foundUsage = false;
        foreach (var method in type.Methods)
        {
            ProcessMethod(method, fieldDefinition, ref foundUsage);
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
            injector.AddField(type,staticConstructor,fieldDefinition);
            type.Fields.Add(fieldDefinition);
        }
    }

    void ProcessMethod(MethodDefinition method, FieldDefinition fieldDefinition, ref bool foundUsage)
    {
        var ilProcessor = method.Body.GetILProcessor();

        var instructions = method.Body.Instructions.Where(x => x.OpCode == OpCodes.Call).ToList();

        for (var index = 0; index < instructions.Count; index++)
        {
            var instruction = instructions[index];
            var methodReference = instruction.Operand as MethodReference;
            if (methodReference == null)
            {
                continue;
            }
            if (methodReference.DeclaringType.FullName != "Anotar.Log")
            {
                continue;
            }
            if (!foundUsage)
            {
                method.Body.InitLocals = true;
                method.Body.SimplifyMacros();
            }
            foundUsage = true;

            var variables = new List<VariableDefinition>();
            foreach (var parameter in methodReference.Parameters)
            {
                var variable = new VariableDefinition(parameter.ParameterType);
                method.Body.Variables.Add(variable);
                variables .Add(variable);
                ilProcessor.InsertBefore(instruction, Instruction.Create(OpCodes.Stloc, variable));
            }

            ilProcessor.InsertBefore(instruction, Instruction.Create(OpCodes.Ldsfld, fieldDefinition));

            foreach (var variable in variables)
            {
                ilProcessor.InsertBefore(instruction, Instruction.Create(OpCodes.Ldloc, variable));
            }
            if (methodReference.Name == "Debug")
            {
                var parameters = methodReference.Parameters;
                if (parameters.Count == 0)
                {
                    instruction.Operand = injector.DebugMethod;
                    continue;
                }
                if (methodReference.IsMatch("String"))
                {
                    instruction.Operand = injector.DebugStringMethod;
                    continue;
                }
                if (methodReference.IsMatch("String", "object[]"))
                {
                    instruction.Operand = injector.DebugParamsMethod;
                    continue;
                }
                if (methodReference.IsMatch("String", "Exception"))
                {
                    instruction.Operand = injector.DebugStringExceptionMethod;
                    continue;
                }
            }
            if (methodReference.Name == "Info")
            {
                var parameters = methodReference.Parameters;
                if (parameters.Count == 0)
                {
                    instruction.Operand = injector.InfoMethod;
                    continue;
                }
                if (methodReference.IsMatch("String"))
                {
                    instruction.Operand = injector.InfoStringMethod;
                    continue;
                }
                if (methodReference.IsMatch("String", "object[]"))
                {
                    instruction.Operand = injector.InfoParamsMethod;
                    continue;
                }
                if (methodReference.IsMatch("String", "Exception"))
                {
                    instruction.Operand = injector.InfoStringExceptionMethod;
                    continue;
                }
            }
            if (methodReference.Name == "Warn")
            {
                var parameters = methodReference.Parameters;
                if (parameters.Count == 0)
                {
                    instruction.Operand = injector.WarnMethod;
                    continue;
                }
                if (methodReference.IsMatch("String"))
                {
                    instruction.Operand = injector.WarnStringMethod;
                    continue;
                }
                if (methodReference.IsMatch("String", "object[]"))
                {
                    instruction.Operand = injector.WarnParamsMethod;
                    continue;
                }
                if (methodReference.IsMatch("String", "Exception"))
                {
                    instruction.Operand = injector.WarnStringExceptionMethod;
                    continue;
                }
            }
            if (methodReference.Name == "Error")
            {
                var parameters = methodReference.Parameters;
                if (parameters.Count == 0)
                {
                    instruction.Operand = injector.ErrorMethod;
                    continue;
                }
                if (methodReference.IsMatch("String"))
                {
                    instruction.Operand = injector.ErrorStringMethod;
                    continue;
                }
                if (methodReference.IsMatch("String", "object[]"))
                {
                    instruction.Operand = injector.ErrorParamsMethod;
                    continue;
                }
                if (methodReference.IsMatch("String", "Exception"))
                {
                    instruction.Operand = injector.ErrorStringExceptionMethod;
                    continue;
                }
            }
        }
        if (foundUsage)
        {
            method.Body.OptimizeMacros();
        }
    }
}