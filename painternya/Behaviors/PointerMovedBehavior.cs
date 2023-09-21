using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using painternya.Controls;

namespace painternya.Behaviors;

public static class PointerMovedBehavior
{
    public static readonly AttachedProperty<ICommand> CommandProperty =
        AvaloniaProperty.RegisterAttached<Control, ICommand>(
            "Command",
            typeof(PointerMovedBehavior),
            default(ICommand),
            true);

    public static ICommand GetCommand(Control control) => control.GetValue(CommandProperty);
    public static void SetCommand(Control control, ICommand value) => control.SetValue(CommandProperty, value);

    static PointerMovedBehavior()
    {
        CommandProperty.Changed.AddClassHandler<Control>(HandleCommandChanged);
    }

    private static void HandleCommandChanged(Control control, AvaloniaPropertyChangedEventArgs e)
    {
        if (e.NewValue is ICommand newCommand)
        {
            // If the left mouse button is pressed, execute the command and send the position
            // of the pointer as a parameter.
            control.PointerMoved += (sender, args) =>
            {
                if (args.GetCurrentPoint(control).Properties.IsLeftButtonPressed)
                {
                    newCommand.Execute(args.GetPosition(control));
                }
            };
        }
    }
}
