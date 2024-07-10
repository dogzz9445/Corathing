using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Corathing.Contracts.DataContexts;

namespace Corathing.Contracts.Messages;

public class DataSourceStateChangedMessage : EntityStateChangedMessage<DataSourceContext>
{
    public DataSourceStateChangedMessage(EntityStateChangedType changedtype, DataSourceContext value) : base(changedtype, value)
    {
    }
}
