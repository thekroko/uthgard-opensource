using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Nodes.Metrics
{
  /** Normal inc/dec counter */
  public class Counter : IComparable<Counter>
  {
    private int val;
    public int Value => val;
    public void Increment() { Interlocked.Add(ref val, 1); }
    public void Decrement() { Interlocked.Add(ref val, -1); }

    public int CompareTo(Counter other) {
      return Value.CompareTo(other.Value);
    }

    public override string ToString() {
      return $"{Value}";
    }
  }

  /// <summary>
  /// By-Key Counter
  /// </summary>
  /// <typeparam name="T"></typeparam>
  public class Counter<T> : AutoDict<T, Counter> { }
}
