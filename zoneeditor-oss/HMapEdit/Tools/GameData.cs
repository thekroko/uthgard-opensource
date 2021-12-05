using System.Collections.Generic;
using System.IO;
using System.Linq;
using Connect.Utils;

namespace HMapEdit.Tools
{
  /// <summary>
  /// Game resource class
  /// </summary>
  static class GameData
  {
    /// <summary>
    /// Path to the game
    /// </summary>
    private static string GamePath
    {
      get { return Program.Arguments.GameDirectory; }
    }

      /// <summary>
    /// True if the specified file exists
    /// </summary>
    /// <param name="file"></param>
    /// <returns></returns>
    public static bool Exists(string file) {
      Stream fs = null;
      try {
        fs = Open(file);
        return fs != null;
      } finally {
        if (fs != null)
          fs.Close(); 
      }
    }

    /// <summary>
    /// Opens the specified file readonly
    /// </summary>
    /// <param name="file"></param>
    /// <returns></returns>
    public static Stream Open(string file) {
      if (GamePath == null)
        return null;

      string curPath = GamePath;
      string[] components = file.ToLower().Split('\\');
      string nifComponent = null;

      foreach (var comp in components) {
        if (nifComponent != null) {
          nifComponent = Path.Combine(nifComponent, comp);
        }
        else {
          curPath = Path.Combine(curPath, comp);
          if (comp.EndsWith(".npk") || comp.EndsWith(".mpk"))
            nifComponent = "";
        } 
      }

      // Normal file/NIF?
      if (!File.Exists(curPath))
        return null;
      if (nifComponent != null) {
        var mpk = TinyMPK.FromFile(curPath);
        var subfile = mpk.GetFile(nifComponent);
        return subfile == null ? null : new MemoryStream(subfile.Data);
      }
      return new FileStream(curPath, FileMode.Open, FileAccess.Read);
    }

    /// <summary>
    /// Opens the first match
    /// </summary>
    /// <param name="files"></param>
    /// <returns></returns>
    public static Stream OpenFirst(IEnumerable<string> files)
    {
      foreach (string file in files)
      {
        if (Exists(file))
          return Open(file);
      }
      return null;
    }

    /// <summary>
    /// Opens the specified nif file
    /// </summary>
    /// <param name="nif"></param>
    /// <returns></returns>
    public static Stream FindNIF(string nif)
    {
      var files = new[] {nif, Path.Combine(nif.ToLower().Replace(".nif", ".npk"), nif)};
      return OpenFirst(BuildPathPermutations(GamePaths, NIFFolders, files));
    }

    /// <summary>
    /// Finds a specific terraintex
    /// </summary>
    /// <param name="tex"></param>
    /// <returns></returns>
    public static Stream FindTerrainTex(string tex) {
      return OpenFirst(BuildPathPermutations(GamePaths, TerrainTexFolders, new[] {tex}));
    }

    /// <summary>
    /// Returns all available terraintex *.dds files
    /// </summary>
    /// <returns></returns>
    public static IEnumerable<string> FindAllTerrainTex() {
      return
        BuildPathPermutations(GamePaths, TerrainTexFolders)
        .Select(d => Path.Combine(GamePath, d))
        .Where(Directory.Exists)
        .SelectMany(x => Directory.GetFiles(x, "*.dds"))
        .Distinct();
    }

    /// <summary>
    /// Returns all world dirs (zones/, frontiers/, ...)
    /// </summary>
    /// <returns></returns>
    private static IEnumerable<string> GamePaths
    {
      get
      {
        yield return "newtowns";
        yield return "frontiers";
        yield return "phousing";
        yield return "";
      }
    }

    /// <summary>
    /// Generates all possible nif paths relative to the game dir
    /// </summary>
    /// <returns></returns>
    private static IEnumerable<string> NIFFolders {
      get {
        yield return "nifs";
        yield return Path.Combine("zones", "nifs");
      }
    }

    /// <summary>
    /// Generates all possible tex paths relative to the game dir
    /// </summary>
    /// <returns></returns>
    private static IEnumerable<string> TerrainTexFolders
    {
      get
      {
        yield return "terraintex";
        yield return Path.Combine("zones", "terraintex");
      }
    }

    #region Awful permutation methods
    private static IEnumerable<string> BuildPathPermutations(IEnumerable<string> a, IEnumerable<string> b) {
      foreach (var elemA in a) {
        foreach (var elemB in b) {
          yield return Path.Combine(elemA, elemB);
        }
      }
    }

    private static IEnumerable<string> BuildPathPermutations(IEnumerable<string> first, params IEnumerable<string>[] rest) {
      if (rest.Length == 0) { /* only one set */
        return first;
      } else if (rest.Length == 1) {
        return BuildPathPermutations(first, rest[0]);
      } else {
        var parts = BuildPathPermutations(first, rest[0]);
        var remainder = rest.Skip(1).ToArray();
        return BuildPathPermutations(parts, remainder);
      }
    }
    #endregion
  }
}
