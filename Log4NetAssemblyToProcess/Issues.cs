using System;
using System.IO;
using Anotar.Log4Net;

public class Issues
{
    public void Issue31()
    {
        var text = "";
        try
        {
            LogTo.Debug("test");
            using (var reader = new StreamReader(new MemoryStream()))
            {
                LogTo.Debug("test");
                text = reader.ReadToEnd();
            }
        }
        catch (Exception exception1)
        {
        }
    }
}