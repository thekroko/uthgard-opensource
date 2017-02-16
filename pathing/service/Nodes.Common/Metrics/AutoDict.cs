using System.Collections;
using System.Collections.Generic;

namespace Nodes.Metrics
{
  /** Dictionary which automatically initializes types */
  public class AutoDict<TKey, TValue> : IEnumerable<KeyValuePair<TKey,TValue>> where TValue : new()
  {
    private readonly Dictionary<TKey, TValue> dict = new Dictionary<TKey, TValue>();

    public TValue this[TKey key]
    {
      get
      {
        if (!dict.ContainsKey(key))
        {
          lock (dict) {
            dict[key] = new TValue();
          } 
        }
        return dict[key];
      }
      set
      {
        lock (dict) {
          dict[key] = value;
        }
      }
    }

    public IEnumerator<KeyValuePair<TKey,TValue>> GetEnumerator() {
      return dict.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return GetEnumerator();
    }
  }
}
