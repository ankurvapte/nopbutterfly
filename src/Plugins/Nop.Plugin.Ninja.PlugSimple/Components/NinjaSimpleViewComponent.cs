
using Microsoft.AspNetCore.Mvc;
using Nop.Web.Framework.Components;

namespace Nop.Plugin.Ninja.PlugSimple.Components
{
    public class NinjaSimpleViewComponent:NopViewComponent
    {
        public async Task<IViewComponentResult> Invoke()
        {
            return View("~/Plugins/Ninja.PlugSimple/Views/NinjaView.cshtml");
        }
    }
}