using DOL.Numbers;

namespace DOL.Geometry {
  /// <summary>
  ///   Geometry utilities for circles
  /// </summary>
  public class Circle {
    /// <summary>
    ///   Gets a uniformly picked random point in the entire circle area
    /// </summary>
    /// <param name="center"></param>
    /// <param name="radius"></param>
    /// <returns></returns>
    public static Vector3 GetRandomPosition(Vector3 center, int radius) {
      // http://stackoverflow.com/questions/5837572/generate-a-random-point-within-a-circle-uniformly
      // We need to use rejection sampling to uniformly end up with a point

      // Pick a random point in the area of the unit circle
      // on avg this should terminate in 1.42 tries
      Vector3 vector;
      do {
        vector = new Vector3(Rand.Double() * 2 - 1, Rand.Double() * 2 - 1, 0);
      } while (vector.SumComponentSqrs() > 1); // x^2 + y^2 > 1^2

      // Use the point from the unit circle to pick our point
      return center + vector * radius;
    }
  }
}