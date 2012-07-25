﻿using System;
using System.Collections.Generic;
using Nohros.Ruby.Protocol;

namespace Nohros.Ruby
{
  /// <summary>
  /// Provides a skeletal implementation of the <see cref="IRubyService"/>
  /// interface to minimize the effort required to implement this interface.
  /// <para>
  /// To implement a service, the programmer needs to extend this class and
  /// provide an implementation for the methods they want to support and
  /// the <see cref="IRubyService.OnMessage"/> method.
  /// </para>
  /// </summary>
  public abstract class AbstractRubyService : IRubyService
  {
    readonly IDictionary<string, string> facts_;

    #region .ctor
    /// <summary>
    /// Initialize a new instance of the <see cref="AbstractRubyService"/>
    /// class.
    /// </summary>
    /// <remarks>
    /// This constructor initialize the facts dictionary and pupulate it with
    /// the default service facts, which are:
    /// <list type="bullet">
    /// <term>OS Version: "host-os-version"</term>
    /// <term>Current User Name: "host-user-name</term>
    /// <term>Machine Name:"host-machine-name"</term>
    /// <term>.NET CLR Version:"host-clr_version"</term>
    /// </list>
    /// </remarks>
    protected AbstractRubyService() {
      facts_ = new Dictionary<string, string>();
      facts_.Add(StringResources.kOSVersionFact, Environment.OSVersion.ToString());
      facts_.Add(StringResources.kUserNameFact, Environment.UserName);
      facts_.Add(StringResources.kMachineNameFact, Environment.MachineName);
      facts_.Add(StringResources.kCLRVersionFact, Environment.Version.ToString());
    }
    #endregion

    /// <inheritdoc/>
    public virtual void Shutdown() {
    }

    /// <inheritdoc/>
    public virtual void Pause(IRubyMessage message) {
    }

    /// <inheritdoc/>
    public virtual void Continue(IRubyMessage message) {
    }

    /// <inheritdoc/>
    public abstract void Start(IRubyServiceHost service_host);

    /// <inheritdoc/>
    public abstract void Stop(IRubyMessage message);

    /// <inheritdoc/>
    public abstract void OnMessage(IRubyMessage message);

    /// <inheritdoc/>
    public virtual IDictionary<string, string> Facts {
      get { return facts_; }
    }
  }
}