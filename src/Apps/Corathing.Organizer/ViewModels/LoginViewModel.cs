using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging.Messages;
using Microsoft.Extensions.DependencyInjection;

namespace Corathing.Organizer.ViewModels;

public class LoginViewModel(
    IServiceProvider _services
    ) : ObservableRecipient
{
    protected override void OnActivated()
    {
        if (_services == null)
            return;
        var authService = _services.GetService<IAuthService>();

        if (authService == null)
            // We use a method group here, but a lambda expression is also valid
            Messenger.Register<LoginViewModel, PropertyChangedMessage<object>, string>(this, "Select", (r, m) => r.Receive(m));
    }

    /// <inheritdoc/>
    public void Receive(PropertyChangedMessage<object> message)
    {
        //if (message.Sender.GetType() == typeof(NetworkComputerWidgetViewModel)
        //    // && message.PropertyName == nameof(NetworkComputerWidgetViewModel.SelectedPost)
        //    )
        //{
        //    NetworkComputer = (NetworkComputer)message.NewValue;
        //}
    }
}
