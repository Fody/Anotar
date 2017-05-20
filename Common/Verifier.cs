using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
#if XUNIT
using Xunit;
#else
using NUnit.Framework;
#endif

public static class Verifier
{
    public static void Verify(string beforeAssemblyPath, string afterAssemblyPath)
    {
        var before = Validate(beforeAssemblyPath);
        var after = Validate(afterAssemblyPath);
        var message = $@"Failed processing {Path.GetFileName(afterAssemblyPath)}
{after}";
        Assert.True(TrimLineNumbers(before) == TrimLineNumbers(after), message);
    }

    static string Validate(string assemblyPath2)
    {
        var exePath = GetPathToPeVerify();
        if (!File.Exists(exePath))
        {
            return string.Empty;
        }
        var process = Process.Start(new ProcessStartInfo(exePath, "\"" + assemblyPath2 + "\"")
        {
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true
        });

        // ReSharper disable once PossibleNullReferenceException
        process.WaitForExit(10000);
        return process.StandardOutput.ReadToEnd().Trim().Replace(assemblyPath2, "");
    }

    static string GetPathToPeVerify()
    {
        var pathToPeVerify = SdkToolsHelper.GetSdkToolPath("peverify.exe");
#if XUNIT
        Skip.IfNot(File.Exists(pathToPeVerify));
#endif
        return pathToPeVerify;
    }

    static string TrimLineNumbers(string foo)
    {
        return Regex.Replace(foo, @"0x.*]", "");
    }
}