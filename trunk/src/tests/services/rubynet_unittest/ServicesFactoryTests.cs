using System;
using NUnit.Framework;
using Nohros.Ruby.Protocol.Control;

namespace Nohros.Ruby.Tests
{
  [TestFixture]
  public class ServicesFactoryTests
  {
    ServiceControlMessage.Builder GetServiceControlMessageBuilder() {
      return new ServiceControlMessage.Builder()
        .SetService("RunAndStopService")
        .AddRangeArguments(
          new[]
          {
            new KeyValuePair.Builder()
              .SetKey("switches")
              .SetValue("-debug")
              .Build(),
            new KeyValuePair.Builder()
              .SetKey("assembly")
              .SetValue("nohros.ruby.tests")
              .Build()
          });
    }

    [Test]
    public void ShouldCreateService() {
      var factory = new ServicesFactory();
      var service_control_message =
        GetServiceControlMessageBuilder()
          .AddArguments(
            new KeyValuePair.Builder()
              .SetKey("type")
              .SetValue(
                "Nohros.Ruby.Tests.RunAndStopService, nohros.ruby.net.tests")
              .Build())
          .Build();

      Assert.DoesNotThrow(delegate
      {
        var service = factory.CreateService(service_control_message);
        Assert.IsAssignableFrom<RunAndStopService>(service);
      });
    }
  }
}
