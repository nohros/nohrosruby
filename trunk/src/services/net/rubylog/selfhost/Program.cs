using System;

namespace Nohros.Ruby.Logging
{
  public sealed class Program
  {
    public static void Main(string[] args) {
      var factory = new AggregatorFactory().CreateService(string.Empty);
      factory.Start(new NopRubyServiceHost());
    }
  }
}
