using System.IO;

namespace MNL
{
    public class NiAlphaProperty : NiProperty
    {
        public ushort Flags = 0;
        public byte Threshold = 0;

        public NiAlphaProperty(NiFile file, BinaryReader reader) : base(file, reader)
        {
            Flags = reader.ReadUInt16();
            Threshold = reader.ReadByte();
        }
    }
}