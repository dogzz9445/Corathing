using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

using Wpf.Ui.Controls;

namespace Corathing.UI.WPF.Controls;

public class SettingsBlock : System.Windows.Controls.Primitives.ButtonBase
{

    /// <summary>Identifies the <see cref="Icon"/> dependency property.</summary>
    public static readonly DependencyProperty IconProperty = DependencyProperty.Register(
        nameof(Icon),
        typeof(IconElement),
        typeof(SettingsBlock),
        new PropertyMetadata(null)
    );

    /// <summary>Identifies the <see cref="Header"/> dependency property.</summary>
    public static readonly DependencyProperty IsDescriptionOnContentRightProperty = DependencyProperty.Register(
        nameof(IsDescriptionOnContentRight),
        typeof(bool),
        typeof(SettingsBlock),
        new PropertyMetadata(false)
    );

    /// <summary>Identifies the <see cref="Header"/> dependency property.</summary>
    public static readonly DependencyProperty CategoryTextProperty = DependencyProperty.Register(
        nameof(CategoryText),
        typeof(string),
        typeof(SettingsBlock),
        new PropertyMetadata(null)
    );

    /// <summary>Identifies the <see cref="Header"/> dependency property.</summary>
    public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register(
        nameof(Header),
        typeof(string),
        typeof(SettingsBlock),
        new PropertyMetadata(null)
    );

    /// <summary>Identifies the <see cref="Description"/> dependency property.</summary>
    public static readonly DependencyProperty DescriptionProperty = DependencyProperty.Register(
        nameof(Description),
        typeof(string),
        typeof(SettingsBlock),
        new PropertyMetadata(null)
    );

    /// <summary>Identifies the <see cref="CornerRadius"/> dependency property.</summary>
    public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register(
        nameof(CornerRadius),
        typeof(CornerRadius),
        typeof(SettingsBlock),
        new PropertyMetadata(new CornerRadius(0))
    );

    /// <summary>
    /// Gets or sets displayed <see cref="IconElement"/>.
    /// </summary>
    [Bindable(true)]
    [Category("Appearance")]
    public IconElement? Icon
    {
        get => (IconElement?)GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    [Bindable(true)]
    public bool IsDescriptionOnContentRight
    {
        get => (bool)GetValue(IsDescriptionOnContentRightProperty);
        set => SetValue(IsDescriptionOnContentRightProperty, value);
    }

    /// <summary>
    /// Gets or sets header which is used for each item in the control.
    /// </summary>
    [Bindable(true)]
    public string? CategoryText
    {
        get => (string?)GetValue(CategoryTextProperty);
        set => SetValue(CategoryTextProperty, value);
    }

    /// <summary>
    /// Gets or sets header which is used for each item in the control.
    /// </summary>
    [Bindable(true)]
    public string? Header
    {
        get => (string?)GetValue(HeaderProperty);
        set => SetValue(HeaderProperty, value);
    }

    /// <summary>
    /// Gets or sets description which is used for each item in the control.
    /// </summary>
    [Bindable(true)]
    public string? Description
    {
        get => (string?)GetValue(DescriptionProperty);
        set => SetValue(DescriptionProperty, value);
    }

    /// <summary>
    /// Gets or sets the corner radius of the control.
    /// </summary>
    [Bindable(true)]
    [Category("Appearance")]
    public CornerRadius CornerRadius
    {
        get => (CornerRadius)GetValue(CornerRadiusProperty);
        set => SetValue(CornerRadiusProperty, value);
    }

}
