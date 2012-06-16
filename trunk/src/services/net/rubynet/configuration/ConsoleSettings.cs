using System;
using Nohros.MyToolsPack.Console;

namespace Nohros.Ruby
{
  internal partial class RubySettings
  {
    string IConsoleSettings.Prompt {
      get { return prompt_; }
    }
  }
}
