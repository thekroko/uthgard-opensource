using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.DirectX.Direct3D;

namespace HMapEdit {
    /// <summary>
    /// Local (zone) Textures
    /// </summary>
    public static class LocalTextures {
        private static readonly Dictionary<string, Texture> m_Textures = new Dictionary<string, Texture>();

        /// <summary>
        /// Retrieves a Texture
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static Texture Get(string file, bool noAlt) {
            if (file == "empty")
                return null; //simply.. nothing
            if (string.IsNullOrEmpty(file))
                return noAlt ? null : Program.FORM.renderControl1.OBJSOLID;

            string n = Path.GetFileName(file).ToLower();

            //Otherwise: Try to load
            if (!m_Textures.ContainsKey(n)) {
                string f = "";
                if (File.Exists(file))
                    f = file;

                if (File.Exists(f)) {
                    Console.WriteLine("Loading Texture " + f);
                    Texture t;
                    if (file.Contains("patch")) {
                        t = TextureLoader.FromFile(Program.FORM.renderControl1.DEVICE, f, 0, 0, 1, Usage.Dynamic, Format.A8R8G8B8, Pool.Default,
                                                   Filter.Box, Filter.Box, 0);
                    }
                    else t = TextureLoader.FromFile(Program.FORM.renderControl1.DEVICE, f);
                    m_Textures.Add(n, t);
                }
                else {
                    m_Textures.Add(n, Program.FORM.renderControl1.OBJSOLID);
                    Console.WriteLine("Missing Texture: " + file);
                }
            }

            return m_Textures[n]; //loaded
        }

        /// <summary>
        /// Get by Texture
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static string Get(Texture t) {
            foreach (KeyValuePair<string, Texture> kv in m_Textures) {
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