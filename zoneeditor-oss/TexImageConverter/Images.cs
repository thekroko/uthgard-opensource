using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace ImageConverter
{
    public static class Images
    {
        private static Device Device
        {
            get
            {
                if (m_Device == null)
                {
                    Form f = Application.OpenForms[0];
                    PresentParameters p = new PresentParameters();
                    p.Windowed = true;
                    p.PresentationInterval = PresentInterval.Default;
                    p.SwapEffect = SwapEffect.Discard;
                    f.Invoke(new EventHandler(delegate
                                                  {
                                                      m_Device =
                                                          new Device(0, DeviceType.NullReference, f,
                                                                     CreateFlags.SoftwareVertexProcessing, p);
                                                  }));
                    }

                return m_Device;
            }
        }
        private static Device m_Device = null;
        
        public static Texture LoadFromFile(string file, string arg)
        {
        	Format f = Format.Dxt1;

			switch (arg) {
				case "3":
					f = Format.Dxt3;
					break;
			}

            return TextureLoader.FromFile(Device, file, 0, 0, 1, Usage.None, Format.A8R8G8B8, Pool.Default, Filter.None, Filter.None, 0);
        }
        public static void SaveToFile(Texture bmp, string file, ImageFileFormat f)
        {           
            TextureLoader.Save(file, f, bmp);
        }
        
        public static Bitmap GetBitmap(Texture t)
        {
            Stream s = TextureLoader.SaveToStream(ImageFileFormat.Bmp, t);
            Bitmap b = new Bitmap(s);
            s.Dispose();
            return b;
        }
        public static Texture GetTexture(Bitmap bmp)
        {
            MemoryStream ms = new MemoryStream();
            bmp.Save(ms, ImageFormat.Bmp);
            ms.Capacity = (int)ms.Length;
            return new Texture(Device, ms, Usage.None, Pool.SystemMemory);
        }
    }
}
