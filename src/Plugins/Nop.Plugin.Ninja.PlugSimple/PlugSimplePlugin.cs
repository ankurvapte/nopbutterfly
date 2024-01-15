using Nop.Services.Plugins;
using Nop.Services.Cms;

namespace Nop.Plugin.Ninja.PlugSimple;

public class PlugSimplePlugin : BasePlugin, IWidgetPlugin
{   
    public override async Task InstallAsync()
    {
        await base.InstallAsync();
    }

    public override async Task UninstallAsync()
    {
        await base.UninstallAsync();
    }
}
