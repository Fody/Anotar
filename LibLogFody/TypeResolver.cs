using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;

public partial class ModuleWeaver
{
    public void Init()
    {

        foreach (var module in GetAllModules())
        {
            var logManagerType = module.Types.FirstOrDefault(x => x.Name == "LogProvider");
            if (logManagerType == null)
            {
                continue;
            }

            var logExtensionsTypeDefinition = module.Types.FirstOrDefault(x => x.Name == "LogExtensions");
            if (logExtensionsTypeDefinition == null)
            {
                continue;
            }

            var getLoggerDefinition = logManagerType.FindMethod("GetLogger","String");
            constructLoggerMethod = ModuleDefinition.ImportReference(getLoggerDefinition);


            TraceMethod = ModuleDefinition.ImportReference(logExtensionsTypeDefinition.FindMethod("Trace", "ILog", "String"));
            TraceFormatMethod = ModuleDefinition.ImportReference(logExtensionsTypeDefinition.FindMethod("TraceFormat", "ILog", "String", "Object[]"));
            IsTraceEnabledMethod = ModuleDefinition.ImportReference(logExtensionsTypeDefinition.FindMethod("IsTraceEnabled", "ILog"));
            TraceExceptionMethod = ModuleDefinition.ImportReference(logExtensionsTypeDefinition.FindMethod("TraceException", "ILog", "String", "Exception", "Object[]"));

            DebugMethod = ModuleDefinition.ImportReference(logExtensionsTypeDefinition.FindMethod("Debug", "ILog", "String"));
            DebugFormatMethod = ModuleDefinition.ImportReference(logExtensionsTypeDefinition.FindMethod("DebugFormat", "ILog", "String", "Object[]"));
            IsDebugEnabledMethod = ModuleDefinition.ImportReference(logExtensionsTypeDefinition.FindMethod("IsDebugEnabled", "ILog"));
            DebugExceptionMethod = ModuleDefinition.ImportReference(logExtensionsTypeDefinition.FindMethod("DebugException", "ILog", "String", "Exception", "Object[]"));

            InfoMethod = ModuleDefinition.ImportReference(logExtensionsTypeDefinition.FindMethod("Info", "ILog", "String"));
            InfoFormatMethod = ModuleDefinition.ImportReference(logExtensionsTypeDefinition.FindMethod("InfoFormat", "ILog", "String", "Object[]"));
            IsInfoEnabledMethod = ModuleDefinition.ImportReference(logExtensionsTypeDefinition.FindMethod("IsInfoEnabled", "ILog"));
            InfoExceptionMethod = ModuleDefinition.ImportReference(logExtensionsTypeDefinition.FindMethod("InfoException", "ILog", "String", "Exception", "Object[]"));

            WarnMethod = ModuleDefinition.ImportReference(logExtensionsTypeDefinition.FindMethod("Warn", "ILog", "String"));
            WarnFormatMethod = ModuleDefinition.ImportReference(logExtensionsTypeDefinition.FindMethod("WarnFormat", "ILog", "String", "Object[]"));
            IsWarnEnabledMethod = ModuleDefinition.ImportReference(logExtensionsTypeDefinition.FindMethod("IsWarnEnabled", "ILog"));
            WarnExceptionMethod = ModuleDefinition.ImportReference(logExtensionsTypeDefinition.FindMethod("WarnException", "ILog", "String", "Exception", "Object[]"));

            ErrorMethod = ModuleDefinition.ImportReference(logExtensionsTypeDefinition.FindMethod("Error", "ILog", "String"));
            ErrorFormatMethod = ModuleDefinition.ImportReference(logExtensionsTypeDefinition.FindMethod("ErrorFormat", "ILog", "String", "Object[]"));
            IsErrorEnabledMethod = ModuleDefinition.ImportReference(logExtensionsTypeDefinition.FindMethod("IsErrorEnabled", "ILog"));
            ErrorExceptionMethod = ModuleDefinition.ImportReference(logExtensionsTypeDefinition.FindMethod("ErrorException", "ILog", "String", "Exception", "Object[]"));

            FatalMethod = ModuleDefinition.ImportReference(logExtensionsTypeDefinition.FindMethod("Fatal", "ILog", "String"));
            FatalFormatMethod = ModuleDefinition.ImportReference(logExtensionsTypeDefinition.FindMethod("FatalFormat", "ILog", "String", "Object[]"));
            IsFatalEnabledMethod = ModuleDefinition.ImportReference(logExtensionsTypeDefinition.FindMethod("IsFatalEnabled", "ILog"));
            FatalExceptionMethod = ModuleDefinition.ImportReference(logExtensionsTypeDefinition.FindMethod("FatalException", "ILog", "String", "Exception", "Object[]"));

            var log = module.Types.First(x => x.Name == "ILog");
            LogType = ModuleDefinition.ImportReference(log);

            return;
        }

        throw new WeavingException("Could not find LibLog types in the current assembly or any of the referenced assemblies.");
    }

    IEnumerable<ModuleDefinition> GetAllModules()
    {
        yield return ModuleDefinition;
        foreach (var assembly in ModuleDefinition
            .AssemblyReferences
            .Select(_ => AssemblyResolver.Resolve(_)))
        {
            yield return assembly.MainModule;
        }
    }

    public MethodReference TraceMethod;
    public MethodReference TraceFormatMethod;
    public MethodReference TraceExceptionMethod;
    public MethodReference DebugMethod;
    public MethodReference DebugFormatMethod;
    public MethodReference DebugExceptionMethod;
    public MethodReference InfoMethod;
    public MethodReference InfoFormatMethod;
    public MethodReference InfoExceptionMethod;
    public MethodReference WarnMethod;
    public MethodReference WarnFormatMethod;
    public MethodReference WarnExceptionMethod;
    public MethodReference ErrorMethod;
    public MethodReference ErrorFormatMethod;
    public MethodReference ErrorExceptionMethod;
    public MethodReference FatalMethod;
    public MethodReference FatalFormatMethod;
    public MethodReference FatalExceptionMethod;

    public TypeReference LogType;

    MethodReference constructLoggerMethod;
    public MethodReference IsTraceEnabledMethod;
    public MethodReference IsDebugEnabledMethod;
    public MethodReference IsInfoEnabledMethod;
    public MethodReference IsWarnEnabledMethod;
    public MethodReference IsErrorEnabledMethod;
    public MethodReference IsFatalEnabledMethod;
}