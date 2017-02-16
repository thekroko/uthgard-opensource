using System.Threading.Tasks;

namespace Nodes.Tasks
{
  /// <summary>
  /// Represents a module that implements a runnable server task
  /// </summary>
  public abstract class AbstractTaskModule : AbstractModule {
    /// <summary>
    ///  Called when this task module is run
    /// </summary>
    public abstract Task Run();

    /// <summary>
    /// Should return when this task is in a ready/serving state, and no longer starting up
    /// </summary>
    public abstract bool IsHealthy { get; }

    protected override void ConfigureBindings() {
      Bind<AbstractTaskModule>().ToConstant(this);
    }
  }
}
