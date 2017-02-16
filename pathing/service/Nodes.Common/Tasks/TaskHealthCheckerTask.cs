using System;
using System.Linq;
using System.Threading.Tasks;
using Ninject;

namespace Nodes.Tasks
{
  /// <summary>
  /// Module which makes sure that all tasks become healthy
  /// </summary>
  public sealed class TaskHealthCheckerTask : AbstractTaskModule {
    //[Flag("--task_healthy_timeout", "Timeout in seconds after which the server startup will fail if not all tasks have become healthy")]
    public static int TaskHealthyTimeout = -1;

    protected override void ConfigureModules() {
    }

    public override async Task Run() {
      var unhealthyTasks = Kernel.GetAll<AbstractTaskModule>().ToList();

      var elapsed = TimeSpan.Zero;
      int currentWait = 1;
      while (TaskHealthyTimeout < 0 || elapsed < TimeSpan.FromSeconds(TaskHealthyTimeout)) {
        // Wait some time
        await Task.Delay(TimeSpan.FromSeconds(currentWait));
        elapsed += TimeSpan.FromSeconds(currentWait);
        currentWait++;

        // Check for tasks becoming healthy
        var healthyTasks = unhealthyTasks.Where(x => x.IsHealthy).ToArray();
        foreach (var healthy in healthyTasks) {
          Logger.Information("{$healthy} is now HEALTHY", healthy);
          unhealthyTasks.Remove(healthy);
        }

        if (unhealthyTasks.Count == 0)
          break;

        Logger.Information("Waiting for UNHEALTHY tasks: {unhealthyTasks}", unhealthyTasks);
      }

      // Check if all service are healthy
      if (unhealthyTasks.Any()) {
        Logger.Fatal("Tasks did not become healthy in time: {unhealthyTasks}", unhealthyTasks);
        throw new InvalidOperationException("Server did not become healthy");
      }
      Logger.Information("All tasks have become healthy");
    }

    public override bool IsHealthy => true;
  }
}
