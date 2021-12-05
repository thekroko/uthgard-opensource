using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;

namespace PatchMap
{
    /// <summary>
    /// Fast Bitmap
    /// </summary>
    public class FastMap : IDisposable
    {
        private BitmapData m_Data = null;

        protected Bitmap m_Bitmap = null;

        ///<summary>
        /// Bitmap
        ///</summary>
        public Bitmap Bitmap {
            get {
                return m_Bitmap;
            }
        }

        protected string m_Name = "";

        ///<summary>
        /// Name
        ///</summary>
        public string Name {
            get { return m_Name; }
        }

        /// <summary>
        /// Create a new FastMap! ;)
        /// </summary>
        /// <param name="b"></param>
        public FastMap(Bitmap b, string name) {
            m_Bitmap = b;
        }

        /// <summary>
        /// Locks the bitmap data
        /// </summary>
        public void Lock() {
            if (m_Data != null)
                return;

            m_Data =
                m_Bitmap.LockBits(new Rectangle(0, 0, m_Bitmap.Width, m_Bitmap.Height), ImageLockMode.ReadWrite,
                                  PixelFormat.Format24bppRgb);
        }

        /// <summary>
        /// Releases the bitmap data
        /// </summary>
        public void Release() {
            if (m_Data == null)
                return;

            m_Bitmap.UnlockBits(m_Data);
            m_Data = null;
        }

        /// <summary>
        /// Disposes the fastmap
        /// </summary>
        public void Dispose() {
            Release();
            m_Bitmap.Dispose();
        }

        /// <summary>
        /// Retrieves a pixel
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public unsafe Color GetPixel(int x, int y) {
            if (m_Data == null)
                throw new InvalidDataException("Fastmap not locked");

            x = Math.Max(Math.Min(x, m_Bitmap.Width - 1), 0);
            y = Math.Max(Math.Min(y, m_Bitmap.Height - 1), 0);

            byte* ptr = (byte*) m_Data.Scan0 + (y*m_Data.Stride) + x*3;
            return Color.FromArgb(ptr[2], ptr[1], ptr[0]);
        }

        /// <summary>
        /// Sets a pixel
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="c"></param>
        public unsafe void SetPixel(int x, int y, Color c) {
            if (m_Data == null)
                throw new InvalidDataException("Fastmap not locked");

            x = Math.Max(Math.Min(x, m_Bitmap.Width - 1), 0);
            y = Math.Max(Math.Min(y, m_Bitmap.Height - 1), 0);

            byte* ptr = (byte*) m_Data.Scan0 + (y*m_Data.Stride) + x*3;
            ptr[2] = c.R;
            ptr[1] = c.G;
            ptr[0] = c.B;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
