using System.Collections;
using System.Collections.Generic;

namespace DOL.Numbers {
  /// <summary>
  ///   Represents a sliding, one-dimensional window of a certain size
  /// </summary>
  public sealed class SlidingWindow<T> : IEnumerable<T> {
    private readonly T[] window;
    private int currentWriteIndex;

    /// <summary>
    ///   Creates a new, empty sliding window of the given size
    /// </summary>
    public SlidingWindow(int size) {
      window = new T[size];
    }

    /// <summary>
    ///   Number of items in this sliding window
    /// </summary>
    public int Length
    {
      get { return window.Length; }
    }

    /// <summary>
    ///   Accesses elements from the sliding window
    /// </summary>
    /// <param name="id">ID where 0 is the oldest item in the window, and n-1 the newest item</param>
    public T this[int id]
    {
      get { return window[(currentWriteIndex + id) % Length]; }
    }

    public IEnumerator<T> GetEnumerator() {
      return (IEnumerator<T>) window.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator() {
      return GetEnumerator();
    }

    /// <summary>
    ///   Records a new value in the sliding window
    /// </summary>
    /// <param name="value"></param>
    public void Record(T value) {
      window[currentWriteIndex++] = value;
      currentWriteIndex %= window.Length;
    }

    /// <summary>
    ///   Returns an array where the first element is the oldest element in the sliding window
    /// </summary>
    public T[] ToArray() {
      var copy = new T[Length];
      for (var i = 0; i < copy.Length; i++) {
        copy[i] = this[i];
      }
      return copy;
    }
  }
}