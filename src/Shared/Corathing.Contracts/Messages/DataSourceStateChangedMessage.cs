using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Corathing.Contracts.Messages;

public class DataSourceStateChangedMessage<T> : EntityStateChangedMessage<T>
{
    public DataSourceStateChangedMessage(EntityStateChangedType changedtype, T value) : base(changedtype, value)
    {
    }
}
