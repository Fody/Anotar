using System;
using System.Collections.Generic;
using System.Linq;

public partial class ModuleWeaver
{

    IInjector GetInjector()
    {
        var injectors = new List<IInjector>
            {
                new NLogInjector(),
                new Log4NetInjector(),
                new SerilogInjector(),
                new MetroLogInjector()
            };

        foreach (var injector1 in injectors)
        {
            var exsitingReference = ModuleDefinition.AssemblyReferences.FirstOrDefault(x => x.Name == injector1.ReferenceName);

            if (exsitingReference != null)
            {
                var reference = AssemblyResolver.Resolve(exsitingReference);
                injector1.Init(reference, ModuleDefinition);
                return injector1;
            }
        }

        foreach (var injector1 in injectors)
        {
            var reference = AssemblyResolver.Resolve(injector1.ReferenceName);
            if (reference != null)
            {
                injector1.Init(reference, ModuleDefinition);
                return injector1;
            }
        }
        throw new Exception("Could not resolve a logging framework");
    }

}