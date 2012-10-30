using Mono.Cecil;

public interface IInjector
{
    TypeReference LoggerType { get; set; }
    MethodReference BuildLoggerMethod { get; set; }
    MethodReference BuildLoggerGenericMethod { get; set; }
    MethodReference DebugMethod { get; set; }
    MethodReference DebugStringMethod { get; set; }
    MethodReference DebugParamsMethod { get; set; }
    MethodReference DebugStringExceptionMethod { get; set; }
    MethodReference InfoMethod { get; set; }
    MethodReference InfoStringMethod { get; set; }
    MethodReference InfoParamsMethod { get; set; }
    MethodReference InfoStringExceptionMethod { get; set; }
    MethodReference WarnMethod { get; set; }
    MethodReference WarnStringMethod { get; set; }
    MethodReference WarnParamsMethod { get; set; }
    MethodReference WarnStringExceptionMethod { get; set; }
    MethodReference ErrorMethod { get; set; }
    MethodReference ErrorStringMethod { get; set; }
    MethodReference ErrorParamsMethod { get; set; }
    MethodReference ErrorStringExceptionMethod { get; set; }
    void AddField(TypeDefinition type, MethodDefinition constructor, FieldDefinition fieldDefinition);
}