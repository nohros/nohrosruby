using System;

namespace Nohros.Ruby.Weblog
{
  public sealed class Program
  {
    public static void Main(string[] args) {
      AppFactory factory = new AppFactory();
      WeblogSettings settings = factory.CreateSettings();
      Aggregator aggregator = factory.CreateAggergator(settings);
      aggregator.Subscribe("zeus.acao.net", 8156);
      aggregator.Run();
    }
  }
}
