﻿//Url：https://github.com/ServiceStack/ServiceStack.Text/blob/master/src/ServiceStack.Text/TaskExtensions.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoobCore
{
    /// <summary>
    /// 
    /// </summary>
    public static class TaskExtensions
    {
        /// <summary>
        /// Successes the specified function.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="task">The task.</param>
        /// <param name="fn">The function.</param>
        /// <param name="onUiThread">if set to <c>true</c> [on UI thread].</param>
        /// <param name="taskOptions">The task options.</param>
        /// <returns></returns>
        public static Task<T> Success<T>(this Task<T> task, Action<T> fn,
            bool onUiThread = true,
            TaskContinuationOptions taskOptions = TaskContinuationOptions.NotOnFaulted | TaskContinuationOptions.OnlyOnRanToCompletion)
        {
            if (onUiThread)
            {
                var source = new CancellationToken();
                task.ContinueWith(t => fn(t.Result), source, taskOptions, TaskScheduler.FromCurrentSynchronizationContext());
            }
            else
            {
                task.ContinueWith(t => fn(t.Result), taskOptions);
            }
            return task;
        }

        /// <summary>
        /// Successes the specified function.
        /// </summary>
        /// <param name="task">The task.</param>
        /// <param name="fn">The function.</param>
        /// <param name="onUiThread">if set to <c>true</c> [on UI thread].</param>
        /// <param name="taskOptions">The task options.</param>
        /// <returns></returns>
        public static Task Success(this Task task, Action fn,
            bool onUiThread = true,
            TaskContinuationOptions taskOptions = TaskContinuationOptions.NotOnFaulted | TaskContinuationOptions.OnlyOnRanToCompletion)
        {
            if (onUiThread)
            {
                var source = new CancellationToken();
                task.ContinueWith(t => fn(), source, taskOptions, TaskScheduler.FromCurrentSynchronizationContext());
            }
            else
            {
                task.ContinueWith(t => fn(), taskOptions);
            }
            return task;
        }

        /// <summary>
        /// Errors the specified function.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="task">The task.</param>
        /// <param name="fn">The function.</param>
        /// <param name="onUiThread">if set to <c>true</c> [on UI thread].</param>
        /// <param name="taskOptions">The task options.</param>
        /// <returns></returns>
        public static Task<T> Error<T>(this Task<T> task, Action<Exception> fn,
            bool onUiThread = true,
            TaskContinuationOptions taskOptions = TaskContinuationOptions.OnlyOnFaulted)
        {
            if (onUiThread)
            {
                var source = new CancellationToken();
                task.ContinueWith(t => fn(t.UnwrapIfSingleException()), source, taskOptions, TaskScheduler.FromCurrentSynchronizationContext());
            }
            else
            {
                task.ContinueWith(t => fn(t.UnwrapIfSingleException()), taskOptions);
            }
            return task;
        }

        /// <summary>
        /// Errors the specified function.
        /// </summary>
        /// <param name="task">The task.</param>
        /// <param name="fn">The function.</param>
        /// <param name="onUiThread">if set to <c>true</c> [on UI thread].</param>
        /// <param name="taskOptions">The task options.</param>
        /// <returns></returns>
        public static Task Error(this Task task, Action<Exception> fn,
            bool onUiThread = true,
            TaskContinuationOptions taskOptions = TaskContinuationOptions.OnlyOnFaulted)
        {
            if (onUiThread)
            {
                var source = new CancellationToken();
                task.ContinueWith(t => fn(t.UnwrapIfSingleException()), source, taskOptions, TaskScheduler.FromCurrentSynchronizationContext());
            }
            else
            {
                task.ContinueWith(t => fn(t.UnwrapIfSingleException()), taskOptions);
            }
            return task;
        }

        /// <summary>
        /// Unwraps if single exception.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="task">The task.</param>
        /// <returns></returns>
        public static Exception UnwrapIfSingleException<T>(this Task<T> task)
        {
            return task.Exception.UnwrapIfSingleException();
        }

        /// <summary>
        /// Unwraps if single exception.
        /// </summary>
        /// <param name="task">The task.</param>
        /// <returns></returns>
        public static Exception UnwrapIfSingleException(this Task task)
        {
            return task.Exception.UnwrapIfSingleException();
        }

        /// <summary>
        /// Unwraps if single exception.
        /// </summary>
        /// <param name="ex">The ex.</param>
        /// <returns></returns>
        public static Exception UnwrapIfSingleException(this Exception ex)
        {
            var aex = ex as AggregateException;
            if (aex == null)
                return ex;

            if (aex.InnerExceptions != null
                && aex.InnerExceptions.Count == 1)
                return aex.InnerExceptions[0].UnwrapIfSingleException();

            return aex;
        }
    }
}
