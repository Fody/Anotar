using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using Serilog;
using Serilog.Events;

[TestFixture]
public class SerilogTests
{
	string beforeAssemblyPath;
	Assembly assembly;
	public List<LogEvent> Errors = new List<LogEvent>();
	public List<LogEvent> Fatals = new List<LogEvent>();
	public List<LogEvent> Debugs = new List<LogEvent>();
	public List<LogEvent> Infos = new List<LogEvent>();
	public List<LogEvent> Warns = new List<LogEvent>();
	string afterAssemblyPath;

	public SerilogTests()
	{

		beforeAssemblyPath = Path.GetFullPath(@"..\..\..\SerilogAssemblyToProcess\bin\Debug\SerilogAssemblyToProcess.dll");
#if (!DEBUG)
        beforeAssemblyPath = beforeAssemblyPath.Replace("Debug", "Release");
#endif
		afterAssemblyPath = WeaverHelper.Weave(beforeAssemblyPath);
		assembly = Assembly.LoadFile(afterAssemblyPath);


		var eventSink = new EventSink
			{
				Action = LogEvent
			};

		Log.Logger = new LoggerConfiguration()
			.MinimumLevel.Verbose()
			.WriteTo.Sink(eventSink)
			.CreateLogger();
	}

	void LogEvent(LogEvent eventInfo)
	{
		if (eventInfo.Level == LogEventLevel.Debug)
		{
			Debugs.Add(eventInfo);
		}
		if (eventInfo.Level == LogEventLevel.Fatal)
		{
			Fatals.Add(eventInfo);
		}
		if (eventInfo.Level == LogEventLevel.Error)
		{
			Errors.Add(eventInfo);
		}
		if (eventInfo.Level == LogEventLevel.Information)
		{
			Infos.Add(eventInfo);
		}
		if (eventInfo.Level == LogEventLevel.Warning)
		{
			Warns.Add(eventInfo);
		}

	}

	[SetUp]
	public void Setup()
	{
		Fatals.Clear();
		Errors.Clear();
		Debugs.Clear();
		Infos.Clear();
		Warns.Clear();
	}

	[Test]
	public void Generic()
	{
		var type = assembly.GetType("GenericClass`1");
		var constructedType = type.MakeGenericType(typeof (string));
		var instance = (dynamic) Activator.CreateInstance(constructedType);
		instance.Debug();
		var message = Debugs.First();
		Assert.IsTrue(message.MessageTemplate.Text.StartsWith("Method: 'System.Void GenericClass`1::Debug()'. Line: ~"));
	}

	[Test]
	public void Debug()
	{
		var type = assembly.GetType("ClassWithLogging");
		var instance = (dynamic) Activator.CreateInstance(type);
		instance.Debug();
		Assert.AreEqual(1, Debugs.Count);
        Assert.IsTrue(Debugs.First().MessageTemplate.Text.StartsWith("Method: 'System.Void ClassWithLogging::Debug()'. Line: ~"));
	}

	[Test]
	public void ClassWithExistingField()
	{
		var type = assembly.GetType("ClassWithExistingField");
		Assert.AreEqual(1, type.GetFields(BindingFlags.NonPublic | BindingFlags.Static).Count());
		var instance = (dynamic) Activator.CreateInstance(type);
		instance.Debug();
		Assert.AreEqual(1, Debugs.Count);
        Assert.IsTrue(Debugs.First().MessageTemplate.Text.StartsWith("Method: 'System.Void ClassWithExistingField::Debug()'. Line: ~"));
	}

	void CheckException(Action<object> action, List<LogEvent> list, string expected)
	{
		Exception exception = null;
		var type = assembly.GetType("OnException");
		var instance = (dynamic) Activator.CreateInstance(type);
		try
		{
			action(instance);
		}
		catch (Exception e)
		{
			exception = e;
		}
		Assert.IsNotNull(exception);
		Assert.AreEqual(1, list.Count);
		var first = list.First();
        Assert.IsTrue(first.MessageTemplate.Text.StartsWith(expected), first.MessageTemplate.Text);
	}


	[Test]
	public void OnExceptionToDebug()
	{
		var expected = "Exception occurred in 'System.Void OnException::ToDebug(System.String,System.Int32)'.  param1 'x' param2 '6'";
		Action<dynamic> action = o => o.ToDebug("x", 6);
		CheckException(action, Debugs, expected);
	}

	[Test]
	public void OnExceptionToDebugWithReturn()
	{
		var expected = "Exception occurred in 'System.Object OnException::ToDebugWithReturn(System.String,System.Int32)'.  param1 'x' param2 '6'";
		Action<dynamic> action = o => o.ToDebugWithReturn("x", 6);
		CheckException(action, Debugs, expected);
	}

	[Test]
	public void OnExceptionToInfo()
	{
		var expected = "Exception occurred in 'System.Void OnException::ToInfo(System.String,System.Int32)'.  param1 'x' param2 '6'";
		Action<dynamic> action = o => o.ToInfo("x", 6);
		CheckException(action, Infos, expected);
	}

	[Test]
	public void OnExceptionToInfoWithReturn()
	{
		var expected = "Exception occurred in 'System.Object OnException::ToInfoWithReturn(System.String,System.Int32)'.  param1 'x' param2 '6'";
		Action<dynamic> action = o => o.ToInfoWithReturn("x", 6);
		CheckException(action, Infos, expected);
	}

	[Test]
	public void OnExceptionToWarn()
	{
		var expected = "Exception occurred in 'System.Void OnException::ToWarn(System.String,System.Int32)'.  param1 'x' param2 '6'";
		Action<dynamic> action = o => o.ToWarn("x", 6);
		CheckException(action, Warns, expected);
	}

	[Test]
	public void OnExceptionToWarnWithReturn()
	{
		var expected = "Exception occurred in 'System.Object OnException::ToWarnWithReturn(System.String,System.Int32)'.  param1 'x' param2 '6'";
		Action<dynamic> action = o => o.ToWarnWithReturn("x", 6);
		CheckException(action, Warns, expected);
	}

	[Test]
	public void OnExceptionToError()
	{
		var expected = "Exception occurred in 'System.Void OnException::ToError(System.String,System.Int32)'.  param1 'x' param2 '6'";
		Action<dynamic> action = o => o.ToError("x", 6);
		CheckException(action, Errors, expected);
	}

	[Test]
	public void OnExceptionToErrorWithReturn()
	{
		var expected = "Exception occurred in 'System.Object OnException::ToErrorWithReturn(System.String,System.Int32)'.  param1 'x' param2 '6'";
		Action<dynamic> action = o => o.ToErrorWithReturn("x", 6);
		CheckException(action, Errors, expected);
	}

	[Test]
	public void OnExceptionToFatal()
	{
		var expected = "Exception occurred in 'System.Void OnException::ToFatal(System.String,System.Int32)'.  param1 'x' param2 '6'";
		Action<dynamic> action = o => o.ToFatal("x", 6);
		CheckException(action, Fatals, expected);
	}

	[Test]
	public void OnExceptionToFatalWithReturn()
	{
		var expected = "Exception occurred in 'System.Object OnException::ToFatalWithReturn(System.String,System.Int32)'.  param1 'x' param2 '6'";
		Action<dynamic> action = o => o.ToFatalWithReturn("x", 6);
		CheckException(action, Fatals, expected);
	}

	[Test]
	public void DebugString()
	{
		var type = assembly.GetType("ClassWithLogging");
		var instance = (dynamic) Activator.CreateInstance(type);
		instance.DebugString();
		Assert.AreEqual(1, Debugs.Count);
        Assert.IsTrue(Debugs.First().MessageTemplate.Text.StartsWith("Method: 'System.Void ClassWithLogging::DebugString()'. Line: ~"));
	}

	[Test]
	public void DebugStringParams()
	{
		var type = assembly.GetType("ClassWithLogging");
		var instance = (dynamic) Activator.CreateInstance(type);
		instance.DebugStringParams();
		Assert.AreEqual(1, Debugs.Count);
        Assert.IsTrue(Debugs.First().MessageTemplate.Text.StartsWith("Method: 'System.Void ClassWithLogging::DebugStringParams()'. Line: ~"));
	}

	[Test]
	public void DebugStringException()
	{
		var type = assembly.GetType("ClassWithLogging");
		var instance = (dynamic) Activator.CreateInstance(type);
		instance.DebugStringException();
		Assert.AreEqual(1, Debugs.Count);
        Assert.IsTrue(Debugs.First().MessageTemplate.Text.StartsWith("Method: 'System.Void ClassWithLogging::DebugStringException()'. Line: ~"));
	}

	[Test]
	public void Info()
	{
		var type = assembly.GetType("ClassWithLogging");
		var instance = (dynamic) Activator.CreateInstance(type);
		instance.Info();
		Assert.AreEqual(1, Infos.Count);
        Assert.IsTrue(Infos.First().MessageTemplate.Text.StartsWith("Method: 'System.Void ClassWithLogging::Info()'. Line: ~"));
	}

	[Test]
	public void InfoString()
	{
		var type = assembly.GetType("ClassWithLogging");
		var instance = (dynamic) Activator.CreateInstance(type);
		instance.InfoString();
		Assert.AreEqual(1, Infos.Count);
        Assert.IsTrue(Infos.First().MessageTemplate.Text.StartsWith("Method: 'System.Void ClassWithLogging::InfoString()'. Line: ~"));
	}

	[Test]
	public void InfoStringParams()
	{
		var type = assembly.GetType("ClassWithLogging");
		var instance = (dynamic) Activator.CreateInstance(type);
		instance.InfoStringParams();
		Assert.AreEqual(1, Infos.Count);
        Assert.IsTrue(Infos.First().MessageTemplate.Text.StartsWith("Method: 'System.Void ClassWithLogging::InfoStringParams()'. Line: ~"));
	}

	[Test]
	public void InfoStringException()
	{
		var type = assembly.GetType("ClassWithLogging");
		var instance = (dynamic) Activator.CreateInstance(type);
		instance.InfoStringException();
		Assert.AreEqual(1, Infos.Count);
        Assert.IsTrue(Infos.First().MessageTemplate.Text.StartsWith("Method: 'System.Void ClassWithLogging::InfoStringException()'. Line: ~"));
	}

	[Test]
	public void Warn()
	{
		var type = assembly.GetType("ClassWithLogging");
		var instance = (dynamic) Activator.CreateInstance(type);
		instance.Warn();
		Assert.AreEqual(1, Warns.Count);
        Assert.IsTrue(Warns.First().MessageTemplate.Text.StartsWith("Method: 'System.Void ClassWithLogging::Warn()'. Line: ~"));
	}

	[Test]
	public void WarnString()
	{
		var type = assembly.GetType("ClassWithLogging");
		var instance = (dynamic) Activator.CreateInstance(type);
		instance.WarnString();
		Assert.AreEqual(1, Warns.Count);
        Assert.IsTrue(Warns.First().MessageTemplate.Text.StartsWith("Method: 'System.Void ClassWithLogging::WarnString()'. Line: ~"));
	}

	[Test]
	public void WarnStringParams()
	{
		var type = assembly.GetType("ClassWithLogging");
		var instance = (dynamic) Activator.CreateInstance(type);
		instance.WarnStringParams();
		Assert.AreEqual(1, Warns.Count);
        Assert.IsTrue(Warns.First().MessageTemplate.Text.StartsWith("Method: 'System.Void ClassWithLogging::WarnStringParams()'. Line: ~"));
	}

	[Test]
	public void WarnStringException()
	{
		var type = assembly.GetType("ClassWithLogging");
		var instance = (dynamic) Activator.CreateInstance(type);
		instance.WarnStringException();
		Assert.AreEqual(1, Warns.Count);
        Assert.IsTrue(Warns.First().MessageTemplate.Text.StartsWith("Method: 'System.Void ClassWithLogging::WarnStringException()'. Line: ~"));
	}

	[Test]
	public void Error()
	{
		var type = assembly.GetType("ClassWithLogging");
		var instance = (dynamic) Activator.CreateInstance(type);
		instance.Error();
		Assert.AreEqual(1, Errors.Count);
        Assert.IsTrue(Errors.First().MessageTemplate.Text.StartsWith("Method: 'System.Void ClassWithLogging::Error()'. Line: ~"));
	}

	[Test]
	public void ErrorString()
	{
		var type = assembly.GetType("ClassWithLogging");
		var instance = (dynamic) Activator.CreateInstance(type);
		instance.ErrorString();
		Assert.AreEqual(1, Errors.Count);
        Assert.IsTrue(Errors.First().MessageTemplate.Text.StartsWith("Method: 'System.Void ClassWithLogging::ErrorString()'. Line: ~"));
	}

	[Test]
	public void ErrorStringParams()
	{
		var type = assembly.GetType("ClassWithLogging");
		var instance = (dynamic) Activator.CreateInstance(type);
		instance.ErrorStringParams();
		Assert.AreEqual(1, Errors.Count);
        Assert.IsTrue(Errors.First().MessageTemplate.Text.StartsWith("Method: 'System.Void ClassWithLogging::ErrorStringParams()'. Line: ~"));
	}

	[Test]
	public void ErrorStringException()
	{
		var type = assembly.GetType("ClassWithLogging");
		var instance = (dynamic) Activator.CreateInstance(type);
		instance.ErrorStringException();
		Assert.AreEqual(1, Errors.Count);
        Assert.IsTrue(Errors.First().MessageTemplate.Text.StartsWith("Method: 'System.Void ClassWithLogging::ErrorStringException()'. Line: ~"));
	}

	[Test]
	public void Fatal()
	{
		var type = assembly.GetType("ClassWithLogging");
		var instance = (dynamic) Activator.CreateInstance(type);
		instance.Fatal();
		Assert.AreEqual(1, Fatals.Count);
        Assert.IsTrue(Fatals.First().MessageTemplate.Text.StartsWith("Method: 'System.Void ClassWithLogging::Fatal()'. Line: ~"));
	}

	[Test]
	public void FatalString()
	{
		var type = assembly.GetType("ClassWithLogging");
		var instance = (dynamic) Activator.CreateInstance(type);
		instance.FatalString();
		Assert.AreEqual(1, Fatals.Count);
        Assert.IsTrue(Fatals.First().MessageTemplate.Text.StartsWith("Method: 'System.Void ClassWithLogging::FatalString()'. Line: ~"));
	}

	[Test]
	public void FatalStringParams()
	{
		var type = assembly.GetType("ClassWithLogging");
		var instance = (dynamic) Activator.CreateInstance(type);
		instance.FatalStringParams();
		Assert.AreEqual(1, Fatals.Count);
        Assert.IsTrue(Fatals.First().MessageTemplate.Text.StartsWith("Method: 'System.Void ClassWithLogging::FatalStringParams()'. Line: ~"));
	}

	[Test]
	public void FatalStringException()
	{
		var type = assembly.GetType("ClassWithLogging");
		var instance = (dynamic) Activator.CreateInstance(type);
		instance.FatalStringException();
		Assert.AreEqual(1, Fatals.Count);
        Assert.IsTrue(Fatals.First().MessageTemplate.Text.StartsWith("Method: 'System.Void ClassWithLogging::FatalStringException()'. Line: ~"));
	}

	[Test]
	public void PeVerify()
	{
		Verifier.Verify(beforeAssemblyPath, afterAssemblyPath);
	}
}