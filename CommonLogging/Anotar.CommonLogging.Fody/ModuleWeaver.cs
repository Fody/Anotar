using System.Collections.Generic;
using System.Linq;
using Fody;

public partial class ModuleWeaver : BaseModuleWeaver
{
    public bool LogMinimalMessage;
    public bool LogMinimalMethodName;
    public bool DoNotLogMethodName;
    public bool DoNotLogLineNumber;

    public override void Execute()
    {
        if (ModuleDefinition.Assembly.CustomAttributes.ContainsAttribute("Anotar.CommonLogging.LogMinimalMessageAttribute")
            || ModuleDefinition.CustomAttributes.ContainsAttribute("Anotar.CommonLogging.LogMinimalMessageAttribute"))
        {
            LogMinimalMessage = true;
        }
        else
        {
            if (ModuleDefinition.Assembly.CustomAttributes.ContainsAttribute("Anotar.CommonLogging.LogMinimalMethodNameAttribute")
            || ModuleDefinition.CustomAttributes.ContainsAttribute("Anotar.CommonLogging.LogMinimalMethodNameAttribute"))
            {
                LogMinimalMethodName = true;
            }

            if (ModuleDefinition.Assembly.CustomAttributes.ContainsAttribute("Anotar.CommonLogging.DoNotLogMethodNameAttribute")
            || ModuleDefinition.CustomAttributes.ContainsAttribute("Anotar.CommonLogging.DoNotLogMethodNameAttribute"))
            {
                DoNotLogMethodName = true;
            }

            if (ModuleDefinition.Assembly.CustomAttributes.ContainsAttribute("Anotar.CommonLogging.DoNotLogLineNumberAttribute")
            || ModuleDefinition.CustomAttributes.ContainsAttribute("Anotar.CommonLogging.DoNotLogLineNumberAttribute"))
            {
                DoNotLogLineNumber = true;
            }
        }

        LoadSystemTypes();
        Init();

        foreach (var type in ModuleDefinition
            .GetTypes()
            .Where(x => x.BaseType != null && !x.IsEnum && !x.IsInterface))
        {
            ProcessType(type);
        }
    }

    public override IEnumerable<string> GetAssembliesForScanning()
    {
        yield return "mscorlib";
        yield return "System.Runtime";
        yield return "System.Core";
        yield return "Common.Logging";
        yield return "Common.Logging.Portable";
        yield return "Common.Logging.Core";
    }

    public override bool ShouldCleanReference => true;
}