using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using DOL.Numbers;
using log4net;

namespace DOL.Core
{
  /// <summary>
  /// Limits the amount of errors a call can throw, and starts rubberstamp blocking requests for a defined window if a certain error rate is exceeded
  /// </summary>
  public class ErrorRateLimiter {
    private readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
//    private static readonly Counter BlockingModeEntered = Metric.Counter("ErrorRateLimiter.BlockingModeEntered", Unit.Calls);
//    private static readonly Meter BlockedRequests = Metric.Meter("ErrorRateLimiter.BlockedRequests", Unit.Calls);

    private readonly ITicker ticker;
    private long? beginningOfWindow;
    private long? beginningOfBlockOutWindow;
    private int numErrors;
    private int numBlockedRequestsInWindow;

    /// <summary>
    /// Maximum number of errors before the limiter gets active
    /// </summary>
    public int MaxErrors { get; set; }

    /// <summary>
    /// Window in which the errors have to occure
    /// </summary>
    public TimeSpan WindowLength { get; set; }

    /// <summary>
    /// Duration for which the limiter will be in blocking mode
    /// </summary>
    public TimeSpan BlockOutTime { get; set; }

    public ErrorRateLimiter(int maxErrors, TimeSpan windowLength, TimeSpan blockOutTime, ITicker ticker = null) {
      MaxErrors = maxErrors;
      WindowLength = windowLength;
      BlockOutTime = blockOutTime;
      this.ticker = ticker ?? ClockTicker.Default;
    }

    /// <summary>
    /// True if this error limiter is currently blocking
    /// </summary>
    public bool IsBlocking => beginningOfBlockOutWindow != null &&
                              ticker.Milliseconds < beginningOfBlockOutWindow + BlockOutTime.TotalMilliseconds;

    private double WindowPercentage
      => !IsBlocking ? 1.0 : (ticker.Milliseconds - beginningOfBlockOutWindow.Value) / BlockOutTime.TotalMilliseconds;

    /// <summary>
    /// The chance with which a request will be blocked in blocking mode
    /// </summary>
    public double BlockChance => 1.0 - WindowPercentage; // slowly unblock in the second half

    /// <summary>
    /// Normal, non-errornous call attempt. Returns null if an error occured, or if the call was throttled
    /// </summary>
    public async Task<T> CallAsync<T>(Func<Task<T>> func) {
      // Still blocking?
      if (IsBlocking) {
        // Roll a dice on how many requests to let pass
        if (Rand.ChanceDouble(BlockChance)) {
          Interlocked.Increment(ref numBlockedRequestsInWindow);
          //BlockedRequests.Mark();
          return default(T);
        }
      }
      // Clear a window otherwise?
      else if (beginningOfBlockOutWindow != null) {
        Interlocked.Exchange(ref numErrors, 0);
        beginningOfBlockOutWindow = null;
        Log.Warn($"ErrorRateLimiter is LEAVING blocking mode (blocked {numBlockedRequestsInWindow} requests)");
      }

      try {
        return await func();
      }
      catch (Exception ex) {
        Log.Warn($"ErrorRateLimiter.Call caught: {ex.Message}");
        IncrementErrors();
        return default(T);
      }
    }

    private void IncrementErrors() {
      lock (this) {
        // Clear window if expired
        if (beginningOfWindow != null && ticker.Milliseconds > beginningOfWindow + WindowLength.TotalMilliseconds) {
          beginningOfWindow = null;
          numErrors = 0;
        }

        // Start new window?
        if (beginningOfWindow == null) {
          beginningOfWindow = ticker.Milliseconds;
        }

        // Record error
        if (numErrors++ >= MaxErrors) {
          Interlocked.Exchange(ref numBlockedRequestsInWindow, 0);
          beginningOfWindow = null;
          beginningOfBlockOutWindow = ticker.Milliseconds;
          //BlockingModeEntered.Increment();
          Log.Warn("ErrorRateLimiter is ENTERING blocking mode");
        }
      }
    }
  }
}
