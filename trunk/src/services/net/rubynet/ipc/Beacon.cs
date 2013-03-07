using System;
using System.Net;
using Google.ProtocolBuffers;

namespace Nohros.Ruby
{
  /// <summary>
  /// Represents a beacon that identifies a peer.
  /// </summary>
  internal class Beacon
  {
    #region .ctor
    /// <summary>
    /// Initializes a new instance of the <see cref="Beacon"/> class using the
    /// specified peer id and endpoint address.
    /// </summary>
    /// <param name="peer_id">
    /// A <see cref="Guid"/> that uniquely identifies a peer.
    /// </param>
    /// <param name="peer_endpoint">
    /// A <see cref="IPEndPoint"/> containing the address of the peer.
    /// </param>
    public Beacon(Guid peer_id, IPEndPoint peer_endpoint) {
      PeerID = peer_id;
      PeerEndpoint = peer_endpoint;
    }
    #endregion

    /// <summary>
    /// Decodes the bytes in the specified byte array into a
    /// <see cref="Beacon"/>.
    /// </summary>
    /// <param name="bytes">
    /// The byte array vontaining the sequence of bytes to decode.
    /// </param>
    /// <param name="peer_address">
    /// The IP address of the peer associated with the beacon.
    /// </param>
    /// <exception cref="ArgumentException">
    /// The byte array does not represents a valid beacon.
    /// </exception>
    public static Beacon FromByteArray(byte[] bytes, IPAddress peer_address) {
      if (bytes.Length > 3 && bytes[0] == 'R' && bytes[1] == 'B' &&
        bytes[2] == 'Y') {
        try {
          BeaconMessage beacon = BeaconMessage
            .ParseFrom(ByteString.CopyFrom(bytes, 3, bytes.Length - 3));
          var id = new Guid(beacon.PeerId.ToByteArray());
          var endpoint = new IPEndPoint(peer_address,
            beacon.PeerMailboxPort);
          return new Beacon(id, endpoint);
        } catch (Exception e) {
          throw new FormatException(Resources.Format_Beacon_FromBytes, e);
        }
      }
      throw new FormatException(Resources.Format_Beacon_FromBytes);
    }

    /// <summary>
    /// Gets a <see cref="Guid"/> that uniquely identifies a peers.
    /// </summary>
    public Guid PeerID { get; private set; }

    /// <summary>
    /// Gets the peer's address.
    /// </summary>
    public IPEndPoint PeerEndpoint { get; private set; }
  }
}
