using System;
using Microsoft.Extensions.DependencyInjection;

namespace Impostor.Server.Plugins.Proxies
{
    public record ServiceRegistration(Type Type, ServiceLifetime Lifetime);
}
