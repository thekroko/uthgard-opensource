using System.IO;

namespace MNL
{
    public class NiShadeProperty : NiProperty
    {
        public ushort Flags;

        public NiShadeProperty(NiFile file, BinaryReader reader)
            : base(file, reader)
        {
            Flags = reader.ReadUInt16();
        }
    }
}