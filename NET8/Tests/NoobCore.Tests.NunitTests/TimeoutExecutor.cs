namespace NoobCore.Tests.NunitTests;

/// <summary>
/// Timeout strategy.
/// </summary>
public enum TimeoutStrategy
{
    /// <summary>
    /// Optimistic strategy will wait for the timeout and then return the result if the task is completed.
    /// </summary>
    Optimistic,
    /// <summary>
    /// Pessimistic strategy will wait for the timeout and then throw an exception if the task is not completed.
    /// </summary>
    Pessimistic
}

/// <summary>
/// TimeoutExecutor类，用于执行带有超时的任务。
/// </summary>
public static class TimeoutExecutor
{
    /// <summary>
    /// 执行一个有返回值的任务，并在指定的超时时间内完成。如果任务未在指定时间内完成，则抛出 TimeoutException 异常。
    /// </summary>
    public static TResult RunWithTimeout<TResult>(
        Func<CancellationToken, TResult> taskFunc,
        Func<TimeSpan> timeoutProvider,
        TimeoutStrategy timeoutStrategy,
        Action<TimeSpan, Task, Exception> onTimeout = null,
        CancellationToken cancellationToken = default)
    {
        return RunWithTimeoutInternal(taskFunc, timeoutProvider, timeoutStrategy, onTimeout, cancellationToken);
    }

    /// <summary>
    /// 执行一个没有返回值的任务，并在指定的超时时间内完成。如果任务未在指定时间内完成，则抛出 TimeoutException 异常。
    /// </summary>
    public static void RunWithTimeout(
        Action<CancellationToken> action,
        Func<TimeSpan> timeoutProvider,
        TimeoutStrategy timeoutStrategy,
        Action<TimeSpan, Task, Exception>? onTimeout = null,
        CancellationToken cancellationToken = default)
    {
        RunWithTimeoutInternal(token =>
        {
            action(token);
            return Task.CompletedTask;
        }, timeoutProvider, timeoutStrategy, onTimeout, cancellationToken).Wait();
    }

    /// <summary>
    /// 内部方法：处理任务并处理超时逻辑。
    /// </summary>
    private static TResult RunWithTimeoutInternal<TResult>(
        Func<CancellationToken, TResult> taskFunc,
        Func<TimeSpan> timeoutProvider,
        TimeoutStrategy timeoutStrategy,
        Action<TimeSpan, Task, Exception>? onTimeout,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        TimeSpan timeout = timeoutProvider();

        using var timeoutCancellationTokenSource = new CancellationTokenSource();
        using var combinedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeoutCancellationTokenSource.Token);

        if (timeoutStrategy == TimeoutStrategy.Optimistic)
        {
            return ExecuteOptimisticTimeout(taskFunc, timeout, combinedTokenSource, timeoutCancellationTokenSource, cancellationToken);
        }
        else
        {
            return ExecuteWithPessimisticTimeout(taskFunc, timeout, combinedTokenSource, timeoutCancellationTokenSource, onTimeout);
        }
    }

    /// <summary>
    /// 执行带有乐观超时策略的任务。
    /// </summary>
    private static TResult ExecuteOptimisticTimeout<TResult>(
        Func<CancellationToken, TResult> taskFunc,
        TimeSpan timeout,
        CancellationTokenSource combinedTokenSource,
        CancellationTokenSource timeoutCancellationTokenSource,
        CancellationToken cancellationToken)
    {
        SystemClock.CancelTokenAfter(timeoutCancellationTokenSource, timeout);

        var task = Task.Run(() => taskFunc(combinedTokenSource.Token), combinedTokenSource.Token);

        var completedTask = Task.WhenAny(task, Task.Delay(timeout, timeoutCancellationTokenSource.Token)).Result;

        if (completedTask == task)
        {
            try
            {
                return task.Result;  // 任务在超时时间内完成
            }
            catch (AggregateException ex)
            {
                // 处理取消操作
                if (ex.Flatten().InnerExceptions.Any(e => e is TaskCanceledException))
                {
                    throw new OperationCanceledException("操作已被取消。", ex, cancellationToken);
                }
                throw;
            }
        }
        else
        {
            throw new TimeoutException($"操作在 {timeout.TotalMilliseconds} 毫秒后超时。");
        }
    }

    /// <summary>
    /// 执行带有悲观超时策略的任务。
    /// </summary>
    private static TResult ExecuteWithPessimisticTimeout<TResult>(
        Func<CancellationToken, TResult> taskFunc,
        TimeSpan timeout,
        CancellationTokenSource combinedTokenSource,
        CancellationTokenSource timeoutCancellationTokenSource,
        Action<TimeSpan, Task, Exception>? onTimeout)
    {
        var task = Task.Run(() => taskFunc(combinedTokenSource.Token), combinedTokenSource.Token);

        if (task.Wait(timeout, timeoutCancellationTokenSource.Token))
        {
            return task.Result;
        }
        else
        {
            onTimeout?.Invoke(timeout, task, new TimeoutException($"操作在 {timeout.TotalMilliseconds} 毫秒后超时。"));
            throw new TimeoutException($"操作在 {timeout.TotalMilliseconds} 毫秒后超时。");
        }
    }

    /// <summary>
    /// 一个模拟系统时钟的方法，用于设置任务的取消时间。
    /// </summary>
    private static class SystemClock
    {
        public static void CancelTokenAfter(CancellationTokenSource cts, TimeSpan timeout)
        {
            if (timeout > TimeSpan.Zero)
            {
                cts.CancelAfter(timeout);
            }
        }
    }
}