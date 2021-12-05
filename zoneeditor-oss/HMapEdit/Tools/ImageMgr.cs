using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace HMapEdit.Tools {
    /// <summary>
    /// ImageMgr
    /// </summary>
    public static class ImageMgr {
        private static readonly Dictionary<string, Image> m_Images = new Dictionary<string, Image>();
        private static readonly Dictionary<string, string> m_Paths = new Dictionary<string, string>();

        static ImageMgr() {
            foreach (string path in Directory.GetFiles(Objects.DIR_OBJECTS, "*.jpg", SearchOption.AllDirectories)) {
                string file = path.ToLower();

                while (file.Contains("."))
                    file = Path.GetFileNameWithoutExtension(file);

                if (!m_Paths.ContainsKey(file))
                    m_Paths.Add(file, path);
            }
        }

        /// <summary>
        /// Retrieves an object image
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static Image Get(string name) {
          if (!File.Exists(name))
            return null;

            string n = name.ToLower();

            while (n.Contains("."))
                n = Path.GetFileNameWithoutExtension(n);

            if (!m_Images.ContainsKey(n)) {
                if (m_Paths.ContainsKey(n))
                    m_Images.Add(n, Image.FromFile(m_Paths[n]));
                else
                    m_Images.Add(n, null);
            }

            return m_Images[n];
        }
    }
}