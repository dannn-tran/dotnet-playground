namespace CSharpConcurrency;

public interface ILogWriter
{
    void Log(string message);
}

public interface ILogReader
{
    IReadOnlyList<string> GetLogs();
}

public class BasicLoggerWithTimestamp : ILogWriter, ILogReader
{
    private readonly List<string> _logs = [];

    public void Log(string message) => _logs.Add($"[{DateTime.Now:O}] {message}");
    public IReadOnlyList<string> GetLogs() => _logs;
}