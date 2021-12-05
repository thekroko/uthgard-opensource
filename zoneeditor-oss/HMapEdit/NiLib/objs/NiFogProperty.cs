using System.IO;
using SlimDX;

namespace MNL
{
    public class NiFogProperty : NiProperty
    {
        public ushort Flags;
        public float Depth;
        public Color3 Color;

        public NiFogProperty(NiFile file, BinaryReader reader) : base(file, reader)
        {
            Flags = reader.ReadUInt16();
            Depth = reader.ReadSingle();
            Color = reader.ReadColor3();
        }
    }
}