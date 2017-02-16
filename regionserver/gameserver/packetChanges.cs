/home/mlinder/git/server/dol-cvs/GameServer/packets/168/PlayerHeadingUpdateHandler.cs:      DispatchingQueue queue = null;
/home/mlinder/git/server/dol-cvs/GameServer/packets/168/PlayerHeadingUpdateHandler.cs-      foreach (var player in client.Player.GetPlayersInRadius(WorldMgr.VISIBILITY_DISTANCE_PLAYERS)) {
/home/mlinder/git/server/dol-cvs/GameServer/packets/168/PlayerHeadingUpdateHandler.cs:        queue = queue ?? DispatchingQueue.Acquire(tcpAction: clt => clt.Out.SendTCPRaw(outpak190_tcp), dispatchChance: GameServer.DispatchPlayerHeading);
/home/mlinder/git/server/dol-cvs/GameServer/packets/168/PlayerHeadingUpdateHandler.cs-        if (!GlobalConstants.IsVisible(player, client.Player)) {
/home/mlinder/git/server/dol-cvs/GameServer/packets/168/PlayerHeadingUpdateHandler.cs-          continue; // no pos packets --> no radar
/home/mlinder/git/server/dol-cvs/GameServer/packets/168/PlayerHeadingUpdateHandler.cs-        }
/home/mlinder/git/server/dol-cvs/GameServer/packets/168/PlayerHeadingUpdateHandler.cs-        queue.Add(player.Client);
/home/mlinder/git/server/dol-cvs/GameServer/packets/168/PlayerHeadingUpdateHandler.cs-      }
/home/mlinder/git/server/dol-cvs/GameServer/packets/168/PlayerHeadingUpdateHandler.cs-      queue?.FinishUdpBroadcast(buildUdpPacket: () => new Tuple<byte[], int>(outpak190_udp.GetBuffer(), (int)outpak190_udp.Length));
/home/mlinder/git/server/dol-cvs/GameServer/packets/168/PlayerHeadingUpdateHandler.cs-      return 1;
/home/mlinder/git/server/dol-cvs/GameServer/packets/168/PlayerHeadingUpdateHandler.cs-    }
/home/mlinder/git/server/dol-cvs/GameServer/packets/168/PlayerHeadingUpdateHandler.cs-  }
/home/mlinder/git/server/dol-cvs/GameServer/packets/168/PlayerHeadingUpdateHandler.cs-}
--
/home/mlinder/git/server/dol-cvs/GameServer/packets/168/PlayerPositionUpdateHandler.cs-      // Find all palyers, and figure out who receives a TCP and who receives a UDP packet
/home/mlinder/git/server/dol-cvs/GameServer/packets/168/PlayerPositionUpdateHandler.cs:      DispatchingQueue broadcastQueue = null;
/home/mlinder/git/server/dol-cvs/GameServer/packets/168/PlayerPositionUpdateHandler.cs-      foreach (var player in client.Player.GetPlayersInRadius(WorldMgr.VISIBILITY_DISTANCE_PLAYERS)) {
/home/mlinder/git/server/dol-cvs/GameServer/packets/168/PlayerPositionUpdateHandler.cs-        //no position updates in different houses
/home/mlinder/git/server/dol-cvs/GameServer/packets/168/PlayerPositionUpdateHandler.cs-        if (player.CurrentHouse != client.Player.CurrentHouse || player.InHouse != client.Player.InHouse) {
/home/mlinder/git/server/dol-cvs/GameServer/packets/168/PlayerPositionUpdateHandler.cs-          continue;
/home/mlinder/git/server/dol-cvs/GameServer/packets/168/PlayerPositionUpdateHandler.cs-        }
/home/mlinder/git/server/dol-cvs/GameServer/packets/168/PlayerPositionUpdateHandler.cs-        if (!GlobalConstants.IsVisible(player, client.Player)) {
/home/mlinder/git/server/dol-cvs/GameServer/packets/168/PlayerPositionUpdateHandler.cs-          continue;
/home/mlinder/git/server/dol-cvs/GameServer/packets/168/PlayerPositionUpdateHandler.cs-        }
/home/mlinder/git/server/dol-cvs/GameServer/packets/168/PlayerPositionUpdateHandler.cs-        if (player.Zone == null)
/home/mlinder/git/server/dol-cvs/GameServer/packets/168/PlayerPositionUpdateHandler.cs-        {
/home/mlinder/git/server/dol-cvs/GameServer/packets/168/PlayerPositionUpdateHandler.cs-          continue;
/home/mlinder/git/server/dol-cvs/GameServer/packets/168/PlayerPositionUpdateHandler.cs-        }
/home/mlinder/git/server/dol-cvs/GameServer/packets/168/PlayerPositionUpdateHandler.cs:        broadcastQueue = broadcastQueue ?? DispatchingQueue.Acquire(tcpAction: clt => clt.Out.SendPlayerPositionUpdate(client.Player), dispatchChance: GameServer.DispatchPlayerPositionUpdate);
/home/mlinder/git/server/dol-cvs/GameServer/packets/168/PlayerPositionUpdateHandler.cs-        if (!client.Player.IsStealthed || player.CanDetect(client.Player)) {
/home/mlinder/git/server/dol-cvs/GameServer/packets/168/PlayerPositionUpdateHandler.cs-          broadcastQueue.Add(player.Client, forceLegacyBroadcast: player.PlayerGroup != null && player.PlayerGroup == client.Player.PlayerGroup);
/home/mlinder/git/server/dol-cvs/GameServer/packets/168/PlayerPositionUpdateHandler.cs-        } else {
/home/mlinder/git/server/dol-cvs/GameServer/packets/168/PlayerPositionUpdateHandler.cs-          player.Out.SendObjectDelete(client.Player); //remove the stealthed player from view
/home/mlinder/git/server/dol-cvs/GameServer/packets/168/PlayerPositionUpdateHandler.cs-        }
/home/mlinder/git/server/dol-cvs/GameServer/packets/168/PlayerPositionUpdateHandler.cs-      }
/home/mlinder/git/server/dol-cvs/GameServer/packets/168/PlayerPositionUpdateHandler.cs-      broadcastQueue?.FinishUdpBroadcast(buildUdpPacket: () => {
/home/mlinder/git/server/dol-cvs/GameServer/packets/168/PlayerPositionUpdateHandler.cs-        var udpPak = PacketLib168.BuildPlayerPositionUpdatePacket<GSUDPPacketOut>(client.Player, false);
/home/mlinder/git/server/dol-cvs/GameServer/packets/168/PlayerPositionUpdateHandler.cs-        udpPak.WritePacketLength();
/home/mlinder/git/server/dol-cvs/GameServer/packets/168/PlayerPositionUpdateHandler.cs-        var bytes = udpPak.GetBuffer();
/home/mlinder/git/server/dol-cvs/GameServer/packets/168/PlayerPositionUpdateHandler.cs-        return Tuple.Create(bytes, bytes.Length); 
/home/mlinder/git/server/dol-cvs/GameServer/packets/168/PlayerPositionUpdateHandler.cs-      });
/home/mlinder/git/server/dol-cvs/GameServer/packets/168/PlayerPositionUpdateHandler.cs-
/home/mlinder/git/server/dol-cvs/GameServer/packets/168/PlayerPositionUpdateHandler.cs-      #endregion
/home/mlinder/git/server/dol-cvs/GameServer/packets/168/PlayerPositionUpdateHandler.cs-
--
/home/mlinder/git/server/dol-cvs/GameServer/gameobjects/GameNPC.cs:      DispatchingQueue broadcastQueue = null;
/home/mlinder/git/server/dol-cvs/GameServer/gameobjects/GameNPC.cs-
/home/mlinder/git/server/dol-cvs/GameServer/gameobjects/GameNPC.cs-      int visRange = VisibilityDistance;
/home/mlinder/git/server/dol-cvs/GameServer/gameobjects/GameNPC.cs-      if (Rider != null)
/home/mlinder/git/server/dol-cvs/GameServer/gameobjects/GameNPC.cs-      {
/home/mlinder/git/server/dol-cvs/GameServer/gameobjects/GameNPC.cs-        visRange = Math.Max(visRange, WorldMgr.VISIBILITY_DISTANCE_PLAYERS);
/home/mlinder/git/server/dol-cvs/GameServer/gameobjects/GameNPC.cs-      }
/home/mlinder/git/server/dol-cvs/GameServer/gameobjects/GameNPC.cs-
/home/mlinder/git/server/dol-cvs/GameServer/gameobjects/GameNPC.cs-      foreach (var player in GetPlayersInRadius(visRange)) {
/home/mlinder/git/server/dol-cvs/GameServer/gameobjects/GameNPC.cs-        if (broadcastQueue == null) {
/home/mlinder/git/server/dol-cvs/GameServer/gameobjects/GameNPC.cs:          broadcastQueue = DispatchingQueue.Acquire(tcpAction: clt => clt.Out.SendNPCUpdate(this), dispatchChance: GameServer.DispatchNpcUpdate);
/home/mlinder/git/server/dol-cvs/GameServer/gameobjects/GameNPC.cs-        }
/home/mlinder/git/server/dol-cvs/GameServer/gameobjects/GameNPC.cs-        numPlayers++;
/home/mlinder/git/server/dol-cvs/GameServer/gameobjects/GameNPC.cs-        broadcastQueue.Add(player.Client, forceLegacyBroadcast: !PacketBuilder.CanBroadcastNPCUpdate(player, this));
/home/mlinder/git/server/dol-cvs/GameServer/gameobjects/GameNPC.cs-        player.CurrentUpdateArray[ObjectID - 1] = true;
/home/mlinder/git/server/dol-cvs/GameServer/gameobjects/GameNPC.cs-      }
/home/mlinder/git/server/dol-cvs/GameServer/gameobjects/GameNPC.cs-      broadcastQueue?.FinishUdpBroadcast(buildUdpPacket: () => {
/home/mlinder/git/server/dol-cvs/GameServer/gameobjects/GameNPC.cs-        byte[] pak = PacketBuilder.BuildNPCUpdateUDP(this);
/home/mlinder/git/server/dol-cvs/GameServer/gameobjects/GameNPC.cs-        return Tuple.Create(pak, pak.Length);
/home/mlinder/git/server/dol-cvs/GameServer/gameobjects/GameNPC.cs-      });
/home/mlinder/git/server/dol-cvs/GameServer/gameobjects/GameNPC.cs-      if (numPlayers > 0) {
/home/mlinder/git/server/dol-cvs/GameServer/gameobjects/GameNPC.cs-        NPCUpdatedCallback();
/home/mlinder/git/server/dol-cvs/GameServer/gameobjects/GameNPC.cs-      }
/home/mlinder/git/server/dol-cvs/GameServer/gameobjects/GameNPC.cs-    }
--
/home/mlinder/git/server/dol-cvs/GameServer/gameobjects/GameNPC.cs-    }
/home/mlinder/git/server/dol-cvs/GameServer/gameobjects/GameNPC.cs-
/home/mlinder/git/server/dol-cvs/GameServer/gameobjects/GameNPC.cs-    private void BroadcastRemoveMe()
/home/mlinder/git/server/dol-cvs/GameServer/gameobjects/GameNPC.cs-    {
/home/mlinder/git/server/dol-cvs/GameServer/gameobjects/GameNPC.cs-      var oid = ObjectID;
/home/mlinder/git/server/dol-cvs/GameServer/gameobjects/GameNPC.cs:      DispatchingQueue bq = null;
/home/mlinder/git/server/dol-cvs/GameServer/gameobjects/GameNPC.cs-      foreach (GamePlayer player in GetPlayersInRadius(VisibilityDistance))
/home/mlinder/git/server/dol-cvs/GameServer/gameobjects/GameNPC.cs-      {
/home/mlinder/git/server/dol-cvs/GameServer/gameobjects/GameNPC.cs:        bq = bq ?? DispatchingQueue.Acquire(clt => clt.Out.SendRemoveObject(this), GameServer.DispatchRemoveObject);
/home/mlinder/git/server/dol-cvs/GameServer/gameobjects/GameNPC.cs-        bq.Add(player.Client);
/home/mlinder/git/server/dol-cvs/GameServer/gameobjects/GameNPC.cs-        player.RemoveKnownOID(oid);
/home/mlinder/git/server/dol-cvs/GameServer/gameobjects/GameNPC.cs-      }
/home/mlinder/git/server/dol-cvs/GameServer/gameobjects/GameNPC.cs-      bq?.FinishUdpBroadcast(() => {
/home/mlinder/git/server/dol-cvs/GameServer/gameobjects/GameNPC.cs-        var pak = GS.PacketBuilder.BuildRemoveNpcUdp(this);
/home/mlinder/git/server/dol-cvs/GameServer/gameobjects/GameNPC.cs-        return Tuple.Create(pak, pak.Length);
/home/mlinder/git/server/dol-cvs/GameServer/gameobjects/GameNPC.cs-      });
/home/mlinder/git/server/dol-cvs/GameServer/gameobjects/GameNPC.cs-    }
/home/mlinder/git/server/dol-cvs/GameServer/gameobjects/GameNPC.cs-
/home/mlinder/git/server/dol-cvs/GameServer/gameobjects/GameNPC.cs-    /// <summary>
/home/mlinder/git/server/dol-cvs/GameServer/gameobjects/GameNPC.cs-    /// Removes the npc from the world
/home/mlinder/git/server/dol-cvs/GameServer/gameobjects/GameNPC.cs-    /// </summary>
/home/mlinder/git/server/dol-cvs/GameServer/gameobjects/GameNPC.cs-    /// <returns>true if the npc has been successfully removed</returns>
/home/mlinder/git/server/dol-cvs/GameServer/gameobjects/GameNPC.cs-    public override void RemoveFromWorld()
/home/mlinder/git/server/dol-cvs/GameServer/gameobjects/GameNPC.cs-    {
--
/home/mlinder/git/server/dol-cvs/GameServer/gameobjects/GameObject.cs-    /// Broadcasts an effect animation on this object to all players in visibility range
/home/mlinder/git/server/dol-cvs/GameServer/gameobjects/GameObject.cs-    /// </summary>
/home/mlinder/git/server/dol-cvs/GameServer/gameobjects/GameObject.cs-    public virtual void BroadcastSpellEffectAnimation(ushort spellid, ushort boltTime = 0, bool noSound = false, byte success = 1, GameObject spellCaster = null)
/home/mlinder/git/server/dol-cvs/GameServer/gameobjects/GameObject.cs-    {
/home/mlinder/git/server/dol-cvs/GameServer/gameobjects/GameObject.cs-      spellCaster = spellCaster ?? this;
/home/mlinder/git/server/dol-cvs/GameServer/gameobjects/GameObject.cs:      DispatchingQueue bq = null;
/home/mlinder/git/server/dol-cvs/GameServer/gameobjects/GameObject.cs-      foreach (var p in GetPlayersInRadius(WorldMgr.VISIBILITY_DISTANCE_SPELL_EFFECTS)) {
/home/mlinder/git/server/dol-cvs/GameServer/gameobjects/GameObject.cs-        if (!IsVisibleTo(p)) continue;
/home/mlinder/git/server/dol-cvs/GameServer/gameobjects/GameObject.cs:        bq = bq ?? DispatchingQueue.Acquire(
/home/mlinder/git/server/dol-cvs/GameServer/gameobjects/GameObject.cs-          clt => clt.Out.SendSpellEffectAnimation(spellCaster, this, spellid, boltTime, noSound, success),
/home/mlinder/git/server/dol-cvs/GameServer/gameobjects/GameObject.cs-          dispatchChance: GameServer.DispatchSpellEffectAnimation);
/home/mlinder/git/server/dol-cvs/GameServer/gameobjects/GameObject.cs-        bq.Add(p.Client);
/home/mlinder/git/server/dol-cvs/GameServer/gameobjects/GameObject.cs-      }
/home/mlinder/git/server/dol-cvs/GameServer/gameobjects/GameObject.cs-      bq?.FinishUdpBroadcast(() => {
/home/mlinder/git/server/dol-cvs/GameServer/gameobjects/GameObject.cs-        var pak = GS.PacketBuilder.SpellEffectAnimationUDP(spellCaster ?? this, this, spellid, boltTime, noSound, success);
/home/mlinder/git/server/dol-cvs/GameServer/gameobjects/GameObject.cs-        return Tuple.Create(pak, pak.Length);
/home/mlinder/git/server/dol-cvs/GameServer/gameobjects/GameObject.cs-      });
/home/mlinder/git/server/dol-cvs/GameServer/gameobjects/GameObject.cs-    }
/home/mlinder/git/server/dol-cvs/GameServer/gameobjects/GameObject.cs-
