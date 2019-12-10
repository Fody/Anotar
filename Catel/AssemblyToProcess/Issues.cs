using System.IO;
using Anotar.Catel;
// ReSharper disable RedundantAssignment

public class Issues
{
    public void Issue31()
    {
        // ReSharper disable once NotAccessedVariable
        var text = "";
        LogTo.Debug("test");
        using var reader = new StreamReader(new MemoryStream());
        LogTo.Debug("test");
        text = reader.ReadToEnd();
    }
}