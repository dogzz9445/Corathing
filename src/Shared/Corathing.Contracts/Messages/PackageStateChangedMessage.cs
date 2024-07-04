using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommunityToolkit.Mvvm.Messaging.Messages;

namespace Corathing.Contracts.Messages;

public class PackageStateChangedMessage : ValueChangedMessage<object?>
{
    public PackageStateChangedMessage(object? customSettings) : base(customSettings)
    {
    }
}
