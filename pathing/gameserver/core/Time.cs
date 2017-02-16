using System;
using System.Text.RegularExpressions;
using CuttingEdge.Conditions;

namespace DOL.Numbers {
  /// <summary>
  ///   Time Helpers
  /// </summary>
  public static class Time {
    private static readonly Regex TimeUnitRegex = new Regex("^(?<value>[0-9.]+)(?<unit>[a-zA-Z]*)");

    /// <summary>
    ///   Parses a timespan in the "5d 5m 10s" format
    /// </summary>
    public static TimeSpan ParseTimespan(string str) {
      var seconds = 0.0;
      foreach (var arg in str.Split(' ')) {
        var match = TimeUnitRegex.Match(arg);
        Condition.Requires(match.Success, arg).IsTrue("Could not parse time unit");
        double value = int.Parse(match.Groups["value"].Value);
        seconds += value * GetMultiplier(match.Groups["unit"].Value);
      }
      return TimeSpan.FromSeconds(seconds);
    }

    /// <summary>
    ///   Returns the time unit multiplier for the given unit (relative to seconds)
    /// </summary>
    private static double GetMultiplier(string unit) {
      switch (unit.ToLowerInvariant()) {
        default:
          throw new NotImplementedException("Unknown unit: " + unit);
        case "millis":
        case "ms":
          return 1.0 / 1000;
        case "secs":
        case "sec":
        case "s":
        case "":
          return 1;
        case "mins":
        case "min":
        case "m":
          return 60;
        case "hours":
        case "hour":
        case "h":
          return 60 * 60;
        case "days":
        case "day":
        case "d":
          return 24 * 60 * 60;
        case "weeks":
        case "week":
        case "w":
          return 7 * 24 * 60 * 60;
        case "months":
        case "month":
          return 30 * 24 * 60 * 60;
        case "years":
        case "year":
        case "y":
          return 365 * 24 * 60 * 60;
      }
    }

    /// <summary>
    ///   Format Time String
    /// </summary>
    public static string FormatTimeString(long time, bool millisec = false) {
      var sec = time / 1000;
      var min = sec / 60;
      var hours = min / 60;
      var days = hours / 24;
      var message = "";
      if (days != 0) {
        message = message + string.Format("{0} days ", days);
      }
      if (hours % 24 != 0) {
        message = message + string.Format("{0} hours ", hours % 24);
      }
      if (min % 60 != 0) {
        message = message + string.Format("{0} minutes ", min % 60);
      }
      if (sec % 60 != 0) {
        message = message + string.Format("{0:00} seconds", sec % 60);
      }
      if (millisec) {
        if (time % 1000 != 0) {
          message = message + string.Format(" {0:0000} millis", time % 1000);
        }
      }
      return message;
    }
  }
}