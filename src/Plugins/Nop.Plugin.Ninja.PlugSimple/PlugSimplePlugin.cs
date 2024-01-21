using Nop.Services.Plugins;
using Nop.Services.Cms;

namespace Nop.Plugin.Ninja.PlugSimple;

public class PlugSimplePlugin : BasePlugin, IWidgetPlugin
{
    public bool HideInWidgetList { get; }
    public override async Task InstallAsync()
    {
        await base.InstallAsync();
    }

    public override async Task UninstallAsync()
    {
        await base.UninstallAsync();
    }

    public Task<IList<string>> GetWidgetZonesAsync()
    {
        return null;
    }

    public Type GetWidgetViewComponent(string widgetZone)
    {
        return null;
    }
}
