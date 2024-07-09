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

using Microsoft.Extensions.DependencyInjection;

using Corathing.Contracts.Attributes;
using Corathing.Contracts.Bases;
using Corathing.Contracts.Entries;
using Corathing.Contracts.Services;
using Corathing.Widgets.Basics.Widgets.WebPages;

namespace Corathing.Widgets.Basics.Widgets.WebQueries;

[EntryCoraWidget(
    contextType: typeof(WebQueryViewModel),
    name: "Create Web Query",
    description: "Provides a one by one square widget.",
    menuPath: "Default/Web Query",
    menuOrder: 0
    )]
public partial class WebQueryViewModel : WidgetContext
{
    public override void OnCreate(IServiceProvider services, WidgetState state)
    {
        ILocalizationService localizationService = _services.GetService<ILocalizationService>();
        localizationService.Provide("Corathing.Widgets.Basics.WebQueryName", value => WidgetTitle = value);
    }
}

/// <summary>
/// Interaction logic for WebQueryWidget.xaml
/// </summary>
public partial class WebQueryWidget
{
    public WebQueryWidget()
    {
        InitializeComponent();
    }
}
