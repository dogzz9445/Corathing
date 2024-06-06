﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using Corathing.Contracts.Attributes;
using Corathing.Contracts.Bases;
using Corathing.Widgets.Basics.Widgets.Commanders;

namespace Corathing.Widgets.Basics.Widgets.LinkOpeners;

[WidgetContextEntry(
    name: "Create Linke Opener",
    description: "Provides a one by one square widget.",
    menuPath: "Default/Link Opener",
    menuOrder: 0,
    targetType: typeof(LinkOpenerWidgetViewModel)
    )]
public partial class LinkOpenerWidgetViewModel : WidgetContext
{
    /// <summary>
    /// Initializes a new instance of the <see cref="OneByOneViewModel"/> class.
    /// </summary>
    public LinkOpenerWidgetViewModel(IServiceProvider services) : base(services)
    {

        WidgetTitle = $"Commander";
    }
}

/// <summary>
/// Interaction logic for LinkOpenerWidget.xaml
/// </summary>
public partial class LinkOpenerWidget
{
    public LinkOpenerWidget()
    {
        InitializeComponent();
    }
}
