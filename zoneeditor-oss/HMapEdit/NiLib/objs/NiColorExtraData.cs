using System.IO;
using SlimDX;

namespace MNL
{
    public class NiColorExtraData : NiExtraData
    {
        public Color4 Data;

        public NiColorExtraData(NiFile file, BinaryReader reader)
            : base(file, reader)
        {
            Data = reader.ReadColor4();
        }
    }
}
