using System.Collections.Generic;

namespace Impostor.Api.Plugins
{
    public interface IPluginManager
    {
        IReadOnlyList<IPluginInformation> Plugins { get; }
    }
}
