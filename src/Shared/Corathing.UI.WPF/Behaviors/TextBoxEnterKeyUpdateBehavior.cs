// original source: https://stackoverflow.com/questions/23613171/wpf-how-to-make-textbox-lose-focus-after-hitting-enter

using System;
using System.Windows.Controls;
using System.Windows.Input;

using Microsoft.Xaml.Behaviors;

namespace Corathing.UI.WPF.Behaviors;

public class TextBoxEnterKeyUpdateBehavior : Behavior<TextBox>
{
    protected override void OnAttached()
    {
        if (AssociatedObject != null)
        {
            base.OnAttached();
            AssociatedObject.KeyDown += AssociatedObject_KeyDown;
        }
    }

    protected override void OnDetaching()
    {
        if (AssociatedObject != null)
        {
            AssociatedObject.KeyDown -= AssociatedObject_KeyDown;
            base.OnDetaching();
        }
    }

    private void AssociatedObject_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
    {
        if (sender is TextBox textBox && e.Key == Key.Return && e.Key == Key.Enter)
        {
            textBox.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
        }
    }
}
