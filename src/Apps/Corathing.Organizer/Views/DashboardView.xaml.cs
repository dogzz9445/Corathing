using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Corathing.Contracts.Bases;
using Corathing.Contracts.Services;
using Corathing.Dashboards.WPF.Controls;
using Corathing.Organizer.Controls;
using Corathing.Organizer.Extensions;
using Corathing.Organizer.Models;
using Corathing.Organizer.ViewModels;

using Microsoft.Extensions.DependencyInjection;

namespace Corathing.Organizer.Views;

/// <summary>
/// Provides properties detailing the validity of a dashboard name
/// </summary>
public class DashboardNameValidResponse
{
    #region Public Properties

    /// <summary>
    /// Gets the invalid reason.
    /// </summary>
    /// <value>The invalid reason.</value>
    public string InvalidReason { get; }

    /// <summary>
    /// Gets a value indicating whether this <see cref="DashboardNameValidResponse"/> is valid.
    /// </summary>
    /// <value><c>true</c> if valid; otherwise, <c>false</c>.</value>
    public bool Valid { get; }

    #endregion Public Properties

    #region Public Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="DashboardNameValidResponse"/> class.
    /// </summary>
    /// <param name="valid">if set to <c>true</c> [valid].</param>
    /// <param name="invalidReason">The invalid reason.</param>
    public DashboardNameValidResponse(bool valid, string invalidReason = null)
    {
        Valid = valid;
        InvalidReason = invalidReason;
    }

    #endregion Public Constructors
}

/// <summary>
/// Interface IDashboardConfigurationHandler
/// </summary>
public interface IDashboardConfigurationHandler
{
    #region Public Methods

    /// <summary>
    /// Complete the dashboard configuration.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <param name="save">if set to <c>true</c> [save].</param>
    /// <param name="newName">The new name.</param>
    void DashboardConfigurationComplete(DashboardConfigurationType type, bool save, string newName);

    /// <summary>
    /// Validate dashboard name.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <returns>DashboardNameValidResponse.</returns>
    DashboardNameValidResponse DashboardNameValid(string name);

    #endregion Public Methods
}

/// <summary>
/// DashboardView.xaml에 대한 상호 작용 논리
/// </summary>
public partial class DashboardView : UserControl
{
    public DashboardsViewModel ViewModel;

    public DashboardView()
    {
        InitializeComponent();

        DataContext = ViewModel = new DashboardsViewModel() { EditMode = false };

        //DashboardHostTabControl.SelectionChanged += DashboardHostTabControl_SelectionChanged;

        Loaded += (s, e) =>
        {
            ViewModel.Start(App.Current.Services);
        };
    }

    private void DashboardHostTabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (e.Source is TabControl)
        {
            var pos = DashboardHostTabControl.SelectedIndex;
            if (pos != 0 && pos == ViewModel.SelectedProject.Workflows.Count - 1) //last tab
            {
                var tab = ViewModel.SelectedProject.Workflows.Last();
                ConvertPlusToNewTab(tab);
                AddNewPlusButton();
            }
        }
    }

    void ConvertPlusToNewTab(WorkflowContext tab)
    {
        //Do things to make it a new tab.
        TabIndex++;
        tab.Title = $"Tab {TabIndex}";
        tab.IsPlaceholder = false;
        //tab.Content = new ContentVM("Tab content", TabIndex);
    }

    void AddNewPlusButton()
    {
        var plusTab = new WorkflowContext()
        {
            Title = "+",
            IsPlaceholder = true
        };
        ViewModel.SelectedProject.Workflows.Add(plusTab);
    }


    class ContentVM
    {
        public ContentVM(string name, int index)
        {
            Name = name;
            Index = index;
        }
        public string Name { get; set; }
        public int Index { get; set; }
    }

    private void OnTabCloseClick(object sender, RoutedEventArgs e)
    {
        var tab = (sender as Button).DataContext as WorkflowContext;
        if (ViewModel.SelectedProject.Workflows.Count > 2)
        {
            var index = ViewModel.SelectedProject.Workflows.IndexOf(tab);
            if (index == ViewModel.SelectedProject.Workflows.Count - 2)//last tab before [+]
            {
                DashboardHostTabControl.SelectedIndex--;
            }
            ViewModel.SelectedProject.Workflows.RemoveAt(index);
        }
    }
}
