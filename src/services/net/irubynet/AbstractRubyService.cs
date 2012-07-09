using System;
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
    #region .ctor
    /// <summary>
    /// Initialize a new instance of the <see cref="AbstractRubyService"/>
    /// class by using the specified service name.
    /// </summary>
    protected AbstractRubyService() {
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
    public abstract IDictionary<string, string> Facts { get; }
  }
}
