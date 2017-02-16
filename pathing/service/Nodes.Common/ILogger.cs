using System;

namespace Nodes
{
  public interface ILogger
  {
    /// <summary>
    /// Create a logger that marks log events as being from the specified
    /// source type.
    /// </summary>
    /// <typeparam name="TSource">Type generating log messages in the context.</typeparam>
    /// <returns>A logger that will enrich log events as specified.</returns>
    ILogger ForContext<TSource>();

    /// <summary>
    /// Create a logger that marks log events as being from the specified
    /// source type.
    /// </summary>
    /// <param name="source">Type generating log messages in the context.</param>
    /// <returns>A logger that will enrich log events as specified.</returns>
    ILogger ForContext(Type source);

    /// <summary>
    /// Write a log event with the Verbose level and associated exception.
    /// </summary>
    /// <param name="messageTemplate">Message template describing the event.</param>
    /// <param name="propertyValues">Objects positionally formatted into the message template.</param>
    /// <example>
    /// Log.Verbose("Staring into space, wondering if we're alone.");
    /// </example>
    void Verbose(string messageTemplate, params object[] propertyValues);

    /// <summary>
    /// Write a log event with the Verbose level and associated exception.
    /// </summary>
    /// <param name="exception">Exception related to the event.</param>
    /// <param name="messageTemplate">Message template describing the event.</param>
    /// <param name="propertyValues">Objects positionally formatted into the message template.</param>
    /// <example>
    /// Log.Verbose(ex, "Staring into space, wondering where this comet came from.");
    /// </example>
    void Verbose(Exception exception, string messageTemplate, params object[] propertyValues);

    /// <summary>
    /// Write a log event with the Debug level and associated exception.
    /// </summary>
    /// <param name="messageTemplate">Message template describing the event.</param>
    /// <param name="propertyValues">Objects positionally formatted into the message template.</param>
    /// <example>
    /// Log.Debug("Starting up at {StartedAt}.", DateTime.Now);
    /// </example>
    void Debug(string messageTemplate, params object[] propertyValues);

    /// <summary>
    /// Write a log event with the Debug level and associated exception.
    /// </summary>
    /// <param name="exception">Exception related to the event.</param>
    /// <param name="messageTemplate">Message template describing the event.</param>
    /// <param name="propertyValues">Objects positionally formatted into the message template.</param>
    /// <example>
    /// Log.Debug(ex, "Swallowing a mundane exception.");
    /// </example>
    void Debug(Exception exception, string messageTemplate, params object[] propertyValues);

    /// <summary>
    /// Write a log event with the Information level and associated exception.
    /// </summary>
    /// <param name="messageTemplate">Message template describing the event.</param>
    /// <param name="propertyValues">Objects positionally formatted into the message template.</param>
    /// <example>
    /// Log.Information("Processed {RecordCount} records in {TimeMS}.", records.Length, sw.ElapsedMilliseconds);
    /// </example>
    void Information(string messageTemplate, params object[] propertyValues);

    /// <summary>
    /// Write a log event with the Information level and associated exception.
    /// </summary>
    /// <param name="exception">Exception related to the event.</param>
    /// <param name="messageTemplate">Message template describing the event.</param>
    /// <param name="propertyValues">Objects positionally formatted into the message template.</param>
    /// <example>
    /// Log.Information(ex, "Processed {RecordCount} records in {TimeMS}.", records.Length, sw.ElapsedMilliseconds);
    /// </example>
    void Information(Exception exception, string messageTemplate, params object[] propertyValues);

    /// <summary>
    /// Write a log event with the Warning level and associated exception.
    /// </summary>
    /// <param name="messageTemplate">Message template describing the event.</param>
    /// <param name="propertyValues">Objects positionally formatted into the message template.</param>
    /// <example>
    /// Log.Warning("Skipped {SkipCount} records.", skippedRecords.Length);
    /// </example>
    void Warning(string messageTemplate, params object[] propertyValues);

    /// <summary>
    /// Write a log event with the Warning level and associated exception.
    /// </summary>
    /// <param name="exception">Exception related to the event.</param>
    /// <param name="messageTemplate">Message template describing the event.</param>
    /// <param name="propertyValues">Objects positionally formatted into the message template.</param>
    /// <example>
    /// Log.Warning(ex, "Skipped {SkipCount} records.", skippedRecords.Length);
    /// </example>
    void Warning(Exception exception, string messageTemplate, params object[] propertyValues);

    /// <summary>
    /// Write a log event with the Error level and associated exception.
    /// </summary>
    /// <param name="messageTemplate">Message template describing the event.</param>
    /// <param name="propertyValues">Objects positionally formatted into the message template.</param>
    /// <example>
    /// Log.Error("Failed {ErrorCount} records.", brokenRecords.Length);
    /// </example>
    void Error(string messageTemplate, params object[] propertyValues);

    /// <summary>
    /// Write a log event with the Error level and associated exception.
    /// </summary>
    /// <param name="exception">Exception related to the event.</param>
    /// <param name="messageTemplate">Message template describing the event.</param>
    /// <param name="propertyValues">Objects positionally formatted into the message template.</param>
    /// <example>
    /// Log.Error(ex, "Failed {ErrorCount} records.", brokenRecords.Length);
    /// </example>
    void Error(Exception exception, string messageTemplate, params object[] propertyValues);

    /// <summary>
    /// Write a log event with the Fatal level and associated exception.
    /// </summary>
    /// <param name="messageTemplate">Message template describing the event.</param>
    /// <param name="propertyValues">Objects positionally formatted into the message template.</param>
    /// <example>
    /// Log.Fatal("Process terminating.");
    /// </example>
    void Fatal(string messageTemplate, params object[] propertyValues);

    /// <summary>
    /// Write a log event with the Fatal level and associated exception.
    /// </summary>
    /// <param name="exception">Exception related to the event.</param>
    /// <param name="messageTemplate">Message template describing the event.</param>
    /// <param name="propertyValues">Objects positionally formatted into the message template.</param>
    /// <example>
    /// Log.Fatal(ex, "Process terminating.");
    /// </example>
    void Fatal(Exception exception, string messageTemplate, params object[] propertyValues);
  }
}
