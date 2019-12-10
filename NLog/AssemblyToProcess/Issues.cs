using System.IO;
using Anotar.NLog;
// ReSharper disable NotAccessedVariable
// ReSharper disable RedundantAssignment

public class Issues
{
    public void Issue31()
    {
        var text = "";
        LogTo.Debug("test");
        using var reader = new StreamReader(new MemoryStream());
        LogTo.Debug("test");
        text = reader.ReadToEnd();
    }
}