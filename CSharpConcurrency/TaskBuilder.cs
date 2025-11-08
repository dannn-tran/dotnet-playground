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

public class TaskBuilder
{
    private ILogWriter? _logger;
    private Func<Task>? _taskFactory;
    private string? _taskId;

    public TaskBuilder WithLogger(ILogWriter logWriter)
    {
        _logger = logWriter;
        return this;
    }

    public TaskBuilder WithTaskFactory(Func<Task> taskFactory)
    {
        _taskFactory = taskFactory;
        return this;
    }

    public TaskBuilder WithTaskId(string taskId)
    {
        _taskId = taskId;
        return this;
    }

    public async Task Run()
    {
        if (_taskFactory is null)
            throw new InvalidOperationException("TaskFactory not initialized");
            
        var taskId = _taskId ?? Guid.NewGuid().ToString();
        
        try
        {
            _logger?.Log($"Task {taskId} started.");
            await _taskFactory();
            _logger?.Log($"Task {taskId} completed.");
        }
        catch (Exception ex)
        {
            _logger?.Log(ex is OperationCanceledException
                ? $"Task {taskId} is canceled."
                : $"Task {taskId} failed: {ex.Message}");
            throw;
        }
    }
}