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

using Corathing.Contracts.Services;
using Corathing.Organizer.WPF.Extensions;
using Corathing.Organizer.WPF.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace Corathing.Organizer.WPF.Views;

/// <summary>
/// Interaction logic for WorkflowSettingsView.xaml
/// </summary>
public partial class WorkflowSettingsView : Page, INavigationView
{
    public WorkflowSettingsViewModel ViewModel { get; }

    public WorkflowSettingsView()
    {
        InitializeComponent();

        DataContext = ViewModel = App.Current.Services.GetService<WorkflowSettingsViewModel>();
    }

    public void OnPreviewGoback(object? parameter = null)
    {
    }

    public void OnBack(object? parameter = null)
    {
    }

    public void OnForward(object? parameter = null)
    {
    }
}
