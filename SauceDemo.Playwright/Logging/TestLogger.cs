using Microsoft.Extensions.Logging;
using NUnit.Framework;
using System.Collections.Concurrent;

namespace SauceDemo.Playwright.Logging;

public sealed class TestLogger
{
    private static readonly AsyncLocal<TestLogger?> CurrentLogger = new();
    private static readonly ConcurrentDictionary<string, TestLogger> TestLoggers = new();
    private static readonly object FileLock = new();
    private readonly string? _logFilePath;

    private TestLogger(string? logFilePath)
    {
        _logFilePath = logFilePath;
    }

    public static TestLogger CreateForCurrentTest()
    {
        try
        {
            var logDirectory = GetLogDirectory();
            Directory.CreateDirectory(logDirectory);

            return new TestLogger(Path.Combine(logDirectory, $"{GetLogFileName()}.log"));
        }
        catch
        {
            return new TestLogger(null);
        }
    }

    public static void SetCurrent(TestLogger? logger)
    {
        CurrentLogger.Value = logger;

        var testId = TestContext.CurrentContext.Test.ID;

        if (logger is null)
        {
            TestLoggers.TryRemove(testId, out _);
        }
        else
        {
            TestLoggers[testId] = logger;
        }
    }

    public static void LogInformation(string message) => GetCurrentLogger()?.Information(message);

    public static void LogWarning(string message) => GetCurrentLogger()?.Warning(message);

    public static void LogError(string message, Exception? exception = null) =>
        GetCurrentLogger()?.Error(message, exception);

    public void Information(string message) => Write(LogLevel.Information, message);

    public void Warning(string message) => Write(LogLevel.Warning, message);

    public void Error(string message, Exception? exception = null) => Write(LogLevel.Error, message, exception);

    private static TestLogger? GetCurrentLogger()
    {
        if (CurrentLogger.Value is not null)
        {
            return CurrentLogger.Value;
        }

        return TestLoggers.TryGetValue(TestContext.CurrentContext.Test.ID, out var logger) ? logger : null;
    }

    private void Write(LogLevel logLevel, string message, Exception? exception = null)
    {
        if (_logFilePath is null)
        {
            return;
        }

        try
        {
            var logEntry = $"{DateTime.Now:O} [{logLevel}] {message}{Environment.NewLine}";

            if (exception is not null)
            {
                logEntry += $"{exception}{Environment.NewLine}";
            }

            lock (FileLock)
            {
                File.AppendAllText(_logFilePath, logEntry);
            }
        }
        catch
        {
            // Logging must not affect test execution.
        }
    }

    private static string GetLogDirectory()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);

        while (directory is not null)
        {
            if (directory.EnumerateFiles("*.csproj").Any())
            {
                return Path.Combine(directory.FullName, "TestResults", "Logs");
            }

            directory = directory.Parent;
        }

        return Path.Combine(TestContext.CurrentContext.WorkDirectory, "TestResults", "Logs");
    }

    private static string GetLogFileName()
    {
        var test = TestContext.CurrentContext.Test;
        var className = test.ClassName?.Split('.').LastOrDefault() ?? "UnknownTestClass";
        var methodName = test.MethodName ?? test.Name;
        var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmssfff");
        var uniqueId = Guid.NewGuid().ToString("N")[..8];

        return $"{SanitizeFileName(className)}_{SanitizeFileName(methodName)}_{timestamp}_{uniqueId}";
    }

    private static string SanitizeFileName(string value)
    {
        foreach (var invalidCharacter in Path.GetInvalidFileNameChars())
        {
            value = value.Replace(invalidCharacter, '_');
        }

        return value;
    }
}
