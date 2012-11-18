using System.IO;
using NUnit.Framework;

[TestFixture]
public class MetroLogTests:BaseTests
{

    public MetroLogTests()
		: base(Path.GetFullPath(@"..\..\..\AssemblyToProcess\bin\DebugMetroLog\MetroLogAssemblyToProcess.dll"))
    {
    }

}