using System;

namespace NoobCore.Logging;

/// <summary>
/// 
/// </summary>
/// <seealso cref="NoobCore.Logging.ILog" />
public class LazyLogger : ILog
{
    /// <summary>
    /// Gets the type.
    /// </summary>
    /// <value>
    /// The type.
    /// </value>
    public Type Type { get; }
    /// <summary>
    /// Initializes a new instance of the <see cref="LazyLogger"/> class.
    /// </summary>
    /// <param name="type">The type.</param>
    public LazyLogger(Type type) => Type = type;
    /// <summary>
    /// Gets or sets a value indicating whether this instance is debug enabled.
    /// </summary>
    /// <value>
    ///   <c>true</c> if this instance is debug enabled; otherwise, <c>false</c>.
    /// </value>
    public bool IsDebugEnabled => LogManager.GetLogger(Type).IsDebugEnabled;
    /// <summary>
    /// Logs a Debug message.
    /// </summary>
    /// <param name="message">The message.</param>
    public void Debug(object message) => LogManager.LogFactory.GetLogger(Type).Debug(message);
    /// <summary>
    /// Logs a Debug message and exception.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="exception">The exception.</param>
    public void Debug(object message, Exception exception) => LogManager.LogFactory.GetLogger(Type).Debug(message, exception);
    /// <summary>
    /// Logs a Debug format message.
    /// </summary>
    /// <param name="format">The format.</param>
    /// <param name="args">The args.</param>
    public void DebugFormat(string format, params object[] args) => LogManager.LogFactory.GetLogger(Type).DebugFormat(format, args);
    /// <summary>
    /// Logs a Error message.
    /// </summary>
    /// <param name="message">The message.</param>
    public void Error(object message) => LogManager.LogFactory.GetLogger(Type).Error(message);
    /// <summary>
    /// Logs a Error message and exception.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="exception">The exception.</param>
    public void Error(object message, Exception exception) => LogManager.LogFactory.GetLogger(Type).Error(message, exception);
    /// <summary>
    /// Logs a Error format message.
    /// </summary>
    /// <param name="format">The format.</param>
    /// <param name="args">The args.</param>
    public void ErrorFormat(string format, params object[] args) => LogManager.LogFactory.GetLogger(Type).ErrorFormat(format, args);

    /// <summary>
    /// Logs a Fatal message.
    /// </summary>
    /// <param name="message">The message.</param>
    public void Fatal(object message) => LogManager.LogFactory.GetLogger(Type).Fatal(message);

    /// <summary>
    /// Logs a Fatal message and exception.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="exception">The exception.</param>
    public void Fatal(object message, Exception exception) => LogManager.LogFactory.GetLogger(Type).Fatal(message, exception);

    /// <summary>
    /// Logs a Error format message.
    /// </summary>
    /// <param name="format">The format.</param>
    /// <param name="args">The args.</param>
    public void FatalFormat(string format, params object[] args) => LogManager.LogFactory.GetLogger(Type).FatalFormat(format, args);

    /// <summary>
    /// Logs an Info message and exception.
    /// </summary>
    /// <param name="message">The message.</param>
    public void Info(object message) => LogManager.LogFactory.GetLogger(Type).Info(message);

    /// <summary>
    /// Logs an Info message and exception.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="exception">The exception.</param>
    public void Info(object message, Exception exception) => LogManager.LogFactory.GetLogger(Type).Info(message, exception);

    /// <summary>
    /// Logs an Info format message.
    /// </summary>
    /// <param name="format">The format.</param>
    /// <param name="args">The args.</param>
    public void InfoFormat(string format, params object[] args) => LogManager.LogFactory.GetLogger(Type).InfoFormat(format, args);

    /// <summary>
    /// Logs a Warning message.
    /// </summary>
    /// <param name="message">The message.</param>
    public void Warn(object message) => LogManager.LogFactory.GetLogger(Type).Warn(message);

    /// <summary>
    /// Logs a Warning message and exception.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="exception">The exception.</param>
    public void Warn(object message, Exception exception) => LogManager.LogFactory.GetLogger(Type).Warn(message, exception);

    /// <summary>
    /// Warns the format.
    /// </summary>
    /// <param name="format">The format.</param>
    /// <param name="args">The arguments.</param>
    public void WarnFormat(string format, params object[] args) => LogManager.LogFactory.GetLogger(Type).WarnFormat(format, args);
}