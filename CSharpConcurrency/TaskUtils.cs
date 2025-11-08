namespace CSharpConcurrency;

public static class TaskUtils
{
    public static async Task TriggerCancelWhenFails(Func<CancellationToken, Task> taskFactory, CancellationTokenSource cts) 
    {
        try
        {
            await taskFactory(cts.Token);
        }
        catch (Exception e)
        {
            if (e is not OperationCanceledException)
                await cts.CancelAsync();
            throw;
        }
    }

    public static Task WhenAllWithEarlyCancel(Func<CancellationToken, Task>[] taskFactories, CancellationTokenSource cts) =>
        Task.WhenAll(taskFactories.Select(x => TriggerCancelWhenFails(x, cts)));

    public static Task WhenAllWithEarlyCancel(params Func<CancellationToken, Task>[] taskFactories)
    {
        var cts = new CancellationTokenSource();
        return WhenAllWithEarlyCancel(taskFactories, cts);
    }
}