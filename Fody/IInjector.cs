using Mono.Cecil;

public interface IInjector
{
    TypeReference LoggerType { get; }
    MethodReference IsTraceEnabledMethod { get; }
    MethodReference TraceMethod { get; }
    MethodReference TraceExceptionMethod { get; }
    MethodReference IsDebugEnabledMethod { get; }
    MethodReference DebugMethod { get; }
    MethodReference DebugExceptionMethod { get; }
    MethodReference InfoMethod { get; }
    MethodReference IsInfoEnabledMethod { get; }
    MethodReference InfoExceptionMethod { get; }
    MethodReference WarnMethod { get; }
    MethodReference IsWarnEnabledMethod { get; }
    MethodReference WarnExceptionMethod { get; }
    MethodReference ErrorMethod { get; }
    MethodReference IsErrorEnabledMethod { get; }
    MethodReference ErrorExceptionMethod { get; }
    void AddField(TypeDefinition type, MethodDefinition constructor, FieldReference fieldDefinition);
    string ReferenceName { get; }
    void Init(AssemblyDefinition reference, ModuleDefinition moduleDefinition);
}