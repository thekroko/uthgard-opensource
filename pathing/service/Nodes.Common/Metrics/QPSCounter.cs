using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Nodes.Structures;

namespace Nodes.Metrics
{
  /// <summary>
  /// Counts QPS
  /// </summary>
  public class QpsCounter : IComparable<QpsCounter>
  {
    const int NUM_SECONDS = 2; // more buckets more smoothing
    const int NUM_BUCKETS = NUM_SECONDS + 1; // one WIP bucket

    private readonly CircularBuffer<int> numRequests = new CircularBuffer<int>(NUM_BUCKETS);

    public long TotalRequests { get; private set; }

    public QpsCounter()
    {
      var t = new Timer {Interval = 1000};
      t.Elapsed += (o, e) => { numRequests.Shift(); numRequests[0] = 0; };
      t.Start(); // dirty to start timer in constructor ...
    }

    public void Increment() { numRequests[0]++; TotalRequests++; }
    public double Value => numRequests.Skip(1).Average();

    public int CompareTo(QpsCounter other) {
      return Value.CompareTo(other.Value);
    }

    public override string ToString() {
      return $"{Value:F1}";
    }
  }

  /// <summary>
  /// By-Key QPS Counter
  /// </summary>
  /// <typeparam name="T"></typeparam>
  public class QpsCounter<T> : AutoDict<T, QpsCounter> { }
}
