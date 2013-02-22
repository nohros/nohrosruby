using System;
using Nohros.MyToolsPack.Console;

namespace Nohros.Ruby
{
  internal partial class RubySettings
  {
    public string Prompt { get; private set; }
    string IConsoleSettings.Prompt { get { return Prompt; } }
  }
}
