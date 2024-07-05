using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommunityToolkit.Mvvm.Messaging.Messages;

namespace Corathing.Contracts.Messages;

public enum EntityStateChangedType
{
    Added = 0,
    Removed = 1,
    Migrated = 2,
    Moved = 3,
    Modified = 4,
}

public class EntityStateChangedMessage<T> : ValueChangedMessage<T>
{
    public EntityStateChangedType ChangedType { get; set; }
    public EntityStateChangedMessage(EntityStateChangedType changedtype, T value) : base(value)
    {
        ChangedType = changedtype;
    }
}
