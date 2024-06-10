using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows;
using Corathing.Organizer.Models;

namespace Corathing.Organizer.Behaviors;

public class TemplateSelector : DataTemplateSelector
{
    public DataTemplate ItemTemplate { get; set; }
    public DataTemplate NewButtonTemplate { get; set; }

    public override DataTemplate SelectTemplate(object item, DependencyObject container)
    {
        if (item == CollectionView.NewItemPlaceholder)
        {
            return NewButtonTemplate;
        }
        else
        {
            return ItemTemplate;
        }
    }
}
