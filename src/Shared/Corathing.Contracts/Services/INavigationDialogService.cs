using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and WPF UI Contributors.
// All Rights Reserved.

namespace Corathing.Contracts.Services;

/// <summary>
/// Represents a contract with a <see cref="System.Windows.FrameworkElement"/> that contains <see cref="INavigationView"/>.
/// Through defined <see cref="IPageService"/> service allows you to use the Dependency Injection pattern in <c>WPF UI</c> navigation.
/// </summary>
public interface INavigationDialogService
{ 
    ///// <summary>
    ///// Lets you navigate to the selected page based on it's type. Should be used with <see cref="IPageService"/>.
    ///// </summary>
    ///// <param name="pageType"><see langword="Type"/> of the page.</param>
    ///// <returns><see langword="true"/> if the operation succeeds. <see langword="false"/> otherwise.</returns>
    //Task<bool> Navigate(Type pageType, CancellationToken cancellationToken = default);

    ///// <summary>
    ///// Lets you navigate to the selected page based on it's type, Should be used with <see cref="IPageService"/>.
    ///// </summary>
    ///// <param name="pageType"><see langword="Type"/> of the page.</param>
    ///// <param name="dataContext">DataContext <see cref="object"/></param>
    ///// <returns><see langword="true"/> if the operation succeeds. <see langword="false"/> otherwise.</returns>
    //Task<bool> Navigate(Type pageType, object? dataContext, CancellationToken cancellationToken = default);

    ///// <summary>
    ///// Lets you navigate to the selected page based on it's tag. Should be used with <see cref="IPageService"/>.
    ///// </summary>
    ///// <param name="pageIdOrTargetTag">Id or tag of the page.</param>
    ///// <returns><see langword="true"/> if the operation succeeds. <see langword="false"/> otherwise.</returns>
    //Task<bool> Navigate(string pageIdOrTargetTag, CancellationToken cancellationToken = default);

    ///// <summary>
    ///// Lets you navigate to the selected page based on it's tag. Should be used with <see cref="IPageService"/>.
    ///// </summary>
    ///// <param name="pageIdOrTargetTag">Id or tag of the page.</param>
    ///// <param name="dataContext">DataContext <see cref="object"/></param>
    ///// <returns><see langword="true"/> if the operation succeeds. <see langword="false"/> otherwise.</returns>
    //Task<bool> Navigate(string pageIdOrTargetTag, object? dataContext, CancellationToken cancellationToken = default);

    ///// <summary>
    ///// Synchronously adds an element to the navigation stack and navigates current navigation Frame to the
    ///// </summary>
    ///// <param name="pageType">Type of control to be synchronously added to the navigation stack</param>
    ///// <returns><see langword="true"/> if the operation succeeds. <see langword="false"/> otherwise.</returns>
    //Task<bool> NavigateWithHierarchy(Type pageType, CancellationToken cancellationToken = default);

    ///// <summary>
    ///// Synchronously adds an element to the navigation stack and navigates current navigation Frame to the
    ///// </summary>
    ///// <param name="pageType">Type of control to be synchronously added to the navigation stack</param>
    ///// <param name="dataContext">DataContext <see cref="object"/></param>
    ///// <returns><see langword="true"/> if the operation succeeds. <see langword="false"/> otherwise.</returns>
    //Task<bool> NavigateWithHierarchy(Type pageType, object? dataContext, CancellationToken cancellationToken = default);


    /// <summary>
    /// Provides direct access to the control responsible for navigation.
    /// </summary>
    /// <returns>Instance of the <see cref="INavigationView"/> control.</returns>
    //INavigationView GetNavigationControl();

    /// <summary>
    /// Lets you attach the control that represents the <see cref="INavigationView"/>.
    /// </summary>
    /// <param name="navigation">Instance of the <see cref="INavigationView"/>.</param>
    //void SetNavigationControl(INavigationView navigation);

    /// <summary>
    /// Lets you attach the service that delivers page instances to <see cref="INavigationView"/>.
    /// </summary>
    /// <param name="pageService">Instance of the <see cref="IPageService"/>.</param>
    //void SetPageService(IPageService pageService);


    bool Navigate<T>(object? parameter = null) where T : INavigationView;

    bool Navigate(Type? viewType, object? parameter = null);

    bool NavigateDataSourceSettings(Type? dataSourceType, object? dataSourceContext = null);

    /// <summary>
    /// Navigates the NavigationView to the previous journal entry.
    /// </summary>
    /// <returns><see langword="true"/> if the operation succeeds. <see langword="false"/> otherwise.</returns>
    bool GoBack(object? parameter = null);
}
