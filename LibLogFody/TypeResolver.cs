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
            constructLoggerMethod = ModuleDefinition.Import(getLoggerDefinition);


            TraceMethod = ModuleDefinition.Import(logExtensionsTypeDefinition.FindMethod("Trace", "ILog", "String"));
            TraceFormatMethod = ModuleDefinition.Import(logExtensionsTypeDefinition.FindMethod("TraceFormat", "ILog", "String", "Object[]"));
            isTraceEnabledMethod = ModuleDefinition.Import(logExtensionsTypeDefinition.FindMethod("IsTraceEnabled", "ILog"));
            TraceExceptionMethod = ModuleDefinition.Import(logExtensionsTypeDefinition.FindMethod("TraceException", "ILog", "String", "Exception", "Object[]"));

            DebugMethod = ModuleDefinition.Import(logExtensionsTypeDefinition.FindMethod("Debug", "ILog", "String"));
            DebugFormatMethod = ModuleDefinition.Import(logExtensionsTypeDefinition.FindMethod("DebugFormat", "ILog", "String", "Object[]"));
            isDebugEnabledMethod = ModuleDefinition.Import(logExtensionsTypeDefinition.FindMethod("IsDebugEnabled", "ILog"));
            DebugExceptionMethod = ModuleDefinition.Import(logExtensionsTypeDefinition.FindMethod("DebugException", "ILog", "String", "Exception", "Object[]"));

            InfoMethod = ModuleDefinition.Import(logExtensionsTypeDefinition.FindMethod("Info", "ILog", "String"));
            InfoFormatMethod = ModuleDefinition.Import(logExtensionsTypeDefinition.FindMethod("InfoFormat", "ILog", "String", "Object[]"));
            isInfoEnabledMethod = ModuleDefinition.Import(logExtensionsTypeDefinition.FindMethod("IsInfoEnabled", "ILog"));
            InfoExceptionMethod = ModuleDefinition.Import(logExtensionsTypeDefinition.FindMethod("InfoException", "ILog", "String", "Exception", "Object[]"));

            WarnMethod = ModuleDefinition.Import(logExtensionsTypeDefinition.FindMethod("Warn", "ILog", "String"));
            WarnFormatMethod = ModuleDefinition.Import(logExtensionsTypeDefinition.FindMethod("WarnFormat", "ILog", "String", "Object[]"));
            isWarnEnabledMethod = ModuleDefinition.Import(logExtensionsTypeDefinition.FindMethod("IsWarnEnabled", "ILog"));
            WarnExceptionMethod = ModuleDefinition.Import(logExtensionsTypeDefinition.FindMethod("WarnException", "ILog", "String", "Exception", "Object[]"));

            ErrorMethod = ModuleDefinition.Import(logExtensionsTypeDefinition.FindMethod("Error", "ILog", "String"));
            ErrorFormatMethod = ModuleDefinition.Import(logExtensionsTypeDefinition.FindMethod("ErrorFormat", "ILog", "String", "Object[]"));
            isErrorEnabledMethod = ModuleDefinition.Import(logExtensionsTypeDefinition.FindMethod("IsErrorEnabled", "ILog"));
            ErrorExceptionMethod = ModuleDefinition.Import(logExtensionsTypeDefinition.FindMethod("ErrorException", "ILog", "String", "Exception", "Object[]"));

            FatalMethod = ModuleDefinition.Import(logExtensionsTypeDefinition.FindMethod("Fatal", "ILog", "String"));
            FatalFormatMethod = ModuleDefinition.Import(logExtensionsTypeDefinition.FindMethod("FatalFormat", "ILog", "String", "Object[]"));
            isFatalEnabledMethod = ModuleDefinition.Import(logExtensionsTypeDefinition.FindMethod("IsFatalEnabled", "ILog"));
            FatalExceptionMethod = ModuleDefinition.Import(logExtensionsTypeDefinition.FindMethod("FatalException", "ILog", "String", "Exception", "Object[]"));

            var log = module.Types.First(x => x.Name == "ILog");
            LogType = ModuleDefinition.Import(log);

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
    public MethodReference isTraceEnabledMethod;
    public MethodReference isDebugEnabledMethod;
    public MethodReference isInfoEnabledMethod;
    public MethodReference isWarnEnabledMethod;
    public MethodReference isErrorEnabledMethod;
    public MethodReference isFatalEnabledMethod;

}