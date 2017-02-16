using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using log4net;

namespace DOL.Numbers {
  /// <summary>
  ///   Randon number utilities
  /// </summary>
  public static class Rand {
    private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

    /// <summary>
    ///   Holds the random number generator instance
    /// </summary>
    [ThreadStatic] private static Random _rng;

    /// <summary>
    ///   Gets the random number generator
    /// </summary>
    private static Random RandomGen
    {
      get { return _rng ?? (_rng = new MersenneTwister((uint) DateTime.UtcNow.Ticks)); }
    }

    /// <summary>
    ///   Generates a random number between 0..max inclusive 0 AND max
    /// </summary>
    /// <param name="max"></param>
    /// <returns></returns>
    public static int Next(int max) {
      return RandomGen.Next(max == int.MaxValue ? max : max + 1);
    }

    /// <summary>
    ///   Generates a random number between min..max inclusive min AND max
    /// </summary>
    /// <param name="min"></param>
    /// <param name="max"></param>
    /// <returns></returns>
    public static int Between(int min, int max) {
      return RandomGen.Next(min, max == int.MaxValue ? max : max + 1);
    }

    /// <summary>
    ///   Generates a random number between 0.0 and 1.0.
    /// </summary>
    /// <returns>
    ///   A double-precision floating point number greater than
    ///   or equal to 0.0, and less than 1.0.
    /// </returns>
    public static double Double() {
      return RandomGen.NextDouble();
    }

    /// <summary>
    ///   returns in chancePercent% cases true
    /// </summary>
    /// <param name="chancePercent">0 .. 100</param>
    /// <returns></returns>
    public static bool ChancePercent(int chancePercent) {
      return chancePercent >= Between(1, 100);
    }

    /// <summary>
    ///   returns in chancePercent% cases true
    /// </summary>
    /// <param name="chancePercent">0.0 .. 1.0</param>
    /// <returns></returns>
    public static bool ChanceDouble(double chancePercent) {
      if (chancePercent >= 1.0) return true;
      if (chancePercent <= 0.0) return false;
      return chancePercent > Double();
    }

    /// <summary>
    ///   Fills the given byte array with random numbers
    /// </summary>
    public static void FillBytes(byte[] toFill) {
      RandomGen.NextBytes(toFill);
    }

    /// <summary>
    ///   Randomly chooses one of the elements
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    /// <returns></returns>
    public static T ChooseOne<T>(ICollection<T> list) {
      var count = list.Count;

      if (count <= 1) {
        return list.FirstOrDefault();
      }

      var randomIndex = Next(count - 1);

      try {
        return list.ElementAt(randomIndex);
      }
      catch (ArgumentOutOfRangeException e) {
        log.Error("Exception caught while retrieving random element from list. List count: " + count +
                  ", chosen index: " + randomIndex);
        return list.FirstOrDefault();
      }
    }

    /// <summary>
    ///   Randomly chooses one of the elements
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    /// <returns></returns>
    public static T ChooseOne<T>(T[] list) {
      if (list.Length == 0) {
        return default(T);
      }
      return list[Next(list.Length)];
    }

    /// <summary>
    ///   Weighted random selection
    ///   Chooses from one roulette slot given in as array
    ///   The number in the array specifies the size of the roulette slot
    ///   The bigger the number the higher the chances that the "ball" lands in there
    /// </summary>
    /// <param name="wheelAllocations"></param>
    /// <returns></returns>
    public static int RouletteSelection(int[] wheelAllocations) {
      var rand = Double() * wheelAllocations.Sum();
      double total = 0;

      for (var i = 0; i < wheelAllocations.Length; i++) {
        total += wheelAllocations[i];
        if (rand < total) {
          return i;
        }
      }
      return 0;
    }

    /// <summary>
    ///   Randomly shuffles a list in O(n)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="data"></param>
    public static void Shuffle<T>(ref IList<T> data) {
      // http://en.wikipedia.org/wiki/Knuth_shuffle
      for (var i = data.Count - 1; i >= 1; i--) {
        var j = Between(0, i);
        var tmp = data[j];
        data[j] = data[i];
        data[i] = tmp;
      }
    }
  }
}