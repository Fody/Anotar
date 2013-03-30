using System.Collections.Generic;
using Mono.Cecil;
using Mono.Cecil.Cil;

public interface IInjector
{
    TypeReference LoggerType { get; }
	IEnumerable<Instruction> GetIsTraceEnabledInstructions();
    MethodReference TraceMethod { get; }
    MethodReference TraceExceptionMethod { get; }
	IEnumerable<Instruction> GetIsDebugEnabledInstructions();
    MethodReference DebugMethod { get; }
    MethodReference DebugExceptionMethod { get; }
    MethodReference InfoMethod { get; }
	IEnumerable<Instruction> GetIsInfoEnabledInstructions();
    MethodReference InfoExceptionMethod { get; }
    MethodReference WarnMethod { get; }
	IEnumerable<Instruction> GetIsWarnEnabledInstructions();
    MethodReference WarnExceptionMethod { get; }
    MethodReference ErrorMethod { get; }
	IEnumerable<Instruction> GetIsErrorEnabledInstructions();
    MethodReference ErrorExceptionMethod { get; }
    void AddField(TypeDefinition type, MethodDefinition constructor, FieldReference fieldDefinition);
    string ReferenceName { get; }
    void Init(AssemblyDefinition reference, ModuleDefinition moduleDefinition);
}