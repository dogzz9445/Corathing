using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

using Microsoft.Xaml.Behaviors;

namespace Corathing.WPF.Sample.Behaviors
{
    public class MouseTrigger : TriggerBase<UIElement>
    {
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register(nameof(Command), typeof(ICommand), typeof(MouseTrigger), new PropertyMetadata(null));

        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.Register(nameof(CommandParameter), typeof(object), typeof(MouseTrigger), new PropertyMetadata(null));

        public static readonly DependencyProperty EventTypeProperty =
            DependencyProperty.Register(nameof(EventType), typeof(MouseTriggerType), typeof(MouseTrigger),
                new PropertyMetadata(MouseTriggerType.PreviewMouseDown, OnEventTypeChanged));

        public ICommand Command
        {
            get => (ICommand)GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }

        public object CommandParameter
        {
            get => GetValue(CommandParameterProperty);
            set => SetValue(CommandParameterProperty, value);
        }

        public MouseTriggerType EventType
        {
            get => (MouseTriggerType)GetValue(EventTypeProperty);
            set => SetValue(EventTypeProperty, value);
        }

        private static void OnEventTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MouseTrigger trigger = (MouseTrigger)d;

            // 기존 이벤트 핸들러 제거
            trigger.DetachHandlers();

            // 새 이벤트 타입에 핸들러 연결
            trigger.AttachHandlers();
        }

        private void AttachHandlers()
        {
            if (AssociatedObject == null) return;

            switch (EventType)
            {
                case MouseTriggerType.PreviewMouseDown:
                    AssociatedObject.PreviewMouseDown += OnPreviewMouseDown;
                    break;
                case MouseTriggerType.MouseDown:
                    AssociatedObject.MouseDown += OnMouseDown;
                    break;
                case MouseTriggerType.MouseUp:
                    AssociatedObject.MouseUp += OnMouseUp;
                    break;
                case MouseTriggerType.MouseEnter:
                    AssociatedObject.MouseEnter += OnMouseEnter;
                    break;
                case MouseTriggerType.MouseLeave:
                    AssociatedObject.MouseLeave += OnMouseLeave;
                    break;
            }
        }

        private void DetachHandlers()
        {
            if (AssociatedObject == null) return;

            AssociatedObject.PreviewMouseDown -= OnPreviewMouseDown;
            AssociatedObject.MouseDown -= OnMouseDown;
            AssociatedObject.MouseUp -= OnMouseUp;
            AssociatedObject.MouseEnter -= OnMouseEnter;
            AssociatedObject.MouseLeave -= OnMouseLeave;
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            AttachHandlers();
        }

        protected override void OnDetaching()
        {
            DetachHandlers();
            base.OnDetaching();
        }

        private void OnPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (Command != null && Command.CanExecute(CommandParameter))
            {
                Command.Execute(e);
            }
        }

        private void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (Command != null && Command.CanExecute(CommandParameter))
            {
                Command.Execute(e);
            }
        }

        private void OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (Command != null && Command.CanExecute(CommandParameter))
            {
                Command.Execute(e);
            }
        }

        private void OnMouseEnter(object sender, MouseEventArgs e)
        {
            if (Command != null && Command.CanExecute(CommandParameter))
            {
                Command.Execute(e);
            }
        }

        private void OnMouseLeave(object sender, MouseEventArgs e)
        {
            if (Command != null && Command.CanExecute(CommandParameter))
            {
                Command.Execute(e);
            }
        }
    }

    public enum MouseTriggerType
    {
        PreviewMouseDown,
        MouseDown,
        MouseUp,
        MouseEnter,
        MouseLeave
    }
}
