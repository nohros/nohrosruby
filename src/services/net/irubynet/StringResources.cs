using System;

namespace Nohros.Ruby
{
  /// <summary>
  /// Common resource like strings.
  /// </summary>
  public class StringResources
  {
    // Common message facts -------------------------------------------------
    public const string kUserNameFact = "host-user-name";
    public const string kCLRVersionFact = "host-clr-version";
    public const string kOSVersionFact = "host-os-version";
    public const string kMachineNameFact = "host-name";

    // Control message tokens ------------------------------------------------
    public const string kAnnounceMessageToken = "node-announce";
    public const string kNodeQueryToken = "node-query";
    public const string kNodeResponseToken = "node-response";
    public const string kServiceNameFact = "service";
    public const string kMessageUUIDFact = "msguuid";
  }
}
