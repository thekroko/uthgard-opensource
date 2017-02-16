using System;

namespace Nodes.Impl
{
  /// <summary>
  /// Proxies all calls to a Serilog logger
  /// </summary>
  internal sealed class SerilogLoggerProxy : ILogger {
    private readonly Serilog.ILogger logger;

    public SerilogLoggerProxy(Serilog.ILogger logger) {
      this.logger = logger;
    }

    public ILogger ForContext<TSource>() {
      return new SerilogLoggerProxy(logger.ForContext<TSource>());
    }

    public ILogger ForContext(Type source) {
      return new SerilogLoggerProxy(logger.ForContext(source));
    }

    public void Verbose(string messageTemplate, params object[] propertyValues) {
      logger.Verbose(messageTemplate, propertyValues);
    }

    public void Verbose(Exception exception, string messageTemplate, params object[] propertyValues) {
      logger.Verbose(exception, messageTemplate, propertyValues);
    }

    public void Debug(string messageTemplate, params object[] propertyValues) {
      logger.Debug(messageTemplate, propertyValues);
    }

    public void Debug(Exception exception, string messageTemplate, params object[] propertyValues) {
      logger.Debug(exception, messageTemplate, propertyValues);
    }

    public void Information(string messageTemplate, params object[] propertyValues) {
      logger.Information(messageTemplate, propertyValues);
    }

    public void Information(Exception exception, string messageTemplate, params object[] propertyValues) {
      logger.Information(exception, messageTemplate, propertyValues);
    }

    public void Warning(string messageTemplate, params object[] propertyValues) {
      logger.Warning(messageTemplate, propertyValues);
    }

    public void Warning(Exception exception, string messageTemplate, params object[] propertyValues) {
      logger.Warning(exception, messageTemplate, propertyValues);
    }

    public void Error(string messageTemplate, params object[] propertyValues) {
      logger.Error(messageTemplate, propertyValues);
    }

    public void Error(Exception exception, string messageTemplate, params object[] propertyValues) {
      logger.Error(exception, messageTemplate, propertyValues);
    }

    public void Fatal(string messageTemplate, params object[] propertyValues) {
      logger.Fatal(messageTemplate, propertyValues);
    }

    public void Fatal(Exception exception, string messageTemplate, params object[] propertyValues) {
      logger.Fatal(exception, messageTemplate, propertyValues);
    }
  }
}
