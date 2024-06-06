using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Corathing.Contracts.Bases;

namespace Corathing.Contracts.Factories
{
    public static class WidgetLayoutFactory
    {
        public static WidgetLayout Create(Guid hostId, WidgetContext context) =>
            new WidgetLayout()
            {
                Id = Guid.NewGuid(),
                WidgetStateId = hostId,
                Rect = new WidgetLayoutRect()
                {
                    X = 0,
                    Y = 0,
                    W = 2,
                    H = 2,
                }
            };
    }
}
