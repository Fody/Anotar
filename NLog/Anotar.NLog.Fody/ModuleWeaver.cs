﻿using Fody;

public partial class ModuleWeaver : BaseModuleWeaver
{
    public bool LogMinimalMessage;

    public override void Execute()
    {
        var assemblyContainsAttribute = ModuleDefinition.Assembly.CustomAttributes.ContainsAttribute("Anotar.NLog.LogMinimalMessageAttribute");
        var moduleContainsAttribute = ModuleDefinition.CustomAttributes.ContainsAttribute("Anotar.NLog.LogMinimalMessageAttribute");
        if (assemblyContainsAttribute || moduleContainsAttribute)
        {
            LogMinimalMessage = true;
        }
        LoadSystemTypes();
        Init();

        foreach (var type in ModuleDefinition
            .GetTypes()
            .Where(_ => _.BaseType != null &&
                        !_.IsEnum &&
                        !_.IsInterface))
        {
            ProcessType(type);
        }
    }

    public override IEnumerable<string> GetAssembliesForScanning()
    {
        yield return "mscorlib";
        yield return "System.Runtime";
        yield return "System.Core";
        yield return "NLog";
    }

    public override bool ShouldCleanReference => true;
}