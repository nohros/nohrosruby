using System;
using System.IO;
using System.Reflection;

namespace Nohros.Ruby.Logging
{
  public class SettingsFactory
  {
    public Settings CreateSettings() {
      string assembly_location = Assembly.GetExecutingAssembly().Location;
      string config_file_path =
        Path.Combine(Path.GetDirectoryName(assembly_location),
          Strings.kConfigFileName);

      Settings settings = new Settings();
      settings.Load(config_file_path, Strings.kRootFileName);
      return settings;
    }
  }
}
