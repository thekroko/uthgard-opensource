using Ninject;
using Nodes.Rpc;
using Nodes.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Timer = System.Timers.Timer;

namespace Nodes.Pathing.Pathing
{
  class GUIModule : AbstractTaskModule
  {
    private bool healthy = false;
    public override bool IsHealthy => healthy;

    [Inject]
    private LoadedNavmeshes Navmeshes { get; set; }
		
		[Inject]
    private RequestMetrics Metrics { get; set; }

    public override Task Run()
    {
		  Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;

      while (!Navmeshes.IsLoadingComplete) {
        Thread.Sleep(1000);
      }

      var t = new Timer();
      t.Interval = 1000;
      t.Elapsed += (o, e) => UpdateGUI();
      t.Start();
			healthy = true;
      return Task.CompletedTask;
    }

    protected override void ConfigureModules()
    {
    }

		private void DrawProgressBar(int x, int y, int NAME_LEN, string name, double percent, string valStr) {
			int BAR_LEN = 26 - NAME_LEN;
			Console.ForegroundColor = ConsoleColor.Yellow;
			Console.SetCursorPosition(x, y);
			Console.Write(name.PadLeft(NAME_LEN));

			Console.ForegroundColor = ConsoleColor.White;
			Console.SetCursorPosition(x + NAME_LEN + 1, y);
			Console.Write("[");
			Console.ForegroundColor = percent <= 1.0 ? (percent <= 0.75 ? ConsoleColor.Gray : ConsoleColor.Yellow): ConsoleColor.Red;
      
			int full = (int)Math.Round(BAR_LEN * Math.Min(1.0, percent));
			Console.Write("".PadLeft(full, '='));
			Console.Write("".PadLeft(BAR_LEN - full, ' '));
			
			Console.ForegroundColor = ConsoleColor.White;
			Console.Write("] ");
      
			Console.ForegroundColor = ConsoleColor.Gray;
			Console.Write(valStr);
		}

		private int frame;

    void UpdateGUI()
    {
			if (frame % 20 == 3)
        Console.Clear();
			frame++;
  
			// Header line
			Console.ForegroundColor = ConsoleColor.White;
      Console.SetCursorPosition(0, 0);
      Console.Write("Nodes.Pathing");
      Console.SetCursorPosition(20, 0);
      Console.Write("Shard {0}/{1}", NavmeshLoadingModule.ShardIndex + 1, NavmeshLoadingModule.ShardCount);
      Console.SetCursorPosition(40, 0);
      Console.Write("Port {0}", RpcServerTask.RpcServerPort);
      Console.SetCursorPosition(70, 0);
      Console.Write(DateTime.UtcNow);

			const int TABLE_START_Y = 1;
      const int REQUESTS_X = 0;
      const int MESHES_X = 38;
      const int MESHES2_X = 75;

      // Table line
		  Console.ForegroundColor = ConsoleColor.Green;
			Console.SetCursorPosition(REQUESTS_X, TABLE_START_Y);
			Console.Write("Requests by Action [QPS]");
			Console.SetCursorPosition(MESHES_X, TABLE_START_Y);
			Console.Write("Requests by Navmesh [QPS]");
			Console.SetCursorPosition(MESHES2_X, TABLE_START_Y);
			Console.Write("Queued Requests");

      // Metrics
			const double QPS_LIMIT = 1000.0;
			const double SHARD_LIMIT = 100.0;
			const double QUEUE_LIMIT = 3;
			int row = TABLE_START_Y + 1;
      foreach (var kv in Metrics.RequestsByAction) {
        var qps = kv.Value.Value;
				DrawProgressBar(REQUESTS_X, row++, 12, kv.Key, qps / QPS_LIMIT, $"{qps:F1}");
			}

      Console.ForegroundColor = ConsoleColor.Green;
      Console.SetCursorPosition(REQUESTS_X, 15); row = 16;
      Console.Write("Results");
      foreach (var kv in Metrics.QPSByResult) {
        var qps = kv.Value.Value;
        DrawProgressBar(REQUESTS_X, row++, 12, kv.Key, qps / QPS_LIMIT, $"{qps:F1}");
      }

      // Navmesh statsi
      const int MAX_MESHES = 25;
      row = TABLE_START_Y + 1;
      Console.SetCursorPosition(MESHES_X, row);
			int num = 0;
			foreach (var n in Navmeshes.Keys.Select(x => new { ID = x, QPS = Metrics.RequestsByZone[x].Value }).OrderByDescending(x => x.QPS).Take(MAX_MESHES)) {
				var queue = Metrics.QueueByZone[n.ID].Value;
				DrawProgressBar(MESHES_X, row, 4, n.ID.ToString(), n.QPS / SHARD_LIMIT, $"{n.QPS:F1}");
				DrawProgressBar(MESHES2_X, row, 4, n.ID.ToString(), queue / QUEUE_LIMIT, $"{queue}");
				row++;
			}
    }
  }
}
