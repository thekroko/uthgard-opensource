/*
 * DAWN OF LIGHT - The first free open source DAoC server emulator
 * 
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation; either version 2
 * of the License, or (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
 *
 */

using System.Reflection;
using DOL.GS.PacketHandler.Cryptography;
using log4net;

namespace DOL.GS.PacketHandler.v168 {
  /// <summary>
  ///   Packet handler for the EncryptionRequest
  ///   @author mlinder
  /// </summary>
  [PacketHandler(PacketHandlerType.TCP, 0x5C ^ 168, "Handles crypt key requests")]
  public class CryptKeyRequestHandler : IPacketHandler {
    /// <summary>
    ///   Defines a logger for this class.
    /// </summary>
    private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

    public int HandlePacket(GameClient client, GSPacketIn packet) {
      // e.g. 00 36 01 0B 03                                    .6...
      var rc4 = packet.ReadByte();
      var clientType = (byte) packet.ReadByte();
      client.ClientType = clientType & 0x0F;
      client.ClientAddons = (GameClient.eClientAddons) (clientType & 0xF0);
      var major = (byte) packet.ReadByte();
      var minor = (byte) packet.ReadByte();
      var build = (byte) packet.ReadByte();

      if (rc4 == 0) {
        // Send the plain version response
        client.Out.SendVersionAndCryptKey();

        /* not yet supported. encryption hacked & removed by connect.exe */
        //if (client.Version >= GameClient.eClientVersion.Version1110) {
        //    if (log.IsDebugEnabled)
        //        log.Debug("Switching to RSA encyption for '" + client + "'");

        //    var enc = (PacketEncoding1110)client.PacketProcessor.Encoding;
        //    enc.CryptMode = PacketEncoding1110.eCryptMode.RSA;
        //}
      } else if (rc4 == 1) {
        // RC4 was requested
        if (client.Version >= GameClient.eClientVersion.Version1110) {
          var commonKey = new byte[packet.ReadByte()];
          packet.Read(commonKey, 0, commonKey.Length);

          if (log.IsDebugEnabled) {
            log.Debug("Enabling RC4 for '" + client + "' (commonKey.size=" + commonKey.Length + ")");
          }

          var enc = (PacketEncoding1110) client.PacketProcessor.Encoding;
          enc.CommonKey = commonKey;
          enc.CryptMode = PacketEncoding1110.eCryptMode.RC4;
          client.RegionServerUniqueId++; // rotate ID to cause re-sync

        } else {
          log.Warn("Client '" + client + "' requested RC4, but we don't support encryption.");
        }
      }
      return 1;
    }
  }
}