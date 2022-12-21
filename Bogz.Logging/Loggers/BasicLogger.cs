namespace Bogz.Logging.Loggers;

public class BasicLogger : ILoggerDisposable
{
    private StreamWriter _streamWriter;

    public BasicLogger(string path = "")
    {
        if (!path.Equals(string.Empty))
            _streamWriter = new StreamWriter(path);
    }

    public void Dispose()
    {
        if (_streamWriter == null) return;

        _streamWriter.Close();
        _streamWriter.Dispose();
    }

    public void Log(in LogLevel level, in string message)
    {
        string msg = $"[{level}] {message}";

        ConsoleColor color;
        switch (level)
        {
            case LogLevel.Info:
                color = ConsoleColor.Gray;
                break;

            case LogLevel.Warning:
                color = ConsoleColor.Yellow;
                break;

            case LogLevel.Error:
                color = ConsoleColor.Red;
                break;

            case LogLevel.FatalError:
                color = ConsoleColor.DarkRed;
                break;

            case LogLevel.Success:
                color = ConsoleColor.Green;
                break;

            default:
                color = Console.ForegroundColor;
                break;
        }
        Console.ForegroundColor = color;
        Console.WriteLine(msg);
        Console.ForegroundColor = ConsoleColor.White;
    }

    public void Log(in LogLevel level, in object message)
    {
        Log(level, message.ToString());
    }
}