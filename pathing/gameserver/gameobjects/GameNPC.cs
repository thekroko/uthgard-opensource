<< SNIP
    /// <summary>
    /// Walk to a certain point with given speed
    /// </summary>
    /// <param name="p"></param>
    /// <param name="speed"></param>
    [Obsolete("Use .PathTo instead")]
    public virtual void WalkTo(IPoint3D p, int speed)
    {
      WalkTo(p.Position, speed);
    }

    /// <summary>
    /// Walk to the spawn point
    /// </summary>
    public virtual void WalkToSpawn()
    {
      StopAttack();
      StopFollow();
      PathTo(SpawnPosition, (int)(MaxSpeed / 2.5));
    }

    /// <summary>
    /// Walk some distance into direction of given target point
    /// </summary>
    /// <param name="directionTarget"></param>
    /// <param name="speed"></param>
    /// <param name="distance"></param>
    [Obsolete("fundamentally does not work well with pathing; avoid if possible")]
    protected async Task WalkDistanceAsync(Vector3 directionTarget, int speed, int distance)
    {
      if (speed == 0) {
        if (directionTarget != Position)
        {
          TurnTo(directionTarget);
        }
        StopMoving();
        return;
      }

--
        return;
      }

      bool movingBackwards = false;
      var moveVector = directionTarget - Position;
      if (distance < 0) {
        movingBackwards = true;
        moveVector.Negate();
        distance = -distance;
      }
      moveVector.Magnitude = distance;
      var destination = Position + moveVector;

      // Fix the destination if it is not on the navmesh. Use MaxSpeeed as the boundary condition here as this is what the follow callback uses for the destination point
      // TODO(mlinder): Might be better to just stop instead of using the corrected vector?
      // TODO(mlinder): Consider using xRange, yRange := 0 here if we don't want NPCs to "evade" to the side
      var navhmeshDestination = movingBackwards 
        ? await PathingMgr.Instance.GetClosestPointAsync(Zone, destination, xRange: MaxSpeed, yRange: MaxSpeed, zRange: MaxSpeed).ConfigureAwait(false) 
        : destination;
      if (navhmeshDestination != null) {
        // TODO(mlinder): Maybe use .PathTo() here instead? But might be overkill
        WalkTo(navhmeshDestination.Value, speed);
      }
    }

    /// <summary>
    /// Helper component for efficiently calculating paths
    /// </summary>
    public PathCalculator PathCalculator { get; protected set; } // Only visible for debugging

    /// <summary>
    /// Finds a valid path to the destination (or picks the direct path otherwise). Uses WalkTo for each of the pathing nodes.
    /// </summary>
    /// <param name="dest"></param>
    /// <param name="speed"></param>
    /// <returns>true if a path was found</returns>
    public async Task<bool> PathTo(IPoint3D dest, int? speed = null, Action<GameNPC> onLastNodeReached = null)
    {
      int walkSpeed = speed ?? MaxSpeed;

      if (DebugLevel == 0)
      {
        DebugSend("PathTo({0}, {1})", dest, walkSpeed);
      }

      Interlocked.Increment(ref Statistics.PathToCalls);

      // Initialize pathing if possible and required
      if (PathCalculator == null && PathCalculator.IsSupported(this))
      {
        // TODO: Only make this check once on spawn since it internally calls .CurrentZone + hashtable lookup?
        PathCalculator = new PathCalculator(this);
      }

      // Pick the next pathing node, and walk towards it
      Vector3? nextNode = null;
      bool didFindPath = false;
      bool shouldUseAirPath = true;
      if (PathCalculator != null)
      {
        var res = await PathCalculator.CalculateNextTargetAsync(dest.Position).ConfigureAwait(false);
        nextNode = res.Item1;
        var noPathReason = res.Item2;
        shouldUseAirPath = noPathReason == NoPathReason.RECAST_FOUND_NO_PATH;
        didFindPath = PathCalculator.DidFindPath;
      }
--
          return false; // no path, and no fallback
        }

        // Directly walk towards the target (or call the customly provided action)
        if (onLastNodeReached != null)
          onLastNodeReached(this); // custom action, e.g. used to start the follow timer
        else
          WalkTo(dest, walkSpeed);
        return true;
      }

      // Do the actual pathing bit: Walk towards the next pathing node
      WalkTo(nextNode.Value, walkSpeed, closeToTargetCallback: npc => npc.PathTo(dest, speed, onLastNodeReached));
      return true;
    }

    /// <summary>
    /// Finds a valid path to the destination (or picks the direct path otherwise). Uses WalkTo for each of the pathing nodes.
    /// </summary>
    /// <param name="dest"></param>
    /// <param name="speed"></param>
    /// <returns>true if a path was found</returns>
    public async Task<bool> PathTo(Vector3 dest, int? speed = null, Action<GameNPC> onLastNodeReached = null)
    {
      // We prefer to use IPoint3D since we can use it to track a target even if it's moving
      return await PathTo(new Point3D(dest), speed, onLastNodeReached).ConfigureAwait(false);
    }

    /// <summary>
    /// Clears all remaining elements in our pathing cache.
    /// </summary>
    public virtual void ClearPathingCache() {
      PathCalculator?.Clear();
    }

    public virtual void FollowTimerCallback()
    {
      << SNIP >>
      if (distance > m_followMinDist) {
        // too far away; use pathing again
        DebugSend("target too far away. distance="+ distance+ ", m_followMinDist="+ m_followMinDist + ", speed="+ speed);
        PathTo(followTarget.Position, speed);
      } else {
        // followspeed = distance, we want to run 1000ms in case we miss a followtimer by lag      
        WalkDistanceAsync(followTarget.Position, speed, speed);
      }
      return FOLLOWCHECKTICKS;
    }

