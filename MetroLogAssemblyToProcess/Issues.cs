using System.IO;
using Anotar.MetroLog;

public class Issues
{

    public void Issue31()
    {
        var text = "";
        LogTo.Debug("test");
        using (var reader = new StreamReader(new MemoryStream()))
        {
            LogTo.Debug("test");
            text = reader.ReadToEnd();
        }
    }



}