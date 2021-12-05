using System;
using System.Collections.Generic;
using System.IO;
using HMapEdit.Tools;
using Microsoft.DirectX.Direct3D;

namespace HMapEdit {
  /// <summary>
  /// TerrainTextures
  /// </summary>
  public static class TerrainTex {
    private static readonly Dictionary<string, Texture> m_Textures = new Dictionary<string, Texture>();

    /// <summary>
    /// Retrieves a Texture
    /// </summary>
    /// <param name="file"></param>
    /// <returns></returns>
    public static Texture Get(string file) {
      string n = Path.GetFileName(file);

      //Otherwise: Try to load
      if (!m_Textures.ContainsKey(n)) {
        Stream fs = GameData.FindTerrainTex(file);

        if (fs != null) {
          Console.WriteLine("Loading Texture " + n);
          Texture t = TextureLoader.FromStream(Program.FORM.renderControl1.DEVICE, fs);
          m_Textures.Add(n, t);
        }
        else
          return null;
      }

      return m_Textures[n]; //loaded
    }

    /// <summary>
    /// Get by Texture
    /// </summary>
    /// <param name="t"></param>
    /// <returns></returns>
    public static string Get(Texture t) {
      if (t == null)
        return null;

      foreach (var kv in m_Textures) {
        if (kv.Value == t)
          return kv.Key;
      }

      return null;
    }

    /// <summary>
    /// Clears all textures
    /// </summary>
    public static void Clear() {
      m_Textures.Clear();
    }
  }
}