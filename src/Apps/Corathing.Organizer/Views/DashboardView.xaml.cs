using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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

        Loaded += (s, e) =>
        {
            IAuthService authService = App.Current.Services.GetService<IAuthService>();
            if (authService != null && authService.UseAuthService)
            {
                //var loginWindow = new BaseWindow();
                //loginWindow.Content = new LoginView();
                //loginWindow.Owner = Window.GetWindow(this);
                //loginWindow.ShowDialog();
                //if (loginWindow.DialogResult == false)
                //{
                //    // System.Shutdown
                //}
            }

            ViewModel.Start(App.Current.Services);
        };
    }
}
