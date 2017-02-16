using System.Collections.Generic;

namespace CEM.Utils {
  /// <summary>
  /// Linq Extensions
  /// </summary>
  internal static class Linq {
    /// <summary>
    /// Shuffles the input
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="inp"></param>
    /// <returns></returns>
    public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> inp) {
      var remaining = new List<T>(inp);
      while (remaining.Count > 0) {
        int ind = Util.Random.Next(0, remaining.Count);
        yield return remaining[ind];
        remaining.RemoveAt(ind);
      }
    }
  }
}