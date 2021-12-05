using System.IO;
using SlimDX;

namespace MNL
{
    public class NiMaterialProperty : NiProperty
    {
        public ushort Flags;
        public Color3 AmbientColor;
        public Color3 DiffuseColor;
        public Color3 SpecularColor;
        public Color3 EmissiveColor;
        public float Glossiness;
        public float Alpha;

        public NiMaterialProperty(NiFile file, BinaryReader reader) : base(file, reader)
        {
            if (Version <= eNifVersion.VER_10_0_1_2)
                Flags = reader.ReadUInt16();

            AmbientColor = reader.ReadColor3();
            DiffuseColor = reader.ReadColor3();
            SpecularColor = reader.ReadColor3();
            EmissiveColor = reader.ReadColor3();

            Glossiness = reader.ReadSingle();
            Alpha = reader.ReadSingle();
        }
    }
}
