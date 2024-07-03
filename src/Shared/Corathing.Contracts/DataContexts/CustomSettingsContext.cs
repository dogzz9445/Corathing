using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;

using Microsoft.VisualBasic.FileIO;

namespace Corathing.Contracts.DataContexts;

public class CustomSettingsChangedMessage : ValueChangedMessage<object?>
{
    public CustomSettingsChangedMessage(object? customSettings) : base(customSettings)
    {
    }
}

public abstract partial class CustomSettingsContext : ObservableObject
{
    protected IServiceProvider Services;
    public Guid Id { get; set; }
    public Guid ParentToken { get; set; }

    [ObservableProperty]
    protected object? _customSettings;

    public void RegisterCustomSettings(Guid token, object? customSettings)
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

    public void Initialize(IServiceProvider services, object? option)
    {
        Id = Guid.NewGuid();
        Services = services;
        CustomSettings = option;

        WeakReferenceMessenger.Default.Register<CustomSettingsChangedMessage, Guid>(this, Id, (r, m)=>
        {

        });

        OnCreate(option);
        OnSettingsChanged(option);
    }

    public void ApplySettings(object? option)
    {
        CustomSettings = option;
        OnSettingsChanged(option);
    }

    public void Destroy()
    {
        OnDestroy();

        Services = null;
        CustomSettings = null;
    }

    public virtual void OnCreate(object? option)
    {
    }

    public abstract void OnContextChanged();

    public abstract void OnSettingsChanged(object? option);

    public virtual void OnMessage(object? message)
    {
    }

    public virtual void OnDestroy()
    {
    }
}
