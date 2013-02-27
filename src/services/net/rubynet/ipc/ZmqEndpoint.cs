using System;
using System.Net;
using Nohros.Ruby.Extensions;
using ZMQ;

namespace Nohros.Ruby
{
  public class ZMQEndPoint
  {
    /// <summary>
    /// The minimum port number that can be used for dynamic binding as defined
    /// by IANA.
    /// </summary>
    public const int kMinEphemeralPort = 49152;

    /// <summary>
    /// The maximum port number that can be used for dynamic binding as defined
    /// by IANA.
    /// </summary>
    public const int kMaxEphemeralPort = 65535;

    readonly string address_;
    readonly string endpoint_;
    readonly int port_;
    readonly Transport transport_;

    #region .ctor
    /// <summary>
    /// Initializes a new instance of the <see cref="ZMQEndPoint"/> class
    /// using the specified <see cref="IPEndPoint"/> and transport protocol.
    /// </summary>
    /// <param name="endpoint">
    /// The endpoint's address.
    /// </param>
    /// <param name="transport">
    /// The transport to use.
    /// </param>
    public ZMQEndPoint(IPEndPoint endpoint, Transport transport)
      : this(endpoint.Address.ToString(), endpoint.Port, transport) {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ZMQEndPoint"/> class
    /// using the specified address, port and transport protocol.
    /// </summary>
    /// <param name="address">
    /// The endpoint's address.
    /// </param>
    /// <param name="port">
    /// The endpoint's port.
    /// </param>
    /// <param name="transport">
    /// The transport to use.
    /// </param>
    public ZMQEndPoint(string address, int port, Transport transport) {
      address_ = address;
      port_ = port;
      transport_ = transport;
      endpoint_ = transport.AsString() + "://" + address_ + ":" + port_;
    }

    public ZMQEndPoint(string endpoint) {
      int index = endpoint.IndexOf("://");
      int index2 = endpoint.IndexOf(":", index + 3);
      if (index == -1 || index2 == -1) {
        throw new ArgumentException("endpoint");
      }
      string transport = endpoint.Substring(0, index);
      switch (transport) {
        case "tcp":
          transport_ = Transport.TCP;
          break;
        case "ipc":
          transport_ = Transport.IPC;
          break;
        case "epgm":
          transport_ = Transport.EPGM;
          break;
        case "inproc":
          transport_ = Transport.INPROC;
          break;
        case "pgm":
          transport_ = Transport.PGM;
          break;
        default:
          throw new ArgumentException("endpoint");
      }

      address_ = endpoint.Substring(index + 3, index2 - index - 3);
      if (!int.TryParse(endpoint.Substring(index2 + 1), out port_)) {
        throw new ArgumentException("exception");
      }
      endpoint_ = endpoint;
    }
    #endregion

    /// <summary>
    /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
    /// </summary>
    /// <returns>
    /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
    /// </returns>
    /// <filterpriority>2</filterpriority>
    public override string ToString() {
      return Endpoint;
    }

    /// <summary>
    /// Determines whether the specified <see cref="Object"/> is equal to the
    /// current <see cref="Object"/>.
    /// </summary>
    /// <returns>
    /// <c>true</c> if the specified <see cref=".Object"/> is equal to the current
    /// <see cref=".Object"/>; otherwise, <c>false.</c>
    /// </returns>
    /// <param name="obj">The <see cref="Object"/> to compare with the
    /// current <see cref="Object"/>.
    /// </param>
    public override bool Equals(object obj) {
      if (obj == null) {
        return false;
      }
      ZMQEndPoint endpoint = obj as ZMQEndPoint;
      if ((object) endpoint == null) {
        return false;
      }
      return endpoint.Endpoint == Endpoint;
    }

    public static bool operator ==(ZMQEndPoint a, ZMQEndPoint b) {
      if (ReferenceEquals(a, b)) {
        return true;
      }

      if (((object) a == null) || ((object) b == null)) {
        return false;
      }
      return a.Endpoint == b.Endpoint;
    }

    public static bool operator ==(ZMQEndPoint a, IPEndPoint b) {
      if ((((object) a == null) || ((object) b == null)) &&
        ReferenceEquals(a, null)) {
        return false;
      }
      return a.Address == b.Address.ToString() && a.Port == b.Port;
    }

    public static bool operator !=(ZMQEndPoint a, IPEndPoint b) {
      return !(a == b);
    }

    public static bool operator !=(ZMQEndPoint a, ZMQEndPoint b) {
      return !(a == b);
    }

    /// <summary>
    /// Serves as a hash function for a particular type. 
    /// </summary>
    /// <returns>
    /// A hash code for the current <see cref="T:System.Object"/>.
    /// </returns>
    public override int GetHashCode() {
      return Endpoint.GetHashCode();
    }

    /// <summary>
    /// Gets the endpoint.
    /// </summary>
    public string Endpoint {
      get { return endpoint_; }
    }

    /// <summary>
    /// Gets the endpoint's address part.
    /// </summary>
    public string Address {
      get { return address_; }
    }

    /// <summary>
    /// Gets the endpoin's port part.
    /// </summary>
    public int Port {
      get { return port_; }
    }

    /// <summary>
    /// Gets the endpoint's transport part.
    /// </summary>
    public Transport Transport {
      get { return transport_; }
    }
  }
}
