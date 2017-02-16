using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using CuttingEdge.Conditions;
using Ninject.Infrastructure;

namespace Nodes.Impl {
  /// <summary>
  /// Parses command-line flags and applies them to all Flag attributes.
  /// </summary>
  internal sealed class FlagParser {
    /// <summary>
    /// Regex for parsing flags in the "--foo=bar" form
    /// </summary>
    private static readonly Regex FlagParsingRegex = new Regex("(?<key>--[^= ]+)([= ](?<value>[^ ]*))?", RegexOptions.Compiled);

    /// <summary>
    /// True if already initialized
    /// </summary>
    private bool isInitialized;

    /// <summary>
    /// All known flags in the codebase (primarily for command line help)
    /// </summary>
    private readonly Dictionary<string, FlagInstance> knownFlags = new Dictionary<string, FlagInstance>();

    /// <summary>
    /// Flag values parsed from the command line
    /// </summary>
    private readonly Multimap<string, string> flagValues = new Multimap<string, string>();

    /// <summary>
    /// Logger for this class
    /// </summary>
    private readonly ILogger logger;

    public FlagParser(ILogger logger) {
      this.logger = logger;
    }

    internal struct FlagInstance {
      public FlagAttribute Attribute;
      public FieldInfo Field;
      public bool HasValue;
      public object ParsedValue;

      /// <summary>
      /// Returns the parsed flag value for this flag
      /// </summary>
      /// <param name="value"></param>
      /// <returns></returns>
      public object GetParsedValue(string value) {
        if (Field.FieldType == typeof (bool)) {
          // Treat "null" as true as the flag is specified
          return !new[] {"false", "0"}.Any(x => x.Equals(value, StringComparison.InvariantCultureIgnoreCase));
        }
				try {
          return Convert.ChangeType(value, Field.FieldType);
				} catch (FormatException) {
					throw new Exception("Flag value cannot be parsed: '" + value + "'");
				}
      }
    }

    /// <summary>
    /// Parses the specified command line flags. Can be called multiple times.
    /// </summary>
    /// <param name="args"></param>
    public void ParseCommandLineArguments(IEnumerable<string> argse) {
      if (!isInitialized)
        FindFlagAttributes();

			var args = argse.ToArray();
			if (args.Length == 0) return;

      logger.Information("Parsing flags:");
      int flagCount = 0;
			var joinedArgs = args.Aggregate((a,b) => a + " " + b);
			var match = FlagParsingRegex.Match(joinedArgs);
      while (match.Success) {
        string key = match.Groups["key"].Value;
        string value = match.Groups["value"].Value;
        logger.Information("\t{key}={value}", key, value);

        // Store the value
        flagValues.Add(key, value);

        // Show help?
        if (key.Equals("--help", StringComparison.InvariantCultureIgnoreCase)) {
          logger.Information("Known Flags for {0}:", Process.GetCurrentProcess().ProcessName);
          foreach (string var in GenerateHelp(detailed: true)) {
            logger.Information(var);
          }
          Environment.Exit(0);
        }

        Condition.Requires(knownFlags.ContainsKey(key)).IsTrue($"Flag {key} is not a valid flag");
        var flagInstance = knownFlags[key];
        flagInstance.ParsedValue = flagInstance.GetParsedValue(value);
        flagInstance.HasValue = true;
        knownFlags[key] = flagInstance;

        // Set static flags directly
        if (flagInstance.Field.IsStatic) {
          flagInstance.Field.SetValue(null, flagInstance.ParsedValue);
        }
        flagCount++;
				match = match.NextMatch();
      }

      logger.Information("Parsed {0} command line flags", flagCount);
    }

    /// <summary>
    /// Injects flag values into the specified object instance
    /// </summary>
    /// <param name="o"></param>
    public void InjectFlags(object o) {
      Condition.Requires(o).IsNotNull();
      foreach (var field in knownFlags.Values.Where(x => x.Field.DeclaringType == o.GetType() && !x.Field.IsStatic)) {
        Condition.Requires(!field.Attribute.IsRequired || field.HasValue, field.Attribute.FlagName)
          .IsTrue("Required flag is missing");
        if (field.HasValue) {
          field.Field.SetValue(o, field.ParsedValue);
        }
      }
    }

    /// <summary>
    /// Generates a command line help of all known flags
    /// </summary>
    /// <param name="detailed">True if detailed descriptions for all flags should be included</param>
    /// <returns></returns>
    public IEnumerable<string> GenerateHelp(bool detailed = false) {
      foreach (var flag in knownFlags) {
        var attr = flag.Value.Attribute;
        var field = flag.Value.Field;
        yield return string.Format("{2}  {0}  {1}", attr.FlagName.PadRight(50), attr.Description, field.FieldType.Name.PadLeft(8));
      }
    } 

    private void FindFlagAttributes() {
      Condition.Requires(isInitialized, "isInitialized").IsFalse();
      logger.Information("Registering flags:");
      foreach (var type in AppDomain.CurrentDomain.GetAssemblies().Where(asm => !asm.GetName().Name.StartsWith("System")).SelectMany(asm => asm.GetTypes())) {
        foreach (var field in type.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance)) {
          var attr = field.GetCustomAttributes<FlagAttribute>(true).FirstOrDefault();
          if (attr == null) {
            continue;
          }
            
          // Check if this flag already exists
          Condition.Requires(attr.FlagName).StartsWith("--", $"Flag name '{attr.FlagName}' must start with --");
          Condition.Requires(knownFlags.ContainsKey(attr.FlagName))
            .IsFalse(string.Format("Flag with name {0} is already defined", attr.FlagName));
          knownFlags.Add(attr.FlagName, new FlagInstance { Attribute = attr, Field = field});
          logger.Information("\t{0}", attr.FlagName);
        }
      }
      isInitialized = true;
    }
  }
}
