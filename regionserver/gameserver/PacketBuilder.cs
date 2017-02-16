using System;
using System.Reflection;
using DOL.Geometry;
using DOL.GS.PacketHandler;
using log4net;

namespace DOL.GS {
  /// <summary>
  ///   Contains packets that only have to be built once, and can then be immediately send to a lot of clients
  ///   without recreating the entire packet.
  ///   These methods are heavily specialized on Uthgards client and server rules (v1.113, RvR) and will break
  ///   if used in any other scenario.
  ///   @authors mlinder
  /// </summary>
  public static class PacketBuilder {
    /// <summary>
    ///   Defines a logger for this class.
    /// </summary>
    private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

    /// <summary>
    ///   Builds a SpellEffectAnimation packet
    /// </summary>
    /// <returns>byte[] for packetProcessor.Send()</returns>
    public static byte[] SpellEffectAnimationTCP(GameObject spellCaster, GameObject spellTarget, ushort spellid,
      ushort boltTime, bool noSound, byte success) {
      var pak = new GSTCPPacketOut((byte) ePackets.SpellEffectAnimation);
      pak.WriteShort((ushort) spellCaster.ObjectID);
      pak.WriteShort(spellid);
      pak.WriteShort((ushort) (spellTarget == null ? 0 : spellTarget.ObjectID));
      pak.WriteShort(boltTime);
      pak.WriteByte((byte) (noSound ? 1 : 0));
      pak.WriteByte(success);

      //Get the packet buffer
      pak.WritePacketLength();
      return pak.GetBuffer(); //packet.WritePacketLength sets the Capacity
    }

    /// <summary>
    ///   Builds a SpellEffectAnimation packet
    /// </summary>
    /// <returns>byte[] for packetProcessor.Send()</returns>
    public static byte[] SpellEffectAnimationUDP(GameObject spellCaster, GameObject spellTarget, ushort spellid,
      ushort boltTime, bool noSound, byte success)
    {
      var pak = new GSUDPPacketOut((byte)ePackets.SpellEffectAnimation, 10);
      pak.WriteShort((ushort)spellCaster.ObjectID);
      pak.WriteShort(spellid);
      pak.WriteShort((ushort)(spellTarget == null ? 0 : spellTarget.ObjectID));
      pak.WriteShort(boltTime);
      pak.WriteByte((byte)(noSound ? 1 : 0));
      pak.WriteByte(success);

      //Get the packet buffer
      pak.WritePacketLength();
      return pak.GetBuffer(); //packet.WritePacketLength sets the Capacity
    }

    /// <summary>
    /// Remove Object packet (for NPCs
    /// </summary>
    /// <param name="npc"></param>
    /// <returns></returns>
    public static byte[] BuildRemoveNpcUdp(GameNPC npc)
    {
      var oType = npc.IsAlive ? 1 : 0;

      var pak = new GSUDPPacketOut((byte)ePackets.RemoveObject, 4);
      pak.WriteShort((ushort)npc.ObjectID);
      pak.WriteShort((ushort)oType);
      pak.WritePacketLength();
      return pak.GetBuffer();
    }

    /// <summary>
    ///   Builds a combat animation packet for broadcast use.
    ///   TODO: Might not work for attacker==defender -- unsure about this.
    ///   len=14
    /// </summary>
    public static byte[] CombatAnimation(GameObject attacker, GameLiving defender, ushort weaponID, ushort shieldID,
      int style, byte stance, byte result) {
      var pak = new GSTCPPacketOut((byte) ePackets.CombatAnimation);

      if (attacker != null) {
        pak.WriteShort((ushort) (attacker.ObjectState == GameObject.eObjectState.Deleted
          ? attacker.LastObjectID
          : attacker.ObjectID));
      } else {
        pak.WriteShort(0x00);
      }

      byte health = 0x00;
      if (defender != null) {
        pak.WriteShort((ushort) (defender.ObjectState == GameObject.eObjectState.Deleted
          ? defender.LastObjectID
          : defender.ObjectID));
        health = defender.HealthPercent;
      } else {
        pak.WriteShort(0x00);
      }

      pak.WriteShort(weaponID);
      pak.WriteShort(shieldID);
      pak.WriteShortLowEndian((ushort) style);
      pak.WriteByte(stance);
      pak.WriteByte(result);
      pak.WriteByte(health);

      pak.WriteByte(0x6F); // TODO: Check whether this still needs to be 0x6F for attacker==player / 0x00 otherwise

      pak.WritePacketLength();
      return pak.GetBuffer();
    }

    /// <summary>
    /// True if this NPC Update can be broadcasted
    /// </summary>
    /// <param name="ply"></param>
    /// <param name="npc"></param>
    /// <returns></returns>
    public static bool CanBroadcastNPCUpdate(GamePlayer ply , GameNPC npc) {
      if (!ply.IsInGameMode) {
        return false;
      }
      if ((int)npc.Realm > 3) {
        return false;
      }
      return true;
    }

    /// <summary>
    ///   Builds a NPC update packet for a normal player.
    ///   NOTE: Will not work with realm Peace npcs.
    ///   NOTE: Will not work for gm chars
    ///   len=24
    /// </summary>
    /// <returns>byte[] for packetProcessor.Send()</returns>
    public static byte[] BuildNPCUpdateUDP(GameNPC npc) {
      var zone = npc.Zone;
      if (zone == null) {
        if (log.IsWarnEnabled) {
          log.Warn("SendNPCUpdate: npc zone == null. npcID:" + npc.InternalID);
        }
        return null;
      }

      var pos = npc.Position;
      var currentZonePos = zone.GlobalToLocalPos(pos);
      var targetZonePos = Vector3.Zero;

      var speed = 0;
      ushort targetZone = 0;
      var flags = (byte) (GameServer.ServerRules.GetLivingRealm(null, npc) << 6);

      // no name only if normal player
      if (npc.Flags.HasFlag(eClientNPCFlags.CANTTARGET)) {
        flags |= 0x01;
      }
      if (npc.Flags.HasFlag(eClientNPCFlags.DONTSHOWNAME)) {
        flags |= 0x02;
      }
      if (npc.IsUnderwater) {
        flags |= 0x10;
      }
      if (npc.Flags.HasFlag(eClientNPCFlags.FLYING)) {
        flags |= 0x20;
      }

      if (npc.IsMoving && !npc.IsOnTarget()) {
        if (!npc.ClientTarget.IsZero) {
          var tz = npc.Region.GetZone(npc.ClientTarget);
          if (tz != null) {
            targetZone = tz.ClientID;
            targetZonePos = tz.GlobalToLocalPos(npc.ClientTarget);
          }
        }
      }
      var velocity = npc.Velocity;
      var backward = npc.CurrentSpeed < 0;
      speed = Math.Abs((int) new Vector3(velocity.X, velocity.Y, 0).Magnitude);
      speed = Math.Min(speed, 0x7ff);
      if (backward) {
        speed |= 0x800;
      }

      var zspeed = (int) (velocity.Z / 4);
      if (zspeed < 0) {
        zspeed = -zspeed;
        speed |= 0x8000;
      }
      speed |= (zspeed & 0x70) << 8;

      PacketOut pak = new GSUDPPacketOut((byte) ePackets.NPCUpdate, 24+5);
      pak.WriteShorts((ushort) speed, (ushort) (((zspeed & 0x0f) << 12) | (npc.Heading & 0xFFF)),
        (ushort) currentZonePos.X, (ushort) targetZonePos.X, (ushort) currentZonePos.Y, (ushort) targetZonePos.Y,
        (ushort) currentZonePos.Z, (ushort) targetZonePos.Z, (ushort) npc.ObjectID);

      var target = npc.CurrentFollowTarget;
      if (npc.AttackState && target != null && target.ObjectState == GameObject.eObjectState.Active &&
          !npc.IsTurningDisabled) {
        pak.WriteShort((ushort) target.ObjectID);
      } else {
        pak.WriteShort(0x00);
      }

      pak.WriteByte(npc.HealthPercent);

      flags |= (byte) (((zone.ClientID & 0x100) >> 6) | ((targetZone & 0x100) >> 5));
      pak.WriteByte(flags);
      pak.WriteByte((byte) zone.ClientID);
      pak.WriteByte((byte) targetZone);

      //Get the packet buffer
      pak.WritePacketLength();
      return pak.GetBuffer(); //packet.WritePacketLength sets the Capacity
    }

    internal static object BuildRemoveObjectUDP(GameNPC gameNPC)
    {
      throw new NotImplementedException();
    }
  }
}
