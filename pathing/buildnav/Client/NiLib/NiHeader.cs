using System;
using System.IO;

namespace MNL {
  public class NiHeader {
    public string VersionString;
    public eNifVersion Version = eNifVersion.VER_UNSUPPORTED;
    public uint UserVersion = 0;
    public uint UserVersion2 = 0;
    public NiString[] BlockTypes;
    public ushort[] BlockTypeIndex;
    public uint[] BlockSizes;
    public uint NumBlocks;
    public uint UnkownInt = 0;

    public NiHeader(NiFile file, BinaryReader reader) {
      var strLen = 0;

      var startOffset = reader.BaseStream.Position;

      while (reader.ReadByte() != 0x0A)
        strLen++;

      reader.BaseStream.Position = startOffset;

      VersionString = new string(reader.ReadChars(strLen));
      reader.ReadByte(); //skip 0x0A
      var ver = reader.ReadUInt32();
      Version = (eNifVersion)ver;
      if (Version >= eNifVersion.VER_20_0_0_4) {
        throw new Exception("NIF Version not supported yet!");
      }
      if (Version >= eNifVersion.VER_10_1_0_0) {
        UserVersion = reader.ReadUInt32();
      }
      if (Version >= eNifVersion.VER_3_3_0_13) {
        NumBlocks = reader.ReadUInt32();
      }

      if (Version >= eNifVersion.VER_10_1_0_0 && (UserVersion == 10 || UserVersion == 11)) {
        UserVersion2 = reader.ReadUInt32();
      }

      if (Version == eNifVersion.VER_20_0_0_5) {
        throw new Exception("Version 20.0.0.5 not supported!");
      }

      if (Version == eNifVersion.VER_10_0_1_2) {
        throw new Exception("NIF Version not supported yet!");
      }

      if (Version >= eNifVersion.VER_10_1_0_0 && (UserVersion == 10 || UserVersion == 11)) {
        throw new Exception("NIF Version not supported yet!");
      }

      if (Version >= eNifVersion.VER_10_0_1_0) {
        var numBlockTypes = reader.ReadUInt16();
        BlockTypes = new NiString[numBlockTypes];
        for (var i = 0; i < numBlockTypes; i++) {
          BlockTypes[i] = new NiString(file, reader);
        }
        BlockTypeIndex = new ushort[NumBlocks];
        for (var i = 0; i < NumBlocks; i++) {
          BlockTypeIndex[i] = reader.ReadUInt16();
        }
      }

      if (Version >= eNifVersion.VER_20_2_0_7) {
        throw new Exception("NIF Version not supported yet!");
      }

      if (Version >= eNifVersion.VER_20_1_0_3) {
        throw new Exception("NIF Version not supported yet!");
      }

      if (Version >= eNifVersion.VER_10_0_1_0) {
        UnkownInt = reader.ReadUInt32();
      }
    }
  }
}
