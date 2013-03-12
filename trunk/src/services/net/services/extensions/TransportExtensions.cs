using System;
using ZMQ;

namespace Nohros.Ruby.Extensions
{
  public static class TransportExtensions
  {
    public static string AsString(this Transport transport) {
      switch (transport) {
        case Transport.TCP:
          return "tcp";
        case Transport.INPROC:
          return "inproc";
        case Transport.IPC:
          return "ipc";
        case Transport.EPGM:
          return "epgm";
        case Transport.PGM:
          return "pgm";
      }
      throw new ArgumentException("transport");
    }
  }
}
