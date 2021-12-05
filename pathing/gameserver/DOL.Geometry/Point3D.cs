/*
 * DAWN OF LIGHT - The first free open source DAoC server emulator
 * 
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation; either version 2
 * of the License, or (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
 *
 */

namespace DOL.Geometry {
  /// <summary>
  ///   Defines a 3D point
  /// </summary>
  public class Point3D : IPoint3D {
    /// <summary>
    ///   Constructs a new 3D point object
    /// </summary>
    public Point3D() {
    }

    /// <summary>
    ///   Constructs a new 3D point object
    /// </summary>
    /// <param name="x">The X coord</param>
    /// <param name="y">The Y coord</param>
    /// <param name="z">The Z coord</param>
    public Point3D(int x, int y, int z) : this(new Vector3(x, y, z)) {
    }

    /// <summary>
    ///   Constructs a new 3D point object
    /// </summary>
    /// <param name="point">2D point</param>
    /// <param name="z">Z coord</param>
    public Point3D(IPoint2D point, int z) : this(point.X, point.Y, z) {
    }

    /// <summary>
    ///   Constructs a new 3D point object
    /// </summary>
    /// <param name="point">3D point</param>
    public Point3D(IPoint3D point) : this(point.Position) {
    }

    /// <summary>
    ///   Constructs a new 3D point object
    /// </summary>
    /// <param name="point">3D point</param>
    public Point3D(Vector3 pos) {
      Position = pos;
    }

    public Vector3 Position { get; set; }

    /// <summary>
    ///   Calculates the hashcode of this point
    /// </summary>
    /// <returns>The hashcode</returns>
    public override int GetHashCode() {
      return (int) Position.X ^ (int) Position.Y ^ (int) Position.Z;
    }

    /// <summary>
    ///   Creates the string representation of this point
    /// </summary>
    /// <returns></returns>
    public override string ToString() {
      return Position.ToString();
    }

    /// <summary>
    ///   Compares this point to any object
    /// </summary>
    /// <param name="obj">The object to compare</param>
    /// <returns>True if object is IPoint3D and equals this point</returns>
    public override bool Equals(object obj) {
      var point = obj as IPoint3D;
      if (point == null) {
        return false;
      }
      return point.Position == Position;
    }

    /// <summary>
    ///   calculates distance between 2 points
    /// </summary>
    /// <param name="p1"></param>
    /// <param name="p2"></param>
    /// <returns></returns>
    public static int GetDistance(IPoint3D p1, IPoint3D p2) {
      return (int) p1.Position.Distance(p2.Position);
    }

    /// <summary>
    ///   checks if the given points are in a certain distance.
    /// </summary>
    /// <param name="p1"></param>
    /// <param name="p2"></param>
    /// <returns></returns>
    public static bool InRange(IPoint3D p1, IPoint3D p2, int dist) {
      return p1.Position.IsInRange(p2.Position, dist);
    }
  }
}