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
    /// 执行一个有返回值的任务，并在指定的超时时间内完成。
    /// 如果任务未在指定时间内完成，则抛出TimeoutException异常。
    /// </summary>
    /// <typeparam name="TResult">任务返回值的类型。</typeparam>
    /// <param name="taskFunc">要执行的任务函数。</param>
    /// <param name="timeoutProvider">超时时间提供者函数。</param>
    /// <param name="timeoutStrategy">超时策略。</param>
    /// <param name="onTimeout">超时后的回调函数。</param>
    /// <param name="cancellationToken">用于取消任务的CancellationToken。</param>
    /// <returns>任务的返回值。</returns>
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
    /// 执行一个没有返回值的任务，并在指定的超时时间内完成。
    /// 如果任务未在指定时间内完成，则抛出TimeoutException异常。
    /// </summary>
    /// <param name="action">要执行的任务操作。</param>
    /// <param name="timeoutProvider">超时时间提供者函数。</param>
    /// <param name="timeoutStrategy">超时策略。</param>
    /// <param name="onTimeout">超时后的回调函数。</param>
    /// <param name="cancellationToken">用于取消任务的CancellationToken。</param>
    public static void RunWithTimeout(
        Action<CancellationToken> action,
        Func<TimeSpan> timeoutProvider,
        TimeoutStrategy timeoutStrategy,
        Action<TimeSpan, Task, Exception> onTimeout = null,
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
    /// <typeparam name="TResult">任务返回值的类型。</typeparam>
    /// <param name="taskFunc">要执行的任务函数。</param>
    /// <param name="timeoutProvider">超时时间提供者函数。</param>
    /// <param name="timeoutStrategy">超时策略。</param>
    /// <param name="onTimeout">超时后的回调函数。</param>
    /// <param name="cancellationToken">用于取消任务的CancellationToken。</param>
    /// <returns>任务的返回值。</returns>
    private static TResult RunWithTimeoutInternal<TResult>(
        Func<CancellationToken, TResult> taskFunc,
        Func<TimeSpan> timeoutProvider,
        TimeoutStrategy timeoutStrategy,
        Action<TimeSpan, Task, Exception> onTimeout,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        TimeSpan timeout = timeoutProvider();

        using var timeoutCancellationTokenSource = new CancellationTokenSource();
        using var combinedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeoutCancellationTokenSource.Token);

        try
        {
            if (timeoutStrategy == TimeoutStrategy.Optimistic)
            {
                SystemClock.CancelTokenAfter(timeoutCancellationTokenSource, timeout);
                return taskFunc(combinedTokenSource.Token);
            }

            // Pessimistic timeout strategy
            return ExecuteWithPessimisticTimeout(taskFunc, timeout, combinedTokenSource, timeoutCancellationTokenSource, onTimeout);
        }
        catch (Exception ex) when (ex is OperationCanceledException && timeoutCancellationTokenSource.IsCancellationRequested)
        {
            onTimeout?.Invoke(timeout, null, ex);
            throw new TimeoutException($"The operation timed out after {timeout.TotalMilliseconds} ms.", ex);
        }
    }

    /// <summary>
    /// 执行带有悲观超时策略的任务。
    /// </summary>
    /// <typeparam name="TResult">任务返回值的类型。</typeparam>
    /// <param name="taskFunc">要执行的任务函数。</param>
    /// <param name="timeout">指定的超时时间。</param>
    /// <param name="combinedTokenSource">组合的CancellationTokenSource。</param>
    /// <param name="timeoutCancellationTokenSource">超时CancellationTokenSource。</param>
    /// <param name="onTimeout">超时后的回调函数。</param>
    /// <returns>任务的返回值。</returns>
    private static TResult ExecuteWithPessimisticTimeout<TResult>(
        Func<CancellationToken, TResult> taskFunc,
        TimeSpan timeout,
        CancellationTokenSource combinedTokenSource,
        CancellationTokenSource timeoutCancellationTokenSource,
        Action<TimeSpan, Task, Exception> onTimeout)
    {
        var task = Task.Run(() => taskFunc(combinedTokenSource.Token), combinedTokenSource.Token);

        if (task.Wait(timeout, timeoutCancellationTokenSource.Token))
        {
            return task.Result;
        }
        else
        {
            onTimeout?.Invoke(timeout, task, new TimeoutException($"The operation timed out after {timeout.TotalMilliseconds} ms."));
            throw new TimeoutException($"The operation timed out after {timeout.TotalMilliseconds} ms.");
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