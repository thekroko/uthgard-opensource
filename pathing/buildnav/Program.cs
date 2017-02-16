using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using CEM.Core;
using CEM.Utils;
using CEM.World;
using System.Diagnostics;
using System.Threading.Tasks;

namespace CEM {
  /// <summary>
  /// CEM
  /// </summary>
  internal static class Program {
    /// <summary>
    /// Current Version
    /// </summary>
    public const string VERSION = "v0.2";

    /// <summary>
    /// Command Line Arguments
    /// </summary>
    public static CLIArgs Arguments { get; private set; }

    /// <summary>
    /// Positional Arguments
    /// </summary>
    public static string[] PositionalArguments { get; private set; }

    /// <summary>
    /// Configuration
    /// </summary>
    public static Config Config { get; private set; }

    public static List<string> NifIgnorelist { get; private set; }

    [STAThread]
    public static void Main(string[] args) {
      Application.EnableVisualStyles();
      Application.SetCompatibleTextRenderingDefault(true);

      // Command & CVar core
      Core.Core.Init();

      // Parse Args
      Arguments = new CLIArgs();
      PositionalArguments = CommandLineArgs.ParseArguments(Arguments, Environment.GetCommandLineArgs().Skip(1).ToArray());
      if (Arguments.Help)
        return;

      if (!Arguments.NormalPriority)
        Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.BelowNormal;

      // Load the config file
      if (!string.IsNullOrEmpty(Arguments.DaocRoot)) {
        Config = new Config {CEM = {GamePath = Arguments.DaocRoot}};
      } else {
        string cfgFile = Path.Combine("..", Arguments.ConfigFile);
        if (!File.Exists(cfgFile)) {
          Log.Fatal("Config file not found: {0}", Arguments.ConfigFile);
          return;
        }
        Config = Config.Load(cfgFile);
      }

      // Load nif whitelist
      string ignorelistFile = Path.Combine("..", Arguments.IgnoreList);
      if (!File.Exists(ignorelistFile)) {
        Log.Warn("NIF ignorelist file not found: {0}", Arguments.IgnoreList);
        NifIgnorelist = new List<string>();
      } else {
        NifIgnorelist = File.ReadAllLines(ignorelistFile).ToList();
      }

      BuildNavmeshes();
      Log.Normal("---------------------------------------------------------------------------");
      Log.Normal("All done.");
      Console.ReadKey();
    }

    private static IEnumerable<Zone2> GetZonesToBuild()
    {
      // Zones
      foreach (var zone in Arguments.RecastBuildZoneID.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
        yield return WorldMgr.GetZone(int.Parse(zone));
      // Regions
      foreach (var region in Arguments.RecastBuildRegionID.Split(new char[] {','}, StringSplitOptions.RemoveEmptyEntries).Select(r=>WorldMgr.GetRegion(int.Parse(r))).Where(r=>r != null)) {
        foreach (var zone in region.Values)
          yield return zone;
      }
      // All?
      if (Arguments.RecastBuildAll)
      {
        foreach (var zone in WorldMgr.GetAllRegions().SelectMany(r => r.Values))
          yield return zone;
      }
    }

    private static void BuildNavmeshes() {
      WorldMgr.Init();
      Log.Normal("-----------------------------------------------------------------------------");
      Log.Normal("Building recast navmeshes!");

      var zones = GetZonesToBuild().Distinct().Shuffle().ToArray();
      Log.Normal("We will be creating {0} zones today. Right here. Right now.", zones.Length);
      Log.Normal("-----------------------------------------------------------------------------");
      Log.Normal("");
      Console.Title = "NavGen";
      int finishedZones = 0;
      var po = new ParallelOptions() { MaxDegreeOfParallelism = 6 };
      Parallel.Invoke(po, zones.Select(z => new Action(() => {
#if !DEBUG
        try
        {
#endif
          NavmeshMgr.BuildNavMesh(z);
#if !DEBUG
        }
        catch (Exception ex)
        {
          Log.Error(ex);
        }
#endif
        int finished = Interlocked.Increment(ref finishedZones);
        Console.Title = String.Format("[{2}%] NavGen {0}/{1}", finished, zones.Length, finished * 100 / zones.Length);
      })).ToArray());
    }

    /// <summary>
    /// Command Line Arguments
    /// </summary>
    internal class CLIArgs {
      /// <summary>
      /// Default arguments
      /// </summary>
      public CLIArgs() {
        ConfigFile = "cem.json";
        IgnoreList = "ignorelist.txt";
        RecastBuildRegionID = "";
        RecastBuildZoneID = "";
        ExportObjOnly = true;
        RecastBuildAll = false;
        RecastBuildRegionID = // classic and SI
          "1,10,20,21,22,23,24,25,50,51,60,61,62,100,101,125,126,127,128,129,150,151,160,161,180,181,190,191,200,201,220,221,222,223,224,246,248,249,250,251,252,253,269,270,271,276,277";
        NormalPriority = false;
      }

      [Argument("config", Description = "Configuration File to use")]
      public string ConfigFile { get; set; }

      [Argument("daoc", Description = "DAoC Root Dir to use")]
      public string DaocRoot { get; set; }

      [Argument("ignorelist", Description = "NIF ignorelist file to use")]
      public string IgnoreList { get; set; }

      [Argument("help", Description = "Shows this help")]
      public bool Help { get; set; }

      [Argument("regions", Description = "Regions to build (recast navmesh), CSV")]
      public string RecastBuildRegionID { get; set; }

      [Argument("zones", Description = "Zones to build (recast navmesh), CSV")]
      public string RecastBuildZoneID { get; set; }

      [Argument("all", Description = "Build All Regions")]
      public bool RecastBuildAll { get; set; }

      [Argument("obj", Description = "Export obj only")]
      public bool ExportObjOnly { get; set; }

      [Argument("normal-priority", Description = "Run at normal priority")]
      public bool NormalPriority { get; set; }
    }
  }
}