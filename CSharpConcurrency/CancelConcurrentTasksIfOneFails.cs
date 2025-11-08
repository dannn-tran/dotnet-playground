namespace CSharpConcurrency;

public static class CancelConcurrentTasksIfOneFails
{
    /// <summary>
    /// Sample run:
    /// [2025-11-08T14:08:27.8283300+08:00] Task Ok started.
    /// [2025-11-08T14:08:27.8622500+08:00] Task Err started.
    /// [2025-11-08T14:08:28.1657510+08:00] Task Err failed: err!!
    /// [2025-11-08T14:08:28.1690830+08:00] Task Ok is canceled.
    /// [2025-11-08T14:08:28.1699010+08:00] Exception caught: err!!
    /// </summary>
    public static void Main()
    {
        var logs = DoWork().Result;
        foreach (var log in logs)
            Console.WriteLine(log);
    }

    private static async Task<IReadOnlyList<string>> DoWork()
    {
        var logger = new BasicLoggerWithTimestamp();
        
        var okTaskFactory = (CancellationToken ct) => new TaskBuilder()
            .WithLogger(logger)
            .WithTaskId("Ok")
            .WithTaskFactory(() => Task.Delay(1000, ct))
            .Run();
        var errTaskFactory = (CancellationToken ct) => new TaskBuilder()
            .WithLogger(logger)
            .WithTaskId("Err")
            .WithTaskFactory(async () =>
            {
                await Task.Delay(300, ct);
                throw new InvalidOperationException("err!!");
            })
            .Run();
            
        try
        {
            await TaskUtils.WhenAllWithEarlyCancel(okTaskFactory, errTaskFactory);
        }
        catch (Exception ex)
        {
            logger.Log($"Exception caught: {ex.Message}");
        }

        return logger.GetLogs();
    }
}