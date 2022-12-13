using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bogz.Logging.Loggers;

public struct LogMessage
{
    public LogMessage(string message, LogLevel level)
    {
        Message = message;
        Level = level;
    }

    public string Message { get; set; }
    public LogLevel Level { get; set; }
}

public class BasicLogger : ILoggerDisposable
{
    private Queue<LogMessage> logMessages;
    private StreamWriter? writer;
    private Task? task;

    public BasicLogger(string logFile)
    {
        if (logFile.Equals(string.Empty))
            return;
        if (File.Exists(logFile)) File.Delete(logFile);

        logMessages = new Queue<LogMessage>();
        writer = new StreamWriter(logFile, new FileStreamOptions() { Access = FileAccess.Write, Mode = FileMode.CreateNew, Options = FileOptions.Asynchronous });
    }


    public void Log(LogLevel level, string message)
    {
        try
        {

            logMessages.Enqueue(new(message, level));
            if (task?.Status == TaskStatus.Running) return;

            task = Task.Factory.StartNew(() => MessageLoggingCallback());
        }
        catch { }
    }

    private void MessageLoggingCallback()
    {
        StringBuilder sb = new StringBuilder();
        while (logMessages.Any())
        {
            var msgQ = logMessages.Dequeue();

            string msg = $"[{msgQ.Level}] {msgQ.Message}";

            sb.AppendLine(msg);

            ConsoleColor color;
            switch (msgQ.Level)
            {
                case LogLevel.Info:
                    color = ConsoleColor.Green;
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
                    color = ConsoleColor.Gray;
                    break;
                default:
                    color = Console.ForegroundColor;
                    break;
            }
            Console.ForegroundColor = color;
            Console.WriteLine(msg);
            Console.ForegroundColor = ConsoleColor.White;

            if (sb.Length > 100)
            {
                writer?.Write(sb.ToString());
                sb.Clear();
            }
        }
        if (sb.Length > 0)
            writer?.Write(sb.ToString());
    }

    public void Log(LogLevel level, object message)
    {
        Log(level, message);
    }
    public void Dispose()
    {
        MessageLoggingCallback();

        Console.WriteLine("Flushing logger.");
        writer?.Flush();
        Console.WriteLine("Disposing logger.");
        writer?.Dispose();
        Console.WriteLine("Logger Finalized.");
    }
}
