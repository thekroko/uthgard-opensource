using System.IO;
using SlimDX;

namespace MNL
{
    public class SkinTransform
    {
        public Matrix Rotation;
        public Vector3 Translation;
        public float Scale;

        public SkinTransform(NiFile file, BinaryReader reader)
        {
            Rotation = reader.ReadMatrix33();
            Translation = reader.ReadVector3();
            Scale = reader.ReadSingle();
        }
    }
}
