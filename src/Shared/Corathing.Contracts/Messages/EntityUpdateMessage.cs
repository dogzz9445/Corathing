using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Corathing.Contracts.Bases.Interfaces;

namespace Corathing.Contracts.Messages;

public class EntityUpdateMessage<T> where T : IEntity
{
    public Guid Id { get; set; }
    public Dictionary<string, object> Changes { get; set; }
}
