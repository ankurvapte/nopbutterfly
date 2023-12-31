﻿using System;
using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Cms
{
    public partial record RenderWidgetModel : BaseNopModel
    {
        public Type WidgetViewComponent { get; set; }
        public object WidgetViewComponentArguments { get; set; }
    }
}