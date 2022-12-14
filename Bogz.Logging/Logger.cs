namespace Bogz.Logging;

public class Logger
{
    private static ILogger _logger;

    public static void InitializeLogger(ILogger logger)
    {
        _logger = logger;
    }

    public static ILogger Instance
    {
        get
        {
            if (_logger == null)
                throw new Exception("No Instance of a Logger Found! Consider using Logger.IntializeLogger();");
            return _logger;
        }
    }

    public static void Dispose()
    {
        if (_logger == null)
            throw new Exception("No Instance of a Logger Found! Consider using Logger.IntializeLogger();");

        if (Instance.GetType() == typeof(ILoggerDisposable) || Instance.GetType() == typeof(Loggers.AsynchronousLogger) || Instance.GetType() == typeof(Loggers.BasicLogger))
            ((ILoggerDisposable)Instance).Dispose();
        else throw new Exception();
    }
}