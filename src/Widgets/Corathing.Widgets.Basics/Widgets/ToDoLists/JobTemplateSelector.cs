using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

using Corathing.Widgets.Basics.Widgets.ToDoLists.Models;

namespace Corathing.Widgets.Basics.Widgets.ToDoLists;

public class JobTemplateSelector : DataTemplateSelector
{
    public override DataTemplate SelectTemplate(object item, DependencyObject container)
    {
        FrameworkElement element = container as FrameworkElement;

        if (item is ToDoJob)
            return element.FindResource("ToDoJobDataTemplate") as DataTemplate;
        else
            return element.FindResource("AddingJobDataTemplate") as DataTemplate;
    }
}
