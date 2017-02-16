using System;
using Nodes.Metrics;

namespace Nodes.Pathing.Pathing {
  /** Measures stats on requests */

  class RequestMetrics {
    public readonly QpsCounter<string> RequestsByAction = new QpsCounter<string>();
    public readonly QpsCounter<uint> RequestsByZone = new QpsCounter<uint>();
    public readonly Counter<uint> QueueByZone = new Counter<uint>();

    public readonly QpsCounter<string> QPSByResult = new QpsCounter<string>();

    /** Records a request. Returns a dequeue callback to be called when a request is being processed */

    public Action Record(string action, uint zone) {
      RequestsByAction[action].Increment();
      RequestsByZone[zone].Increment();
      QueueByZone[zone].Increment();
      return () => QueueByZone[zone].Decrement();
    }
  }
}
