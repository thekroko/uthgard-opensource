namespace DOL.GS.PacketHandler.Cryptography {
  /// <summary>
  ///   Null packet encoding which won't perform any action
  ///   @author mlinder
  /// </summary>
  internal class NullPacketEncoding : IPacketEncoding {
    public byte[] DecryptPacket(byte[] content, bool udpPacket) {
      return content;
    }

    public byte[] EncryptPacket(byte[] content, bool udpPacket) {
      return content;
    }
  }
}