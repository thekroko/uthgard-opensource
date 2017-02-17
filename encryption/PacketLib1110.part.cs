    public override void SendVersionAndCryptKey() {
      // Packet got smaller. Encryption is now enabled by default.
      var pak = new GSTCPPacketOut(GetPacketCode(ePackets.CryptKey));
      pak.WriteByte(m_gameClient.MajorVersion);
      pak.WriteByte(m_gameClient.MinorVersion);
      pak.WriteByte(m_gameClient.BuildVersion);
      SendTCP(pak);
    }

