using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Interop;
using System.Xml.Linq;

using MahApps.Metro.Controls;

namespace Corathing.Organizer.WPF.Managers;

public class WeakReferenceCollection<T> : IEnumerable<T>
    where T : class
{
    private readonly List<WeakReference> _references = new List<WeakReference>();

    public IEnumerator<T> GetEnumerator()
    {
        var references = _references.ToList();
        foreach (var reference in references)
        {
            var target = reference.Target;
            if (target != null)
                yield return (T)target;
        }
        Trim();
    }

    public void Add(T item)
    {
        _references.Add(new WeakReference(item));
    }

    public void Remove(T item)
    {
        _references.RemoveAll(r => (r.Target ?? item) == item);
    }

    public void Trim()
    {
        _references.RemoveAll(r => !r.IsAlive);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}

public class HotKeyEventArgs : EventArgs
{
    public string Name { get; }
    public bool Handled { get; set; }
    public bool Succeeded { get; set; }
    public bool AlreadyRegistered { get; set; } = false;

    public HotKeyEventArgs(string name)
    {
        Name = name;
    }
}
[Serializable]
public class HotkeyAlreadyRegisteredException : Exception
{
    private readonly string _name;

    public HotkeyAlreadyRegisteredException(string name, Exception inner) : base(inner.Message, inner)
    {
        _name = name;
        HResult = Marshal.GetHRForException(inner);
    }

    protected HotkeyAlreadyRegisteredException(
        SerializationInfo info,
        StreamingContext context) : base(info, context)
    {
        _name = (string)info.GetValue("_name", typeof(string));
    }

    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        base.GetObjectData(info, context);
        info.AddValue("_name", _name);
    }

    public string Name
    {
        get { return _name; }
    }
}

public class HotKey
{
    //핫키등록       
    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool RegisterHotKey(IntPtr hWnd, int id, ModifierKeys fsModifiers, uint vk);

    //핫키제거       
    [DllImport("user32.dll", SetLastError = true)]
    internal static extern bool UnregisterHotKey(IntPtr hWnd, int id);

    private static int _nextId;

    public int Id { get; }

    public uint VirtualKey { get; }

    public ModifierKeys Flags { get; }

    public EventHandler<HotKeyEventArgs> Handler { get; }

    private IntPtr _hwnd;

    public HotKey(uint virtualKey, ModifierKeys flags, EventHandler<HotKeyEventArgs> handler)
    {
        Id = ++_nextId;
        VirtualKey = virtualKey;
        Flags = flags;
        Handler = handler;
    }

    public void Register(IntPtr hwnd, string name)
    {
        if (!RegisterHotKey(hwnd, Id, Flags, VirtualKey))
        {
            var hr = Marshal.GetHRForLastWin32Error();
            var ex = Marshal.GetExceptionForHR(hr);
            if ((uint)hr == 0x80070581)
                throw new HotkeyAlreadyRegisteredException(name, ex);
            throw ex;
        }
        _hwnd = hwnd;

    }

    public void Unregister()
    {
        if (_hwnd != IntPtr.Zero)
        {
            if (!UnregisterHotKey(_hwnd, Id))
            {
                var hr = Marshal.GetHRForLastWin32Error();
                throw Marshal.GetExceptionForHR(hr);
            }
            _hwnd = IntPtr.Zero;
        }
    }

}

public class GlobalHotKeyManager
{
    private readonly Lazy<WeakReferenceCollection<KeyBinding>> _keyBindings
        = new(() => []);
    public static WeakReferenceCollection<KeyBinding> KeyBindings
    {
        get => Instance._keyBindings.Value;
    }

    public readonly static new GlobalHotKeyManager Instance = new GlobalHotKeyManager();
    private readonly Dictionary<int, string> _hotkeyNames = new Dictionary<int, string>();
    private readonly Dictionary<string, HotKey> _hotkeys = new Dictionary<string, HotKey>();

    private static readonly IntPtr HwndMessage = (IntPtr)(-3);
    private const int WmHotkey = 0x0312;

    private IntPtr _hwnd;
    private readonly HwndSource _source;

    public bool IsEnabled { get; set; } = true;

    private void SetHwnd(IntPtr hwnd)
    {
        _hwnd = hwnd;
    }

    public static void AddOrReplace(string name, uint virtualKey, ModifierKeys flags, EventHandler<HotKeyEventArgs> handler)
    {
        var hotkey = new HotKey(virtualKey, flags, handler);
        lock (Instance._hotkeys)
        {
            Remove(name);
            Instance._hotkeys.Add(name, hotkey);
            Instance._hotkeyNames.Add(hotkey.Id, name);
            if (Instance._hwnd != IntPtr.Zero)
                hotkey.Register(Instance._hwnd, name);
        }
    }

    public static void Remove(string name)
    {
        lock (Instance._hotkeys)
        {
            HotKey hotkey;
            if (Instance._hotkeys.TryGetValue(name, out hotkey))
            {
                Instance._hotkeys.Remove(name);
                Instance._hotkeyNames.Remove(hotkey.Id);
                if (Instance._hwnd != IntPtr.Zero)
                    hotkey.Unregister();
            }
        }
    }

    public void RemoveAll()
    {
        lock (_hotkeys)
        {
            if (_hwnd != IntPtr.Zero)
            {
                foreach (var hotkeyPair in Instance._hotkeys)
                {
                    hotkeyPair.Value.Unregister();
                }
            }
            _hotkeys.Clear();
        }
    }

    private GlobalHotKeyManager()
    {
        var parameters = new HwndSourceParameters("Hotkey sink")
        {
            HwndSourceHook = HandleMessage,
            ParentWindow = HwndMessage
        };
        _source = new HwndSource(parameters);
        SetHwnd(_source.Handle);
        App.Current.Exit += (s, e) => RemoveAll();
    }

    public static void AddOrReplace(string name, KeyGesture gesture, EventHandler<HotKeyEventArgs> handler)
    {
        AddOrReplace(name, gesture, handler);
    }

    public void AddOrReplace(string name, Key key, ModifierKeys modifiers, EventHandler<HotKeyEventArgs> handler)
    {
        var flags = modifiers;
        var vk = (uint)KeyInterop.VirtualKeyFromKey(key);
        AddOrReplace(name, vk, flags, handler);
    }

    public static IntPtr HandleHotkeyMessage(
        IntPtr hwnd,
        int msg,
        IntPtr wParam,
        IntPtr lParam,
        ref bool handled,
        out HotKey hotkey)
    {
        hotkey = null;
        if (Instance.IsEnabled && msg == WmHotkey)
        {
            int id = wParam.ToInt32();
            string name;
            if (Instance._hotkeyNames.TryGetValue(id, out name))
            {
                hotkey = Instance._hotkeys[name];
                var handler = hotkey.Handler;
                if (handler != null)
                {
                    var e = new HotKeyEventArgs(name);
                    handler(Instance, e);
                    handled = e.Handled;
                }
            }
        }
        return IntPtr.Zero;
    }

    private IntPtr HandleMessage(IntPtr hwnd, int msg, IntPtr wparam, IntPtr lparam, ref bool handled)
    {
        HotKey hotkey;
        var result = HandleHotkeyMessage(hwnd, msg, wparam, lparam, ref handled, out hotkey);
        if (handled)
            return result;

        if (hotkey != null)
            handled = ExecuteBoundCommand(hotkey);
        return result;
    }

    private bool ExecuteBoundCommand(HotKey hotkey)
    {
        var key = KeyInterop.KeyFromVirtualKey((int)hotkey.VirtualKey);
        var modifiers = hotkey.Flags;
        bool handled = false;
        foreach (var binding in KeyBindings)
        {
            if (binding.Key == key && binding.Modifiers == modifiers)
            {
                handled |= ExecuteCommand(binding);
            }
        }
        return handled;
    }

    private static bool ExecuteCommand(InputBinding binding)
    {
        var command = binding.Command;
        var parameter = binding.CommandParameter;
        var target = binding.CommandTarget;

        if (command == null)
            return false;

        var routedCommand = command as RoutedCommand;
        if (routedCommand != null)
        {
            if (routedCommand.CanExecute(parameter, target))
            {
                routedCommand.Execute(parameter, target);
                return true;
            }
        }
        else
        {
            if (command.CanExecute(parameter))
            {
                command.Execute(parameter);
                return true;
            }
        }
        return false;
    }
}
