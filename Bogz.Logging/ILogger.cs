namespace Bogz.Logging;

public interface ILogger
{
    void Log(in LogLevel level, in string message);

    void Log(in LogLevel level, in object message);
}

public interface ILoggerDisposable : IDisposable, ILogger
{
}