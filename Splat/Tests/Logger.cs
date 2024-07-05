using Splat;

public class Logger : ILogger
{
    public Logger()
    {
        Level = LogLevel.Debug;
    }
    public List<string> Errors = new();
    public List<string> Fatals = new();
    public List<string> Debugs = new();
    public List<string> Informations = new();
    public List<string> Warns = new();
    public List<(Exception exception,string message,LogLevel level)> Exceptions = new();
    public void Write(string message, LogLevel logLevel)
    {
        if (logLevel == LogLevel.Fatal)
        {
            Fatals.Add(message);
            return;
        }
        if (logLevel == LogLevel.Error)
        {
            Errors.Add(message);
            return;
        }
        if (logLevel == LogLevel.Warn)
        {
            Warns.Add(message);
            return;
        }
        if (logLevel == LogLevel.Info)
        {
            Informations.Add(message);
            return;
        }
        if (logLevel == LogLevel.Debug)
        {
            Debugs.Add(message);
        }
    }

    public void Write(Exception exception, string message, LogLevel logLevel)
    {
        Exceptions.Add((exception, message, logLevel));
    }

    public void Write(string message, Type type, LogLevel logLevel)
    {
        throw new NotImplementedException();
    }

    public void Write(Exception exception, string message, Type type, LogLevel logLevel)
    {
        throw new NotImplementedException();
    }

    public LogLevel Level { get; set; }

    public void Clear()
    {
        Exceptions.Clear();
        Debugs.Clear();
        Informations.Clear();
        Warns.Clear();
        Fatals.Clear();
        Errors.Clear();
    }
}