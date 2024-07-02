﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Corathing.Contracts.Attributes;
using Corathing.Contracts.Bases;
using Corathing.Widgets.Basics.Widgets.FileOpeners;

namespace Corathing.Widgets.Basics.Widgets.Emptys;

[EntryCoraWidget(
    contextType: typeof(EmptyWidgetContext),
    name: "Create Empty",
    description: "Provides empty widget",
    menuPath: "Default/Empty",
    menuOrder: 0,
    visibleTitle: false
    )]
public partial class EmptyWidgetContext : WidgetContext
{
    public EmptyWidgetContext(IServiceProvider services, WidgetState state)
        : base(services, state)
    {

    }
}
