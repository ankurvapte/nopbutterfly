using Nop.Services.Plugins;
using Nop.Services.Cms;
using Nop.Web.Framework.Infrastructure;
using Nop.Plugin.Ninja.PlugSimple.Components;

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
        return Task.FromResult<IList<string>>(new List<string> { PublicWidgetZones.HomepageBeforeNews });
    }

    public Type GetWidgetViewComponent(string widgetZone)
    {
        return typeof(NinjaSimpleViewComponent);
    }
}
