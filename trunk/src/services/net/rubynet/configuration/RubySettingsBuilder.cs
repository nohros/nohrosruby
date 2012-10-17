using System;
using System.Globalization;
using Nohros.Logging;
using Nohros.Configuration.Builders;

namespace Nohros.Ruby
{
  internal partial class RubySettings
  {
    public class Builder : AbstractConfigurationBuilder<RubySettings>
    {
      CultureInfo culture_;
      LogLevel logger_level_;
      string prompt_;
      RunningMode running_mode_;
      string service_folder_;

      public Builder SetServiceFolder(string service_folder) {
        service_folder_ = service_folder;
        return this;
      }

      public Builder SetPrompt(string prompt) {
        prompt_ = prompt;
        return this;
      }

      public Builder SetCulture(CultureInfo culture) {
        culture_ = culture;
        return this;
      }

      public Builder SetLoggerLevel(LogLevel logger_level) {
        logger_level_ = logger_level;
        return this;
      }

      public Builder SetRunningMode(RunningMode running_mode) {
        running_mode_ = running_mode;
        return this;
      }

      public override RubySettings Build() {
        return new RubySettings(this);
      }

      public string ServiceFolder {
        get { return service_folder_; }
      }

      public string Prompt {
        get { return prompt_; }
      }

      public CultureInfo Culture {
        get { return culture_; }
      }

      public LogLevel LoggerLevel {
        get { return logger_level_; }
      }

      public RunningMode RunningMode {
        get { return running_mode_; }
      }
    }
  }
}
