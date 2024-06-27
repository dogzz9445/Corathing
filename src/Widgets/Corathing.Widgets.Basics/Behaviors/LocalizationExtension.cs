using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

using Corathing.Widgets.Basics.Resources;

using Binding = System.Windows.Data.Binding;

namespace Corathing.Widgets.Basics.Behaviors;

public class LocalizationExtension : Binding
{
    public LocalizationExtension(string name) : base("[" + name + "]")
    {
        Mode = BindingMode.OneWay;
        Source = Localizer.Instance;
    }
}

public class Localizer
{
    #region Singleton
    private static Localizer? _instance;
    public static Localizer Instance => _instance ??= new Localizer();
    #endregion

    public string? this[string key]
    {
        get => BasicWidgetStringResources.ResourceManager.GetString(key);
    }

}
