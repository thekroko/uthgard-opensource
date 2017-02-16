using System;
using CuttingEdge.Conditions;

namespace Nodes
{
  /// <summary>
  /// Represents a command-line flag
  /// </summary>
  [AttributeUsage(AttributeTargets.Field)]
  public class FlagAttribute : Attribute {
    /// <summary>
    /// Name of this flag, used for specifying it on the command line. Has to be prefixed with "--"
    /// </summary>
    public readonly string FlagName;
    
    /// <summary>
    /// Description of what this flag does.
    /// </summary>
    public readonly string Description;

    /// <summary>
    /// True if this flag is required
    /// </summary>
    public bool IsRequired { get; set; }

    /// <summary>
    /// Specifies a Command-Line flag
    /// </summary>
    /// <param name="flagName">(Name used for specifiying on the command line, i.e. "--foobar" can be set by using "--foobar=baz"</param>
    /// <param name="description">Description used when displaying the command line help</param>
    public FlagAttribute(string flagName, string description) {
      Condition.Requires(flagName, "flagName").IsNotNullOrWhiteSpace();
      Condition.Requires(flagName.Contains(" ")).IsFalse("Flag Name cannot contain white-space");
      Condition.Requires(flagName.StartsWith("--")).IsTrue("Flag Name must start with '--'");
      Condition.Requires(flagName.ToLowerInvariant().Equals(flagName)).IsTrue("Flag Name must be all lower case");
      Condition.Requires(description, "description").IsNotNullOrWhiteSpace();

      FlagName = flagName;
      Description = description;
    }
  }
}
