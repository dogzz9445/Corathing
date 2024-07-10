using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Corathing.Organizer.WPF.Behaviors;

/// <summary>
/// Empty data template selector.
/// </summary>
public class EmptyDataTemplateSelector : DataTemplateSelector
{
    public DataTemplate? EmptyTemplate { get; set; }

    /// <summary>
    /// Selects the template.
    /// If the item is null or DataTemplate does not exist, returns the empty template.
    /// </summary>
    /// <param name="item"></param>
    /// <param name="container"></param>
    /// <returns></returns>
    public override DataTemplate? SelectTemplate(object item, DependencyObject container)
    {
        if (item == null)
        {
            return EmptyTemplate;
        }

        if (container is FrameworkElement frameworkElement)
        {
            var dataTemplateKey = new DataTemplateKey(item.GetType());
            var template = frameworkElement.TryFindResource(dataTemplateKey) as DataTemplate;
            return template ?? EmptyTemplate;
        }

        return base.SelectTemplate(item, container);
    }
}
