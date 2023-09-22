using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using painternya.Models;

namespace painternya.Behaviors;

public class PointerWheelChangedBehavior
{
    public static readonly AttachedProperty<ICommand> CommandProperty =
        AvaloniaProperty.RegisterAttached<Control, ICommand>(
            "Command",
            typeof(PointerWheelChangedBehavior),
            default(ICommand),
            true);

    public static ICommand GetCommand(Control control) => control.GetValue(CommandProperty);
    public static void SetCommand(Control control, ICommand value) => control.SetValue(CommandProperty, value);

    static PointerWheelChangedBehavior()
    {
        CommandProperty.Changed.AddClassHandler<Control>(HandleCommandChanged);
    }

    private static void HandleCommandChanged(Control control, AvaloniaPropertyChangedEventArgs e)
    {
        if (e.NewValue is ICommand newCommand)
        {
            // If the left mouse button is pressed, execute the command and send the position
            // of the pointer as a parameter.
            control.PointerWheelChanged += (sender, args) =>
            {
                // if left ctrl is pressed, zoom
                if (args.KeyModifiers == KeyModifiers.Control)
                {
                    newCommand.Execute(new PointerWheelChangedArgs(args.Delta.Y, args.GetPosition(control as Visual), true, false));
                }
            };
        }
    }
}