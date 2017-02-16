using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using System.Globalization;
using CEM.Utils;

namespace CEM
{
  /// <summary>
  /// LadderWriter
  /// </summary>
  public class LadderWriter : StreamWriter
  {
    public LadderWriter(string file) : base(file, false)
    {
      AutoFlush = true;
      WriteLine("; x, y, z");
      Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
    }

    public void WriteLadder(float x, float y, float z)
    {
      var line = string.Format("{0},{1},{2}", x, y, z);
      WriteLine(line);
      Log.Debug("LADDER: " + line);
    }
  }
}
