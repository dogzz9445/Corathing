using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Corathing.Widgets.Basics.DataSources.ToDos;

public enum ToDoDataSourceType
{
    Local,
    Remote,
}

public enum ToDoDataFormat
{
    Markdown,
    Org,
}

public enum ToDoTaskType
{
    TODO,
    DOING,
    NOW,
    LATER,
    DONE,
}

public struct ToDo
{
    [JsonIgnore]
    public Guid Id { get; private set; }

    public bool IsDone { get; set; }
    public ToDoTaskType TaskType { get; set; }
    public string? Job { get; set; }

    public ToDo()
    {
        Id = Guid.NewGuid();
    }

    public ToDo Copy(
        bool? isDone = null,
        ToDoTaskType? taskType = null,
        string? job = null
    )
        => new ToDo()
        {
            Id = Id,
            IsDone = isDone ?? IsDone,
            TaskType = taskType ?? TaskType,
            Job = job ?? Job,
        };
}

public class ToDoDataSourceOption
{
    public List<ToDo>? ToDos { get; set; }
}
