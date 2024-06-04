using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Corathing.Contracts.Bases.Interfaces;

public interface IWidgetTypeComponent
{
    // Define the members of WidgetTypeComponent as per your requirements
}

public interface IWidgetGenerator<TDefaultSettings> : IEntity
{
    string Name { get; }
    string Icon { get; }
    Size MinSize { get; }
    string Description { get; }
    bool? Maximizable { get; }
    IWidgetTypeComponent WidgetComp { get; }
    IWidgetTypeComponent SettingsEditorComp { get; }
    Func<TDefaultSettings> CreateDefaultSettings { get; }
    IEnumerable<string> RequiresApi { get; }
    IEnumerable<string> RequiresState { get; }
}
