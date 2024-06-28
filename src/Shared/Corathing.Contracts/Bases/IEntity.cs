using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Corathing.Contracts.Bases;

public interface IEntity
{
    // Assuming the Entity interface has an Id property
    Guid Id { get; set; }
}
