      protected volatile eClientState _state = eClientState.NotConnected;
      /// <summary>
      /// This variable holds all info about the active player
      /// </summary>
      protected volatile GamePlayer m_player;
      /// <summary>
      /// This variable holds the active charindex
      /// </summary>
      protected int      m_activeCharIndex;
      /// <summary>
      /// This variable holds the accountdata
      /// </summary>
      protected Account     m_account;
      /// <summary>
      /// Holds the time of the last ping
      /// </summary>
      protected long m_pingTime = DateTime.Now.Ticks;  // give ping time on creation
      /// <summary>
      /// Constructor for a game client
      /// </summary>
      /// <param name="srvr">The server that's communicating with this client</param>
      public GameClient(BaseServer srvr) : base(srvr)
      {
        LanguageOverride = false;
        m_clientVersion = eClientVersion.VersionNotChecked;
        m_player = null;
        m_activeCharIndex=-1; //No character loaded yet!
        Language = eLang.Default;
        PreferredModelChoice = ModelMgr.ChoiceKey.None;
        ObjectCounter.ObjectCreate(this);
        RegionServerUniqueId = Rand.Next(int.MaxValue);
      }

      /// <summary>
      /// Which models this client prefers
      /// </summary>
      public ModelMgr.ChoiceKey PreferredModelChoice { get; set; }

      /// <summary>
      /// TempProperties
      /// </summary>
      public PropertyCollection Properties { get; set; } = new PropertyCollection();

      /// <summary>
      /// Client Dialog if associated (v1.110+), or null.
      /// Use clientDialog.Dispose() to clear this field.
      /// </summary>
      public ClientDialog.ClientDialog Dialog { get; internal set; }

      /// <summary>
      /// Last Set of Suspicious modules detected
      /// </summary>
      public string LastSuspiciousModules { get; set; }

      //~GameClient()
      //{
      //  ObjectCounter.ObjectDelete(this);
      //}

      /// <summary>
      /// Called when a packet has been received.
--
                Player?.Quit(true); //calls delete
              }
              catch (Exception e)
              {
                log.Error("player cleanup on client quit", e);
              }
              try
              {
                //Now free our objid and sessionid again
                WorldMgr.RemoveClient(this); //calls RemoveSessionID -> player.Delete
              }
              catch (Exception e)
              {
                log.Error("client cleanup on quit", e);
              }
            }          
            ClientState = eClientState.Disconnected;
            Player = null;
            GameEventMgr.Notify(GameClientEvent.Disconnected, this);

            if (MasterAccount != null)
            {
              MasterAccount.QueueGraceEndTime = DateTime.UtcNow + UthgardServer.QueueGraceTime;
              GameServer.Database.SaveObject(MasterAccount);
            }

            if(Account != null)
            {
              if (log.IsInfoEnabled)
              {
                if (NonRegionServerUdpEndPoint != null) {
                  log.Info("(" + NonRegionServerUdpEndPoint.Address.ToString() + ") " + Account + " just disconnected!");
                  ConnectionLog.Info("(" + NonRegionServerUdpEndPoint.Address.ToString() + ") " + Account + " just disconnected!");
                } else {
                  log.Info("(" + TcpEndpoint + ") " + Account + " just disconnected!");
                  ConnectionLog.Info("(" + TcpEndpoint + ") " + Account + " just disconnected!");
                }
              }
            }

            Dialog?.Dispose();
          }
          catch(Exception e)
          {
            log.Error("Quit", e);
          }
        }
      }

      /// <summary>
      /// true if UDP is active
      /// </summary>
      public bool UdpActivated { get; set; }

      /// <summary>
      /// UDP address for this client (if no region server is used; otherwise potentially null)
      /// </summary>
      public IPEndPoint NonRegionServerUdpEndPoint { get; set; }

      /// <summary>
      /// RegionServer this client uses
      /// </summary>
      public IRegionServer RegionServer => Player?.Region.RegionServer;

      /// <summary>
      /// true if UDP is active
      /// </summary>
      public bool RegionServerUdpActivated { get; set; }

      /// <summary>
      /// Region server UDP endpoint
      /// </summary>
      public string RegionServerUdpEndpoint { get; set; }

      /// <summary>
      /// Unique ID sent to the regionserver for client identification
      /// </summary>
      public int RegionServerUniqueId { get; set; }

      /// <summary>
      /// True if UDP traffic for this client should be sent to a region server instead
      /// </summary>
      public bool UsesRegionServer => RegionServerUdpActivated && RegionServer?.IsAvailable == true;

      /// <summary>
      /// Gets or sets the client state
      /// </summary>
      public eClientState ClientState
      {
        get { return _state; }
        set
        {
          if (value == _state) return;
          eClientState oldState = _state;

          // refresh ping timeouts immediately when we change into playing state or charscreen
          if ((oldState!=eClientState.Playing && value==eClientState.Playing) ||
              (oldState!=eClientState.CharScreen && value==eClientState.CharScreen)) 
          {
            PingTime = DateTime.UtcNow.Ticks;
          }          

          _state=value;
          ClientStateChanged = DateTime.UtcNow;
          GameEventMgr.Notify(GameClientEvent.StateChanged, this);
        }
      }

      public DateTime ClientStateChanged { get; private set; }

      /// <summary>
      /// Gets whether or not the client is playing
      /// </summary>
