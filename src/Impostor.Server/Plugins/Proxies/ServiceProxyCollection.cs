using System.Collections.Generic;

namespace Impostor.Server.Plugins.Proxies
{
    public class ServiceProxyCollection
    {
        public ServiceProxyCollection(IReadOnlyList<ServiceRegistration> serviceTypes)
        {
            ServiceTypes = serviceTypes;
        }

        public IReadOnlyList<ServiceRegistration> ServiceTypes { get; }
    }
}
