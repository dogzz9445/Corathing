using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Corathing.Widgets.Basics.DataSources.ToDos;

[ValueConversion(typeof(ToDoTaskType), typeof(int))]
public class ToDoTaskTypeToIndexConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is ToDoTaskType taskType)
        {
            return taskType switch
            {
                ToDoTaskType.TODO => 0,
                ToDoTaskType.DOING => 1,
                ToDoTaskType.NOW => 2,
                ToDoTaskType.LATER => 3,
                ToDoTaskType.DONE => 4,
                _ => 0,
            };
        }
        return 0;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is int taskIndex)
        {
            return taskIndex switch
            {
                0 => ToDoTaskType.TODO,
                1 => ToDoTaskType.DOING,
                2 => ToDoTaskType.NOW,
                3 => ToDoTaskType.LATER,
                4 => ToDoTaskType.DONE,
                _ => ToDoTaskType.TODO,
            };
        }
        return ToDoTaskType.TODO;
    }
}
