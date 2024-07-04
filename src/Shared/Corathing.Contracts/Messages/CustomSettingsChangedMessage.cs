using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommunityToolkit.Mvvm.Messaging.Messages;

namespace Corathing.Contracts.Messages;

public class CustomSettingsChangedMessage : ValueChangedMessage<object?>
{
    public CustomSettingsChangedMessage(object? customSettings) : base(customSettings)
    {
    }
}
