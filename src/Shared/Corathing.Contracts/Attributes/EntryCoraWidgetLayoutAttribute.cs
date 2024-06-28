using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Corathing.Contracts.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class EntryCoraWidgetLayoutAttribute : Attribute
{
    public int DefaultColumnSpan { get; }
    public int DefaultRowSpan { get; }
    public int MinimumColumnSpan { get; }
    public int MinimumRowSpan { get; }
    public int MaximumColumnSpan { get; }
    public int MaximumRowSpan { get; }

    public EntryCoraWidgetLayoutAttribute(
        int defaultColumnSpan = 2,
        int defaultRowSpan = 2,
        int minimumColumnSpan = 1,
        int minimumRowSpan = 1,
        int maximumColumnSpan = 10,
        int maximumRowSpan = 10
        )
    {
        DefaultColumnSpan = defaultColumnSpan;
        DefaultRowSpan = defaultRowSpan;
        MinimumColumnSpan = minimumColumnSpan;
        MinimumRowSpan = minimumRowSpan;
        MaximumColumnSpan = maximumColumnSpan;
        MaximumRowSpan = maximumRowSpan;
    }
}
