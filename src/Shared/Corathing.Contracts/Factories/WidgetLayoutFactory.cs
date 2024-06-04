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
        public static WidgetLayout Create(WidgetContext context) =>
            new WidgetLayout()
            {
                Id = Guid.NewGuid(),
                WidgetStateId = context.WidgetStateGuid,
                Rect = new WidgetLayoutRect()
                {
                    X = 0,
                    Y = 0,
                    W = 0,
                    H = 0,
                }
            };
    }
}
