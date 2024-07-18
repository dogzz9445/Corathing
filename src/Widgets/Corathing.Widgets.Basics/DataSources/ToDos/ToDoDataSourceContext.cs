using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Corathing.Contracts.Attributes;
using Corathing.Contracts.Bases;
using Corathing.Contracts.DataContexts;
using Corathing.Contracts.Services;
using Corathing.Widgets.Basics.DataSources.ExecutableApps;

using Microsoft.CodeAnalysis;

using static System.Windows.Forms.AxHost;

namespace Corathing.Widgets.Basics.DataSources.ToDos;


[EntryCoraDataSource(
    dataSourceType: typeof(ToDoDataSourceContext),
    optionType: typeof(ToDoDataSourceOption),
    settingsContextType: typeof(ToDoDataSourceOptionContext),
    name: "To Do Data",
    description: "To Do Data.",
    defaultTitle: "Default To Do Data"
)]
[EntryCoraDataSourceDefaultTitle(ApplicationLanguage.en_US, "Default To Do Data")]
[EntryCoraDataSourceDefaultTitle(ApplicationLanguage.ko_KR, "기본 할일 목록")]
[EntryCoraDataSourceName(ApplicationLanguage.en_US, "To Do Data")]
[EntryCoraDataSourceName(ApplicationLanguage.ko_KR, "할 일 목록")]
[EntryCoraDataSourceDescription(ApplicationLanguage.en_US, "To Do Data")]
[EntryCoraDataSourceDescription(ApplicationLanguage.ko_KR, "할 일 목록")]
public class ToDoDataSourceContext : DataSourceContext
{
    public override void OnCreate(IServiceProvider services, DataSourceState state)
    {
    }

    public override void OnStateChanged(DataSourceState state)
    {
        if (state.CustomSettigns is not ToDoDataSourceOption option)
        {
            return;
        }
    }

    public void Add(ToDo toDo)
    {
        if (State.CustomSettigns is not ToDoDataSourceOption option)
        {
            return;
        }
        option.ToDos.Add(toDo);
        SaveState();
    }

    public void Remove(ToDo toDo)
    {
        if (State.CustomSettigns is not ToDoDataSourceOption option)
        {
            return;
        }
        option.ToDos.Remove(toDo);
        SaveState();
    }

    public void Update(ToDo toDo)
    {
        if (State.CustomSettigns is not ToDoDataSourceOption option)
        {
            return;
        }
        var index = option.ToDos.FindIndex(t => t.Id == toDo.Id);
        if (index == -1)
        {
            option.ToDos.Add(toDo);
        }
        else
        {
            option.ToDos[index] = toDo;
        }
        SaveState();
    }

    public List<ToDo>? GetToDos()
    {
        if (State.CustomSettigns is not ToDoDataSourceOption option)
        {
            return null;
        }
        return option.ToDos;
    }
}
