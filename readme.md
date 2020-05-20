# <img src="/package_icon.png" height="30px"> Anotar.Fody

[![Chat on Gitter](https://img.shields.io/gitter/room/fody/fody.svg)](https://gitter.im/Fody/Fody)

Simplifies logging through a static class and some IL manipulation


### This is an add-in for [Fody](https://github.com/Fody/Home/)

**It is expected that all developers using Fody either [become a Patron on OpenCollective](https://opencollective.com/fody/contribute/patron-3059), or have a [Tidelift Subscription](https://tidelift.com/subscription/pkg/nuget-fody?utm_source=nuget-fody&utm_medium=referral&utm_campaign=enterprise). [See Licensing/Patron FAQ](https://github.com/Fody/Home/blob/master/pages/licensing-patron-faq.md) for more information.**


## Supported Logging Libraries

* [Catel](http://www.catelproject.com/)
* Custom (for frameworks/toolkits with custom logging)
* [CommonLogging](http://netcommon.sourceforge.net/)
* [NLog](http://nlog-project.org/)
* [NServiceBus](http://particular.net/nservicebus)
* [Serilog](http://serilog.net/)
* [Splat](https://github.com/paulcbetts/splat)


## Usage

See also [Fody usage](https://github.com/Fody/Home/blob/master/pages/usage.md).


### NuGet installation

Install the [Anotar.xxx.Fody NuGet package](https://www.nuget.org/packages?q=anotar) and update the [Fody NuGet package](https://nuget.org/packages/Fody/):

```powershell
PM> Install-Package Fody
PM> Install-Package Anotar.xxx.Fody
```

The `Install-Package Fody` is required since NuGet always defaults to the oldest, and most buggy, version of any dependency.


### Add to FodyWeavers.xml

Add `<Anotar.xxx/>` to [FodyWeavers.xml](https://github.com/Fody/Home/blob/master/pages/usage.md#add-fodyweaversxml)

```xml
<Weavers>
  <Anotar.xxx/>
</Weavers>
```


## NuGets

 * Catel package http://nuget.org/packages/Anotar.Catel.Fody [![NuGet Status](http://img.shields.io/nuget/v/Anotar.Catel.Fody.svg)](https://www.nuget.org/packages/Anotar.Catel.Fody/)
 * CommonLogging package http://nuget.org/packages/Anotar.CommonLogging.Fody [![NuGet Status](http://img.shields.io/nuget/v/Anotar.CommonLogging.Fody.svg)](https://www.nuget.org/packages/Anotar.CommonLogging.Fody/)
 * Custom package http://nuget.org/packages/Anotar.Custom.Fody [![NuGet Status](http://img.shields.io/nuget/v/Anotar.Custom.Fody.svg)](https://www.nuget.org/packages/Anotar.Custom.Fody/)
 * NLog package http://nuget.org/packages/Anotar.NLog.Fody [![NuGet Status](http://img.shields.io/nuget/v/Anotar.NLog.Fody.svg)](https://www.nuget.org/packages/Anotar.NLog.Fody/)
 * NServiceBus package http://nuget.org/packages/Anotar.NServiceBus.Fody [![NuGet Status](http://img.shields.io/nuget/v/Anotar.NServiceBus.Fody.svg)](https://www.nuget.org/packages/Anotar.NServiceBus.Fody/)
 * Serilog package http://nuget.org/packages/Anotar.Serilog.Fody [![NuGet Status](http://img.shields.io/nuget/v/Anotar.Serilog.Fody.svg)](https://www.nuget.org/packages/Anotar.Serilog.Fody/)
 * Splat package http://nuget.org/packages/Anotar.Splat.Fody [![NuGet Status](http://img.shields.io/nuget/v/Anotar.Splat.Fody.svg)](https://www.nuget.org/packages/Anotar.Splat.Fody/)


## Explicit Logging


### Your Code

```c#
public class MyClass
{
    void MyMethod()
    {
        LogTo.Debug("TheMessage");
    }
}
```


### What gets compiled


#### In Catel

```c#
public class MyClass
{
    static ILog logger = LogManager.GetLogger(typeof(MyClass));

    void MyMethod()
    {
        logger.WriteWithData("Method: 'Void MyMethod()'. Line: ~12. TheMessage", null, LogEvent.Debug);
    }
}
```


#### In CommonLogging

```c#
public class MyClass
{
    static ILog logger = LoggerManager.GetLogger("MyClass");

    void MyMethod()
    {
        logger.Debug("Method: 'Void MyMethod()'. Line: ~12. TheMessage");
    }
}
```


#### In Custom

```c#
public class MyClass
{
    static ILogger AnotarLogger = LoggerFactory.GetLogger<MyClass>();

    void MyMethod()
    {
        AnotarLogger.Debug("Method: 'Void MyMethod()'. Line: ~12. TheMessage");
    }
}
```


#### In NLog

```c#
public class MyClass
{
    static Logger logger = LogManager.GetLogger("MyClass");

    void MyMethod()
    {
        logger.Debug("Method: 'Void MyMethod()'. Line: ~12. TheMessage");
    }
}
```


#### In NServiceBus

```c#
public class MyClass
{
    static ILog logger = LogManager.GetLogger("MyClass");

    void MyMethod()
    {
        logger.DebugFormat("Method: 'Void MyMethod()'. Line: ~12. TheMessage");
    }
}
```


#### In Serilog

```c#
public class MyClass
{
    static ILogger logger = Log.ForContext<MyClass>();

    void MyMethod()
    {
        if (logger.IsEnabled(LogEventLevel.Debug))
        {
            logger
                .ForContext("MethodName", "Void MyMethod()")
                .ForContext("LineNumber", 8)
                .Debug("TheMessage");
        }
    }
}
```

#### In Splat

```c#
public class MyClass
{
    static IFullLogger logger = ((ILogManager) Locator.Current.GetService(typeof(ILogManager), null))
                                .GetLogger(typeof(ClassWithLogging));

    void MyMethod()
    {
        logger.Debug("Method: 'Void MyMethod()'. Line: ~12. TheMessage");
    }
}
```


### Other Log Overloads in Explicit Logging

There are also appropriate methods for Warn, Info, Error etc as applicable to each of the logging frameworks. 

Each of these methods has the expected 'message', 'params' and 'exception' overloads. 


## Checking logging level

The `LogTo` class also has `IsLevelEnabled` properties that redirect to the respective level enabled checks in each framework. 


### Your code

```c#
public class MyClass
{
    void MyMethod()
    { 
        if (LogTo.IsDebugEnabled)
        {
            LogTo.Debug("TheMessage");
        }
    }
}
```


### What gets compiled

```c#
public class MyClass
{
    static Logger logger = LogManager.GetLogger("MyClass");

    void MyMethod()
    {
        if (logger.IsDebugEnabled)
        {
            logger.Debug("Method: 'Void MyMethod()'. Line: ~12. TheMessage");
        }
    }
}
```


## Delegate Logging

All the `LogTo` methods have equivalent overloads that accept a `Func<string>` instead of a string. This delegate is used to construct the message and should be used when that message construction is resource intensive. At compile time the logging will be wrapped in a `IsEnabled` check so as to only incur the cost if that level of logging is required.


### Your code

```c#
public class MyClass
{
    void MyMethod()
    { 
        LogTo.Debug(()=>"TheMessage");
    }
}
```


### What gets compiled

```c#
public class MyClass
{
    static Logger logger = LogManager.GetLogger("MyClass");

    void MyMethod()
    {
        if (logger.IsDebugEnabled)
        {
            Func<string> messageConstructor = () => "TheMessage";
            logger.Debug("Method: 'Void DebugStringFunc()'. Line: ~58. " + messageConstructor());
        }
    }
}
```


## Exception logging


### Your code

```c#
[LogToErrorOnException]
void MyMethod(string param1, int param2)
{
    //Do Stuff
}
```


### What gets compiled


#### In NLog

```c#
void MyMethod(string param1, int param2)
{
    try
    {
        //Do Stuff
    }
    catch (Exception exception)
    {
        if (logger.IsErrorEnabled)
        {
            var message = string.Format("Exception occurred in SimpleClass.MyMethod. param1 '{0}', param2 '{1}'", param1, param2);
            logger.ErrorException(message, exception);
        }
        throw;
    }
}
```


## Custom logging

The custom logging variant exist for several reasons

  1. Projects targeting an obscure logging libraries i.e. not NLog or SeriLog. Or wraps a logging library with a custom API.
  2. Projects that have their own logging custom logging libraries
  3. Projects that support multiple different logging libraries
  
It works by allowing you to have custom logger construction and a custom logger instance.


### Expected factory and instance formats


#### Factory

The Logger Factory is responsible for building an instance of a logger. 

  * Named `LoggerFactory`.
  * Namespace doesnt matter.
  * Have a static method GetLogger. 
  
For example

```c#
public class LoggerFactory
{
    public static Logger GetLogger<T>()
    {
        return new Logger();
    }
}
```

#### Instance

The Logger instance is responsible for building an instance of a logger. 

  * Name doesn't matter. It will be derived from the return type of `LoggerFactory.GetLogger`.
  * Must not be generic.
  * Namespace doesn't matter.
  * Can be either an interface, a concrete class or an abstract class.
  * Can contain the members listed below. All members are optional. However an build error will be thrown if you attempt to use one of the members that doesn't exist. So for example if you call `LogTo.Debug` and `Logger.Debug` (with the same parameters) doesn't.
  
For example

```c#
public class Logger
{
    public void Trace(string message){}
    public void Trace(string format, params object[] args){}
    public void Trace(Exception exception, string format, params object[] args){}
    public bool IsTraceEnabled { get; private set; }
    public void Debug(string message){}
    public void Debug(string format, params object[] args){}
    public void Debug(Exception exception, string format, params object[] args){}
    public bool IsDebugEnabled { get; private set; }
    public void Information(string message){}
    public void Information(string format, params object[] args){}
    public void Information(Exception exception, string format, params object[] args){}
    public bool IsInformationEnabled { get; private set; }
    public void Warning(string message){}
    public void Warning(string format, params object[] args){}
    public void Warning(Exception exception, string format, params object[] args){}
    public bool IsWarningEnabled { get; private set; }
    public void Error(string message){}
    public void Error(string format, params object[] args){}
    public void Error(Exception exception, string format, params object[] args){}
    public bool IsErrorEnabled { get; private set; }
    public void Fatal(string message){}
    public void Fatal(string format, params object[] args){}
    public void Fatal(Exception exception, string format, params object[] args){}
    public bool IsFatalEnabled { get; private set; }
}
```


### Discovery


#### Current Assembly

If `LoggerFactory` and `Logger` exist in the current assembly they will be picked up automatically.


#### Other Assembly

If `LoggerFactory` and `Logger` exist in a different assembly You will need to use a `[LoggerFactoryAttribute]` to tell Anotar where to look.

    [assembly: LoggerFactoryAttribute(typeof(MyUtilsLibrary.LoggerFactory))]


## Nothing to deploy


After compilation the reference to the Anotar assemblies will be removed so you don't need to deploy the assembly.


## But why? What purpose does this serve?


### 1. Don't make me think

When I am coding I often want to quickly add a line of logging code. If I don't already have the static `logger` field I have to jump back to the top of the file to add it. This breaks my train of thought. I know this is minor but it is still an annoyance. Static logging methods are much less disruptive to call.


### 2. I want some extra information

Often when I am logging I want to know the method and line number I am logging from. I don't want to manually add this. So using IL I just prefix the message with the method name and line number. Note that the line number is prefixed with '~'. The reason for this is that a single line of code can equate to multiple IL instructions. So I walk back up the instructions until I find one that has a line number and use that. Hence it is an approximation.


## I don't want extra information

If you don't want the extra information, method name and line number, then add this to `AssemblyInfo.cs`:

    [assembly: LogMinimalMessage]


## Why not use CallerInfoAttributes

The CallerInfoAttributes consist of  [CallerLineNumberAttribute](http://msdn.microsoft.com/en-us/library/system.runtime.compilerservices.callerlinenumberattribute.aspx),  [CallerFilePathAttribute](http://msdn.microsoft.com/en-us/library/system.runtime.compilerservices.callerfilepathattribute.aspx) and [CallerMemberNameAttribute](http://msdn.microsoft.com/en-us/library/system.runtime.compilerservices.callermembernameattribute.aspx). The allow you to pass information about the caller method to the callee method. 

So some of this could be achieved using these attributes however there are a couple of points that complicate things.


### 1. Only .net 4.5 and up

So this makes it a little difficult to use with other runtimes.


### 2. Can't be used when passing arrays as `params`

Logging APIs all make use of `params` to pass arguments to a `string.Format`. Since you can't use `params` with CallerInfoAttributes most logging APIs choose not to use these attributes.

You can vote for [Compatibility between `params` with CallerInfoAttributes](http://visualstudio.uservoice.com/forums/121579-visual-studio/suggestions/2762025-caller-membername-filepath-linenumber-of-net-4-5-) 


## Icon

Icon courtesy of [The Noun Project](https://thenounproject.com)
