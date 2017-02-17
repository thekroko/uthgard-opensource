namespace DOL.GS.PacketHandler.Cryptography {
  /// <summary>
  ///   CryptLib for RC4
  ///   @author mlinder
  /// </summary>
  internal interface ICryptLibRC4 {
    /// <summary>
    ///   Encodes the packet using Mythic RC4
    /// </summary>
    void EncodeMythicRC4Packet(byte[] packet, int offset, int len, byte[] sbox, bool udpPacket);

    /// <summary>
    ///   Decodes the packet using Mythic RC4
    /// </summary>
    /// <param name="buf"></param>
    /// <param name="sbox"></param>
    void DecodeMythicRC4Packet(byte[] buf, int offset, int len, byte[] sbox, bool udpPacket);
  }
}