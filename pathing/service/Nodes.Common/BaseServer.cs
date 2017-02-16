using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using CuttingEdge.Conditions;
using Ninject;
using Nodes.Impl;
using Nodes.Tasks;
using Serilog;

namespace Nodes {
  /// <summary>
  ///   Basis for an RPC server
  /// </summary>
  public abstract class BaseServer : AbstractModule {
    /// <summary>
    ///   The kernel used for all injections
    /// </summary>
    protected new IKernel Kernel { get; private set; }

    /// <summary>
    ///   Global, central logging instance from which all loggers are derived
    /// </summary>
    private ILogger GlobalLogger { get; set; }

    /// <summary>
    /// Flag parser used for parsing command line flags
    /// </summary>
    private FlagParser FlagParser { get; set; }

    /// <summary>
    ///   Context logger for this class
    /// </summary>
    protected ILogger Log { get; private set; }

    private ILogger ConfigureGlobalLogger() {
      return new SerilogLoggerProxy(
        new LoggerConfiguration()
          .WriteTo.ColoredConsole()
          .CreateLogger());
    }

    protected override sealed void ConfigureModules() {
      Install(new TaskHealthCheckerTask());
      Configure();
    }

    protected override sealed void ConfigureBindings() {
      Kernel.Bind<ILogger>().ToMethod(context => GlobalLogger);
      //Kernel.Bind(typeof(object), typeof(string), typeof(int), typeof(uint), typeof(bool), typeof(long),
      //  typeof(ulong), typeof(char), typeof(ushort), typeof(short))
      //  .ToConstant("test")
      //  .WhenMemberHas<FlagAttribute>();
    }

    /// <summary>
    ///   Configures this server by Install()ing modules
    /// </summary>
    protected abstract void Configure();

    /// <summary>
    ///   Runs this server
    /// </summary>
    public void Run() {
      Condition.Requires(Kernel, "kernel").IsNull("Expected server to not be configured yet");

      // Install logging
      GlobalLogger = ConfigureGlobalLogger();
      Condition.Requires(GlobalLogger, "GlobalLogger").IsNotNull();
      AppDomain.CurrentDomain.UnhandledException +=
        (obj, args) => GlobalLogger.Error(args.ExceptionObject as Exception, "Unhandled Exception");
      Log = GlobalLogger.ForContext(GetType());

      // Load all modules
      Log.Information("Assembling module list");
      var modules = new List<AbstractModule>();
      GetAllRegisteredModules(modules);

      // Parse Flags, and set static flags before initializing any module. Has to happen
      // after the module list is generated so that all required assemblies are loaded.
      FlagParser = new FlagParser(Log.ForContext<FlagParser>());
      FlagParser.ParseCommandLineArguments(Environment.GetCommandLineArgs().Skip(1));

      // Initialize every module
      Log.Information("Registering Modules:");
      foreach (var module in modules) {
        Log.Information("  {module}", module);

        // Pre-inject the logger so that it is always available (even before the kernel is initialized)
        module.Logger = GlobalLogger.ForContext(module.GetType());
        
        // Same goes for flags
        FlagParser.InjectFlags(module);
      }

      // Initialize Kernel
      Log.Information("Initializing Injection Kernel");
      Kernel = new StandardKernel(modules.ToArray());
      Kernel.Settings.InjectNonPublic = true;
      ConfigureBindings();

      // Initialize modules
      Log.Information("Injecting module members");
      foreach (var module in modules) {

        // Lazily initialize module injections
        Kernel.Inject(module);
      }

      // Run all server tasks
      var taskModules = modules.OfType<AbstractTaskModule>().ToArray();
      Log.Information("==============================================================");
      Log.Information("Starting Tasks: {taskModules}", taskModules);
      var tasks = taskModules.Select(x => Task.Run(async () => {
        try {
          await x.Run();
        } catch (Exception ex) {
          Log.Error(ex, "Task execution failed");
        }
      })).ToArray();

      // Wait for all tasks to finish
      Task.WaitAll(tasks);
      Log.Information("All tasks have finished execution");
    }
  }
}