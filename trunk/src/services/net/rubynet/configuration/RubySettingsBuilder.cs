﻿using System;
using System.Globalization;
using Nohros.Logging;
using Nohros.Configuration.Builders;

namespace Nohros.Ruby
{
  internal partial class RubySettings
  {
    public class Builder : AbstractConfigurationBuilder<RubySettings>
    {
      #region .ctor
      public Builder() {
        Culture = CultureInfo.CurrentCulture;
        LoggerLevel = LogLevel.Info;
        Prompt = Strings.kDefaultPrompt;
        RunningMode = RunningMode.Service;
        ServiceFolder = Strings.kDefaultServiceFolder;
        SelfHost = false;
        TrackerAddress = string.Empty;
        ReceiverEndpoint = string.Empty;
        SenderEndpoint = string.Empty;
      }
      #endregion

      public Builder SetServiceFolder(string service_folder) {
        ServiceFolder = service_folder;
        return this;
      }

      public Builder SetPrompt(string prompt) {
        Prompt = prompt;
        return this;
      }

      public Builder SetTrackerAddress(string tracker_address) {
        TrackerAddress = tracker_address;
        return this;
      }

      public Builder SetCulture(CultureInfo culture) {
        Culture = culture;
        return this;
      }

      public Builder SetLoggerLevel(LogLevel logger_level) {
        LoggerLevel = logger_level;
        return this;
      }

      public Builder SetRunningMode(RunningMode running_mode) {
        RunningMode = running_mode;
        return this;
      }

      public override RubySettings Build() {
        return new RubySettings(this);
      }

      public Builder SetReceiverEndpoint(string receiver_endpoint) {
        ReceiverEndpoint = receiver_endpoint;
        return this;
      }

      public Builder SetSenderEndpoint(string sender_endpoint) {
        SenderEndpoint = sender_endpoint;
        return this;
      }

      public Builder SetSelfHost(bool value) {
        SelfHost = value;
        return this;
      }

      public bool SelfHost { get; private set; }
      public string ReceiverEndpoint { get; private set; }
      public string SenderEndpoint { get; private set; }
      public string TrackerAddress { get; private set; }
      public string ServiceFolder { get; private set; }
      public string Prompt { get; private set; }
      public CultureInfo Culture { get; private set; }
      public LogLevel LoggerLevel { get; private set; }
      public RunningMode RunningMode { get; private set; }
    }
  }
}