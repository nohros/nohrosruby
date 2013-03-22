using System;

namespace Nohros.Ruby
{
  /// <summary>
  /// Common resource like strings.
  /// </summary>
  public class RubyStrings
  {
    /// <summary>
    /// Machine user name fact.
    /// </summary>
    public const string kUserNameFact = "host-user-name";

    /// <summary>
    /// CLR version fact.
    /// </summary>
    public const string kCLRVersionFact = "host-clr-version";

    /// <summary>
    /// OS version fact.
    /// </summary>
    public const string kOSVersionFact = "host-os-version";

    /// <summary>
    /// Machine name fact.
    /// </summary>
    public const string kMachineNameFact = "host-name";

    /// <summary>
    /// Service name fact.
    /// </summary>
    public const string kServiceNameFact = "service";

    /// <summary>
    /// Message ID fact.
    /// </summary>
    public const string kMessageIDFact = "mssgid";

    /// <summary>
    /// The key of the endpoint value that is send in response to a service
    /// query.
    /// </summary>
    public const string kQueryResponseEndpointKey = "endpoint";
  }
}
