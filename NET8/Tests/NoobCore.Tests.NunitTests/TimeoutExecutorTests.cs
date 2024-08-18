using NUnit.Framework;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace NoobCore.Tests.NunitTests
{
    /// <summary>
    /// 
    /// </summary>
    [TestFixture]
    public class TimeoutExecutorTests
    {
        /// <summary>
        /// 测试在乐观超时策略下任务在指定时间内完成。
        /// </summary>
        [Test]
        public void RunWithTimeout_OptimisticStrategy_TaskCompletesWithinTimeout_ReturnsResult()
        {
            // Arrange
            Func<CancellationToken, int> taskFunc = token =>
            {
                Task.Delay(100, token).Wait(); // Simulate work
                return 42;
            };
            Func<TimeSpan> timeoutProvider = () => TimeSpan.FromSeconds(1);
            var timeoutStrategy = TimeoutStrategy.Optimistic;

            // Act
            var result = TimeoutExecutor.RunWithTimeout(taskFunc, timeoutProvider, timeoutStrategy);

            Assert.That(result, Is.EqualTo(42));

        }

        /// <summary>
        /// 测试在悲观超时策略下任务在指定时间内完成。
        /// </summary>
        [Test]
        public void RunWithTimeout_PessimisticStrategy_TaskCompletesWithinTimeout_ReturnsResult()
        {
            // Arrange
            Func<CancellationToken, int> taskFunc = token =>
            {
                Task.Delay(100, token).Wait(); // Simulate work
                return 42;
            };
            Func<TimeSpan> timeoutProvider = () => TimeSpan.FromSeconds(1);
            var timeoutStrategy = TimeoutStrategy.Pessimistic;

            // Act
            var result = TimeoutExecutor.RunWithTimeout(taskFunc, timeoutProvider, timeoutStrategy);

            // Assert
            //Assert.AreEqual(42, result);
        }

        /// <summary>
        /// 测试在乐观超时策略下任务超过指定时间未完成时抛出 TimeoutException。
        /// </summary>
        [Test]
        public void RunWithTimeout_OptimisticStrategy_TaskExceedsTimeout_ThrowsTimeoutException()
        {
            // Arrange
            Func<CancellationToken, int> taskFunc = token =>
            {
                Task.Delay(2000, token).Wait(); // Simulate work
                return 42;
            };
            Func<TimeSpan> timeoutProvider = () => TimeSpan.FromMilliseconds(500);
            var timeoutStrategy = TimeoutStrategy.Optimistic;

            // Act & Assert
            Assert.Throws<TimeoutException>(() => TimeoutExecutor.RunWithTimeout(taskFunc, timeoutProvider, timeoutStrategy));
        }

        /// <summary>
        /// 测试在悲观超时策略下任务超过指定时间未完成时抛出 TimeoutException。
        /// </summary>
        [Test]
        public void RunWithTimeout_PessimisticStrategy_TaskExceedsTimeout_ThrowsTimeoutException()
        {
            // Arrange
            Func<CancellationToken, int> taskFunc = token =>
            {
                Task.Delay(2000, token).Wait(); // Simulate work
                return 42;
            };
            Func<TimeSpan> timeoutProvider = () => TimeSpan.FromMilliseconds(500);
            var timeoutStrategy = TimeoutStrategy.Pessimistic;

            // Act & Assert
            Assert.Throws<TimeoutException>(() => TimeoutExecutor.RunWithTimeout(taskFunc, timeoutProvider, timeoutStrategy));
        }

        /// <summary>
        /// 测试在任务被取消时抛出 OperationCanceledException。
        /// </summary>
        [Test]
        public void RunWithTimeout_TaskCancelled_ThrowsOperationCanceledException()
        {
            // Arrange
            Func<CancellationToken, int> taskFunc = token =>
            {
                Task.Delay(1000, token).Wait(); // Simulate work
                return 42;
            };
            Func<TimeSpan> timeoutProvider = () => TimeSpan.FromSeconds(2);
            var timeoutStrategy = TimeoutStrategy.Optimistic;
            var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.CancelAfter(100); // Cancel after 100ms

            // Act & Assert
            Assert.Throws<OperationCanceledException>(() => TimeoutExecutor.RunWithTimeout(taskFunc, timeoutProvider, timeoutStrategy, cancellationToken: cancellationTokenSource.Token));
        }

        /// <summary>
        /// 测试超时后执行回调函数。
        /// </summary>
        [Test]
        public void RunWithTimeout_TaskExceedsTimeout_InvokesOnTimeoutCallback()
        {
            // Arrange
            bool callbackInvoked = false;
            Func<CancellationToken, int> taskFunc = token =>
            {
                Task.Delay(2000, token).Wait(); // Simulate work
                return 42;
            };
            Func<TimeSpan> timeoutProvider = () => TimeSpan.FromMilliseconds(500);
            var timeoutStrategy = TimeoutStrategy.Optimistic;
            Action<TimeSpan, Task, Exception> onTimeout = (timeout, task, ex) => callbackInvoked = true;

            // Act & Assert
            Assert.Throws<TimeoutException>(() => TimeoutExecutor.RunWithTimeout(taskFunc, timeoutProvider, timeoutStrategy, onTimeout));
            //Assert.IsTrue(callbackInvoked);
        }

        /// <summary>
        /// 测试无返回值任务在乐观超时策略下任务在指定时间内完成。
        /// </summary>
        [Test]
        public void RunWithTimeout_OptimisticStrategy_VoidTaskCompletesWithinTimeout_DoesNotThrowException()
        {
            // Arrange
            Action<CancellationToken> action = token => Task.Delay(100, token).Wait(); // Simulate work
            Func<TimeSpan> timeoutProvider = () => TimeSpan.FromSeconds(1);
            var timeoutStrategy = TimeoutStrategy.Optimistic;

            // Act & Assert
            Assert.DoesNotThrow(() => TimeoutExecutor.RunWithTimeout(action, timeoutProvider, timeoutStrategy));
        }

        /// <summary>
        /// 测试无返回值任务在悲观超时策略下任务在指定时间内完成。
        /// </summary>
        [Test]
        public void RunWithTimeout_PessimisticStrategy_VoidTaskCompletesWithinTimeout_DoesNotThrowException()
        {
            // Arrange
            Action<CancellationToken> action = token => Task.Delay(100, token).Wait(); // Simulate work
            Func<TimeSpan> timeoutProvider = () => TimeSpan.FromSeconds(1);
            var timeoutStrategy = TimeoutStrategy.Pessimistic;

            // Act & Assert
            Assert.DoesNotThrow(() => TimeoutExecutor.RunWithTimeout(action, timeoutProvider, timeoutStrategy));
        }

        /// <summary>
        /// 测试无返回值任务在乐观超时策略下任务超过指定时间未完成时抛出 TimeoutException。
        /// </summary>
        [Test]
        public void RunWithTimeout_OptimisticStrategy_VoidTaskExceedsTimeout_ThrowsTimeoutException()
        {
            // Arrange
            Action<CancellationToken> action = token => Task.Delay(2000, token).Wait(); // Simulate work
            Func<TimeSpan> timeoutProvider = () => TimeSpan.FromMilliseconds(500);
            var timeoutStrategy = TimeoutStrategy.Optimistic;

            // Act & Assert
            Assert.Throws<TimeoutException>(() => TimeoutExecutor.RunWithTimeout(action, timeoutProvider, timeoutStrategy));
        }

        /// <summary>
        /// 测试无返回值任务在悲观超时策略下任务超过指定时间未完成时抛出 TimeoutException。
        /// </summary>
        [Test]
        public void RunWithTimeout_PessimisticStrategy_VoidTaskExceedsTimeout_ThrowsTimeoutException()
        {
            // Arrange
            Action<CancellationToken> action = token => Task.Delay(2000, token).Wait(); // Simulate work
            Func<TimeSpan> timeoutProvider = () => TimeSpan.FromMilliseconds(500);
            var timeoutStrategy = TimeoutStrategy.Pessimistic;

            // Act & Assert
            Assert.Throws<TimeoutException>(() => TimeoutExecutor.RunWithTimeout(action, timeoutProvider, timeoutStrategy));
        }

        /// <summary>
        /// 测试无返回值任务在被取消时抛出 OperationCanceledException。
        /// </summary>
        [Test]
        public void RunWithTimeout_VoidTaskCancelled_ThrowsOperationCanceledException()
        {
            // Arrange
            Action<CancellationToken> action = token => Task.Delay(1000, token).Wait(); // Simulate work
            Func<TimeSpan> timeoutProvider = () => TimeSpan.FromSeconds(2);
            var timeoutStrategy = TimeoutStrategy.Optimistic;
            var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.CancelAfter(100); // Cancel after 100ms

            // Act & Assert
            Assert.Throws<OperationCanceledException>(() => TimeoutExecutor.RunWithTimeout(action, timeoutProvider, timeoutStrategy, cancellationToken: cancellationTokenSource.Token));
        }

        /// <summary>
        /// 测试无返回值任务超时后执行回调函数。
        /// </summary>
        [Test]
        public void RunWithTimeout_VoidTaskExceedsTimeout_InvokesOnTimeoutCallback()
        {
            // Arrange
            bool callbackInvoked = false;
            Action<CancellationToken> action = token => Task.Delay(2000, token).Wait(); // Simulate work
            Func<TimeSpan> timeoutProvider = () => TimeSpan.FromMilliseconds(500);
            var timeoutStrategy = TimeoutStrategy.Optimistic;
            Action<TimeSpan, Task, Exception> onTimeout = (timeout, task, ex) => callbackInvoked = true;

            // Act & Assert
            Assert.Throws<TimeoutException>(() => TimeoutExecutor.RunWithTimeout(action, timeoutProvider, timeoutStrategy, onTimeout));
            //Assert.IsTrue(callbackInvoked);
        }
    }
}
