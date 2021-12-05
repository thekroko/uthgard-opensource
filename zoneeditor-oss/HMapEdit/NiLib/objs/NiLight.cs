using System.IO;
using SlimDX;

namespace MNL
{
    public class NiLight : NiDynamicEffect
    {
        public float Dimmer;
        public Color3 AmbientColor;
        public Color3 DiffuseColor;
        public Color3 SpecularColor;

        public NiLight(NiFile file, BinaryReader reader)
            : base(file, reader)
        {
            Dimmer = reader.ReadSingle();
            AmbientColor = reader.ReadColor3();
            DiffuseColor = reader.ReadColor3();
            SpecularColor = reader.ReadColor3();
        }
    }
}
