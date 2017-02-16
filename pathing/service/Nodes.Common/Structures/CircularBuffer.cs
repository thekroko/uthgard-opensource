using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nodes.Structures {
  /** A circular buffer whose starting index can be shifted */
  class CircularBuffer<T> : IEnumerable<T> {
    private readonly T[] buckets;
    private int currentIndex = 0;

    public CircularBuffer(int size) {
      buckets = new T[size];
    }

    public T this[int bucket]
    {
      get { return buckets[IndexOf(bucket)]; }
      set { buckets[IndexOf(bucket)] = value; }
    }

    public IEnumerator<T> GetEnumerator() {
      for (int i = 0; i < buckets.Length; i++)
        yield return buckets[IndexOf(i)];
    }

    IEnumerator IEnumerable.GetEnumerator() {
      return GetEnumerator();
    }

    int IndexOf(int bucket) {
      return (currentIndex + bucket) % buckets.Length;
    }

    public void Shift() {
      lock (this) {
        currentIndex++;
        currentIndex %= buckets.Length;
      }
    }
  }
}
