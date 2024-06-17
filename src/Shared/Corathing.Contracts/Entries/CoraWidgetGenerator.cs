using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

using Corathing.Contracts.Bases;

namespace Corathing.Contracts.Entries;

public class CoraWidgetGenerator
{
    #region Private Fields

    public IServiceProvider Services { get; set; }

    public Func<WidgetContext> CreateWidgetInternal;

    // FIXME:
    // 어떤 방식을 사용할지 정의해야함
    // Generic? Type Converting?
    public Func<WidgetState> CreateWidgetOptionInternal;

    #endregion Private Fields

    #region Public Properties

    /// <summary>
    /// Gets the description.
    /// </summary>
    /// <value>The description.</value>
    public string Description { get; }

    /// <summary>
    /// Gets the name.
    /// </summary>
    /// <value>The name.</value>
    public string Name { get; }

    public CoraWidgetInfo Info { get; set; }

    public Type ViewType { get; }

    public Type ContextType { get; }

    public string DataTemplateSource { get; }

    public Type OptionType { get; }

    #endregion Public Properties

    #region Public Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="Widget"/> class.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="description">The description.</param>
    /// <param name="createWidget">The create widget.</param>
    public CoraWidgetGenerator(
        Type viewType,
        Type contextType,
        string dataTemplateSource,
        Type optionType
        )
    {
        ViewType = viewType;
        ContextType = contextType;
        DataTemplateSource = dataTemplateSource;
        OptionType = optionType;
    }

    #endregion Public Constructors

    #region Public Methods

    /// <summary>
    /// Creates the widget.
    /// </summary>
    /// <returns>WidgetBase.</returns>
    public WidgetContext CreateWidget()
    {
        return (WidgetContext)Activator.CreateInstance(ContextType, Services);
    }

    #endregion Public Methods
}
