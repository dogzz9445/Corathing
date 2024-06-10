using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;

using Microsoft.Xaml.Behaviors;
using System.Windows.Controls;
using ControlzEx.Behaviors;
using Corathing.Dashboards.WPF.Controls;

namespace Corathing.Organizer.Behaviors;

public class DoubleClickTextBoxBehavior : Behavior<TextBox>
{
    #region Public Properties

    /// <summary>
    /// The edit mode property
    /// </summary>
    public static readonly DependencyProperty EditModeProperty = DependencyProperty.Register(
        nameof(EditMode),
        typeof(bool?),
        typeof(DoubleClickTextBoxBehavior),
        new PropertyMetadata(false));

    /// <summary>
    /// Gets or sets a value indicating whether the dashboard is in [edit mode].
    /// </summary>
    /// <value><c>true</c> if [edit mode]; otherwise, <c>false</c>.</value>
    public bool? EditMode
    {
        get => (bool?)GetValue(EditModeProperty);
        set => SetValue(EditModeProperty, value);
    }


    public Style TextBoxNoEditStyle
    {
        get => (Style)GetValue(TextBoxNoEditStyleProperty);
        set => SetValue(TextBoxNoEditStyleProperty, value);
    }

    public static readonly DependencyProperty TextBoxNoEditStyleProperty = DependencyProperty.Register(
        nameof(TextBoxNoEditStyle),
        typeof(Style),
        typeof(DoubleClickTextBoxBehavior),
        new PropertyMetadata(null));

    public Style TextBoxEditingStyle
    {
        get => (Style)GetValue(TextBoxEditingStyleProperty);
        set => SetValue(TextBoxEditingStyleProperty, value);
    }

    public static readonly DependencyProperty TextBoxEditingStyleProperty = DependencyProperty.Register(
        nameof(TextBoxEditingStyle),
        typeof(Style),
        typeof(DoubleClickTextBoxBehavior),
        new PropertyMetadata(null));

    #endregion

    protected override void OnAttached()
    {
        AssociatedObject.Focusable = false;
        AssociatedObject.Cursor = Cursors.Arrow;

        AssociatedObject.Style = TextBoxNoEditStyle;
        AssociatedObject.MouseDoubleClick += AssociatedObjectOnMouseDoubleClick;
        AssociatedObject.LostFocus += AssociatedObjectOnLostFocus;
        AssociatedObject.KeyDown += AssociatedObject_KeyDown;
    }

    private void AssociatedObject_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Return && e.Key == Key.Enter)
        {
            RaiseLostFocus();
        }
    }

    private void RaiseLostFocus()
    {
        AssociatedObject.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
    }

    protected override void OnDetaching()
    {
        base.OnDetaching();

        AssociatedObject.MouseDoubleClick -= AssociatedObjectOnMouseDoubleClick;
        AssociatedObject.LostFocus -= AssociatedObjectOnLostFocus;
        AssociatedObject.KeyDown -= AssociatedObject_KeyDown;
    }

    private void AssociatedObjectOnMouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (!(EditMode ?? false))
            return;

        if (e.ChangedButton != MouseButton.Left)
            return;

        if (AssociatedObject.Focusable)
            return;//fix an issue of selecting all text on double click

        e.Handled = true;
        AssociatedObject.Style = TextBoxEditingStyle;
        AssociatedObject.Cursor = Cursors.IBeam;
        AssociatedObject.Focusable = true;
        AssociatedObject.Focus();
        AssociatedObject.CaretIndex = AssociatedObject.Text.Length;
        AssociatedObject.SelectionStart = AssociatedObject.Text.Length;
    }

    private void AssociatedObjectOnLostFocus(object sender, RoutedEventArgs e)
    {
        AssociatedObject.Style = TextBoxNoEditStyle;
        AssociatedObject.Cursor = Cursors.Arrow;
        AssociatedObject.Focusable = false;
    }
}
