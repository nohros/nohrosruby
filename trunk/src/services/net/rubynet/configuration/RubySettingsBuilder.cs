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
      string ipc_channel_address_;
      LogLevel logger_level_;
      string prompt_;
      RunningMode running_mode_;
      string self_host_address_;
      string service_folder_;
      bool self_host_;

      #region .ctor
      public Builder() {
        culture_ = CultureInfo.CurrentCulture;
        ipc_channel_address_ = string.Empty;
        logger_level_ = LogLevel.Info;
        prompt_ = Strings.kDefaultPrompt;
        running_mode_ = RunningMode.Service;
        self_host_address_ = Strings.kDefaultSelfHostIPCChannelAddress;
        service_folder_ = Strings.kDefaultServiceFolder;
        self_host_ = false;
      }
      #endregion

      public Builder SetSelfHostAddress(string self_host_address) {
        self_host_address_ = self_host_address;
        self_host_ = true;
        return this;
      }

      public bool SelfHost {
        get { return self_host_; }
      }

      public Builder SetIPCChannelAddress(string ipc_channel_address) {
        ipc_channel_address_ = ipc_channel_address;
        return this;
      }

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

      public string SelfHostAddress {
        get { return self_host_address_; }
        internal set { SetSelfHostAddress(value); }
      }

      public string IPCChannelAddress {
        get { return ipc_channel_address_; }
        internal set { SetIPCChannelAddress(ipc_channel_address_); }
      }

      public string ServiceFolder {
        get { return service_folder_; }
        internal set { SetServiceFolder(service_folder_); }
      }

      public string Prompt {
        get { return prompt_; }
        internal set { SetPrompt(prompt_); }
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
