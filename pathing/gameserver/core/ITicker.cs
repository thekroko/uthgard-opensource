using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DOL.Core
{
  /// <summary>
  /// Ticker
  /// </summary>
  public interface ITicker {
    /// <summary>
    /// Returns the milliseconds recorded by this ticker
    /// </summary>
    long Milliseconds { get; }
  }

  public sealed class ClockTicker : ITicker {
    public static readonly ClockTicker Default = new ClockTicker();
    public long Milliseconds => DateTime.UtcNow.Ticks / 10000;
  }

  public sealed class FakeTicker : ITicker {
    public long Milliseconds { get; set; }
  }
}
