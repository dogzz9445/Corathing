using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Corathing.Contracts.Bases;

public class WidgetGenerator
{
    #region Private Fields

    public Func<WidgetContext> CreateWidgetInternal;

    // FIXME:
    // 어떤 방식을 사용할지 정의해야함
    // Generic? Type Converting?
    public Func<WidgetState<object>> CreateWidgetOptionInternal;

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

    /// <summary>
    /// Gets the MenuPath
    /// </summary>
    public string MenuPath { get; }

    public int MenuOrder { get; }

    public Type TargetType { get; }

    #endregion Public Properties

    #region Public Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="Widget"/> class.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="description">The description.</param>
    /// <param name="createWidget">The create widget.</param>
    public WidgetGenerator(
        string name,
        string description,
        string menuPath,
        int menuOrder,
        Type targetType,
        Func<WidgetContext> createWidget)
    {
        Name = name;
        MenuPath = menuPath;
        Description = description;
        MenuOrder = menuOrder;
        TargetType = targetType;
        CreateWidgetInternal = createWidget;
    }

    #endregion Public Constructors

    #region Public Methods

    /// <summary>
    /// Creates the widget.
    /// </summary>
    /// <returns>WidgetBase.</returns>
    public WidgetContext CreateWidget()
    {
        return CreateWidgetInternal.Invoke();
    }

    #endregion Public Methods
}
