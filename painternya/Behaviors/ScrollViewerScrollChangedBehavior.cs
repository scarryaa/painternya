using System;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;

namespace painternya.Behaviors;

public class ScrollViewerScrollChangedBehavior
{
    public static readonly AttachedProperty<ICommand> CommandProperty =
        AvaloniaProperty.RegisterAttached<Control, ICommand>(
            "Command",
            typeof(ScrollViewerScrollChangedBehavior),
            default(ICommand),
            true);

    public static ICommand GetCommand(Control control) => control.GetValue(CommandProperty);
    public static void SetCommand(Control control, ICommand value) => control.SetValue(CommandProperty, value);

    static ScrollViewerScrollChangedBehavior()
    {
        CommandProperty.Changed.AddClassHandler<ScrollViewer>(HandleCommandChanged);
    }

    private static void HandleCommandChanged(ScrollViewer control, AvaloniaPropertyChangedEventArgs e)
    {
        control.ScrollChanged -= Control_ScrollChanged;

        if (e.NewValue is ICommand newCommand)
        {
            control.Tag = newCommand;
            control.ScrollChanged += Control_ScrollChanged;
        }
    }

    private static void Control_ScrollChanged(object sender, EventArgs e)
    {
        if (sender is ScrollViewer control && control.Tag is ICommand command)
        {
            command.Execute(sender);
        }
    }

}