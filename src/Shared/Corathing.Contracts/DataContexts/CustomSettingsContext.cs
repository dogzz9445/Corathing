﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;

using Corathing.Contracts.Messages;

using Microsoft.VisualBasic.FileIO;

namespace Corathing.Contracts.DataContexts;

public abstract partial class CustomSettingsContext : ObservableObject
{
    protected IServiceProvider? Services;
    protected Guid Id { get; set; }
    public Guid ParentToken { get; set; }

    [ObservableProperty]
    protected object? _customSettings;

    /// <summary>
    /// This is called from generator.
    /// Do not call this method directly.
    /// </summary>
    /// <param name="services"></param>
    /// <param name="emtpyOption"></param>
    public void Initialize(IServiceProvider services, object? emtpyOption)
    {
        Services = services;
        CustomSettings = emtpyOption;
        Id = Guid.NewGuid();
        WeakReferenceMessenger.Default.Register<CustomSettingsChangedMessage, Guid>(
            this,
            Id,
            OnChildCustomSettingsChanged
        );

        OnCreate(emtpyOption);
        OnSettingsChanged(emtpyOption);
    }

    public void RegisterSettings(Guid token, object? customSettings)
    {
        ParentToken = token;
        OnSettingsChanged(customSettings);
        PropertyChanged += (sender, e) =>
        {
            OnContextChanged();
            WeakReferenceMessenger.Default.Send(
                new CustomSettingsChangedMessage(CustomSettings), ParentToken
            );
        };
    }

    public void ApplySettings(object? option)
    {
        CustomSettings = option;
        OnSettingsChanged(option);
    }

    public void Destroy()
    {
        OnDestroy();
    }

    protected abstract void OnCreate(object? defaultOption);

    /// <summary>
    /// Must define Context To CustomSettings
    /// Context 에서 WebPage Option 으로의 변환을 정의해야함
    /// <code>
    /// if (CustomSettings is not WebPageOption webPageOption)
    ///     throw new ArgumentException($"Not a valid type for CustomSettings {nameof(WebPageOption)}");
    /// webPageOption.AutoReloadInterval = AutoReloadInterval;
    /// webPageOption.WebSessionDataSourceId = null;
    /// if (WebSessionDataSourceSelector.SelectedDataSourceContext != null)
    /// {
    ///    webPageOption.WebSessionDataSourceId = WebSessionDataSourceSelector.SelectedDataSourceContext.DataSourceId;
    /// }
    /// CustomSettings = webPageOption;
    /// </code>
    /// </summary>
    protected abstract void OnContextChanged();

    /// <summary>
    /// Must define CustomSettings(parameter option) To Context
    /// CustomSettings 에서 Context 로의 변환을 정의해야함
    /// <code>
    /// if (option is not WebPageOption webPageOption)
    ///     throw new ArgumentException($"Not a valid type for CustomSettings {nameof(WebPageOption)}");
    /// AutoReloadInterval = webPageOption.AutoReloadInterval;
    /// WebSessionDataSourceSelector.Select(webPageOption.WebSessionDataSourceId);        
    /// </code>
    /// </summary>
    /// <param name="option"></param>
    protected abstract void OnSettingsChanged(object? option);

    protected virtual void OnMessage(object? message)
    {
    }

    protected virtual void OnDestroy()
    {
    }

    protected virtual void OnChildCustomSettingsChanged(object? sender, CustomSettingsChangedMessage? message)
    {
    }
}
