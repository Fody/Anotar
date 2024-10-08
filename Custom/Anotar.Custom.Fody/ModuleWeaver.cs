﻿using Fody;

public partial class ModuleWeaver: BaseModuleWeaver
{
    public bool LogMinimalMessage;

    public override void Execute()
    {
        FindGetLoggerMethod();

        var assemblyContainsAttribute = ModuleDefinition.Assembly.CustomAttributes.ContainsAttribute("Anotar.Custom.LogMinimalMessageAttribute");
        var moduleContainsAttribute = ModuleDefinition.CustomAttributes.ContainsAttribute("Anotar.Custom.LogMinimalMessageAttribute");
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
    }

    public override bool ShouldCleanReference => true;
}