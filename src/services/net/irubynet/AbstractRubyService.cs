using System;
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
    readonly string name_;

    #region .ctor
    /// <summary>
    /// Initialize a new instance of tje <see cref="AbstractRubyService"/>
    /// class by using the specified service name.
    /// </summary>
    /// <param name="name">
    /// The name of the service.
    /// </param>
    protected AbstractRubyService(string name) {
      name_ = name;
    }
    #endregion

    /// <inheritdoc/>
    public virtual string Name {
      get { return name_; }
    }

    /// <inheritdoc/>
    public virtual void Start(IRubyServiceHost service_host) {
    }

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
    public virtual void Stop(IRubyMessage message) {
    }

    public abstract IRubyMessage OnMessage(IRubyMessage message);
  }
}
