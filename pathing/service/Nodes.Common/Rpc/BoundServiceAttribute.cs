using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nodes.Rpc
{
  /// <summary>
  /// Annotates a string that represents the service name of a bound service
  /// </summary>
  [AttributeUsage(AttributeTargets.Property)]
  class BoundServiceAttribute : Attribute
  {
  }
}
