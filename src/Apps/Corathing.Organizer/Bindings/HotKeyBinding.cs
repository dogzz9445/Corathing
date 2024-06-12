using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;

using Corathing.Organizer.Managers;

namespace Corathing.Organizer.Bindings;

/// <summary>
/// <code>
/// <Window.InputBindings>
///     <KeyBinding Gesture = "Ctrl+Alt+Add" Command="{Binding IncrementCommand}"
///                 HotKeyBinding.RegisterGlobalHotkey="True" />
///     <KeyBinding Gesture = "Ctrl+Alt+Subtract" Command="{Binding DecrementCommand}"
///                 HotKeyBinding.RegisterGlobalHotkey="True" />
/// </Window.InputBindings>
/// </code>
/// </summary>
public class HotKeyBinding
{
    #region Attached property for KeyBindings

    [AttachedPropertyBrowsableForType(typeof(KeyBinding))]
    public static bool GetRegisterGlobalHotkey(KeyBinding binding)
    {
        return (bool)binding.GetValue(RegisterGlobalHotkeyProperty);
    }

    public static void SetRegisterGlobalHotkey(KeyBinding binding, bool value)
    {
        binding.SetValue(RegisterGlobalHotkeyProperty, value);
    }

    public static readonly DependencyProperty RegisterGlobalHotkeyProperty =
        DependencyProperty.RegisterAttached(
            "RegisterGlobalHotkey",
            typeof(bool),
            typeof(HotKeyBinding),
            new PropertyMetadata(
                false,
                RegisterGlobalHotkeyPropertyChanged));

    private static void RegisterGlobalHotkeyPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var keyBinding = d as KeyBinding;
        if (keyBinding == null)
            return;

        bool oldValue = (bool)e.OldValue;
        bool newValue = (bool)e.NewValue;

        if (DesignerProperties.GetIsInDesignMode(d))
            return;

        if (oldValue && !newValue)
        {
            RemoveKeyBinding(keyBinding);
        }
        else if (newValue && !oldValue)
        {
            AddKeyBinding(keyBinding);
        }
    }
    #endregion

    public static event EventHandler<HotKeyEventArgs> HotkeyAlreadyRegistered;

    private static void OnHotkeyAlreadyRegistered(string name)
    {
        var handler = HotkeyAlreadyRegistered;
        if (handler != null)
            handler(null, new HotKeyEventArgs(name));
    }


    #region Key Binding
    private static readonly Lazy<KeyGestureConverter> _gestureConverter
        = new(() => new KeyGestureConverter());
    public static KeyGestureConverter KeyGestureConverter
    {
        get => _gestureConverter.Value;
    }

    private static string GetNameForKeyBinding(KeyGesture gesture)
    {
        string name = gesture.DisplayString;
        if (string.IsNullOrEmpty(name))
            name = KeyGestureConverter.ConvertToString(gesture);
        return name;
    }
    public static void AddKeyBinding(KeyBinding keyBinding)
    {
        var gesture = (KeyGesture)keyBinding.Gesture;
        string name = GetNameForKeyBinding(gesture);
        try
        {
            GlobalHotKeyManager.Instance.AddOrReplace(name, gesture.Key, gesture.Modifiers, null);
            GlobalHotKeyManager.KeyBindings.Add(keyBinding);
        }
        catch (HotkeyAlreadyRegisteredException)
        {
            OnHotkeyAlreadyRegistered(name);
        }
    }

    public static void RemoveKeyBinding(KeyBinding keyBinding)
    {
        var gesture = (KeyGesture)keyBinding.Gesture;
        string name = GetNameForKeyBinding(gesture);
        GlobalHotKeyManager.Remove(name);
        GlobalHotKeyManager.KeyBindings.Remove(keyBinding);
    }
    #endregion
}
