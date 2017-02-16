using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using CuttingEdge.Conditions;
using Ninject;
using Ninject.Modules;

namespace Nodes {
  /// <summary>
  ///   Abstract module
  /// </summary>
  public abstract class AbstractModule : NinjectModule {
    /// <summary>
    ///   Mapping of moduleType -> moduleInstance
    /// </summary>
    private readonly Dictionary<Type, AbstractModule> registeredModules = new Dictionary<Type, AbstractModule>();

    private bool configuringModule;

    /// <summary>
    /// Logger for this module
    /// </summary>
    public ILogger Logger { get; set; }

    /// <summary>
    ///   Configures sub-modules by Install()ing them
    /// </summary>
    protected abstract void ConfigureModules();


    /// <summary>
    ///   Configures Bindings by using the Bind() methods.
    /// </summary>
    protected abstract void ConfigureBindings();

    /// <summary>
    ///   Registers the specified modules
    /// </summary>
    protected void Install(params AbstractModule[] modules) {
      Install(true, modules);
    }

    public sealed override void Load() {
      ConfigureBindings();
    }

    /// <summary>
    ///   Registers the specified module if the specified condition is matched
    /// </summary>
    /// <param name="condition"></param>
    /// <param name="modules"></param>
    protected void Install(bool condition, params AbstractModule[] modules) {
      Condition.Requires(configuringModule, "configuringModules").IsTrue();
      Condition.Requires(modules, "modules").IsNotEmpty();

      if (!condition) {
        return;
      }

      foreach (var module in modules) {
        if (registeredModules.ContainsKey(module.GetType())) {
          Console.WriteLine("Skipping already installed module: "+module+" of type"+module.GetType());
          continue; // Skip already added modules
        }
        registeredModules.Add(module.GetType(), module);
      }
    }

    /// <summary>
    ///   Returns all (recursively) registered modules
    /// </summary>
    /// <returns></returns>
    internal void GetAllRegisteredModules(List<AbstractModule> moduleRegistry) {
      try {
        configuringModule = true;
        ConfigureModules();
      }
      finally {
        configuringModule = false;
      }

      var currentStackFrames = new StackTrace().GetFrames();
      Condition.Requires(currentStackFrames).IsNotNull();
      foreach (var module in registeredModules.Values) {
        if (moduleRegistry.Contains(module) || moduleRegistry.Any(x => x.GetType() == module.GetType())) {
					Console.WriteLine("Skipping module: "+module);
          continue; // Prevent cyclic and duplicate dependencies
				}

        // Load the module and its submodules..
        moduleRegistry.Add(module);
        module.GetAllRegisteredModules(moduleRegistry);
      }
    }
  }
}
