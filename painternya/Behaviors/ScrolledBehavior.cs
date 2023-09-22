using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;

namespace painternya.Behaviors;

public class ScrolledBehavior
{
    public static readonly AttachedProperty<ICommand> CommandProperty =
        AvaloniaProperty.RegisterAttached<Control, ICommand>(
            "Command",
            typeof(ScrolledBehavior),
            default(ICommand),
            true);

    public static ICommand GetCommand(Control control) => control.GetValue(CommandProperty);
    public static void SetCommand(Control control, ICommand value) => control.SetValue(CommandProperty, value);

    static ScrolledBehavior()
    {
        CommandProperty.Changed.AddClassHandler<ScrollViewer>(HandleCommandChanged);
    }

    private static void HandleCommandChanged(ScrollViewer control, AvaloniaPropertyChangedEventArgs e)
    {
        if (e.NewValue is ICommand newCommand)
        {
            // If the left mouse button is pressed, execute the command and send the position
            // of the pointer as a parameter.
            control.ScrollChanged += (sender, args) =>
            {
                newCommand.Execute(sender);
            };
        }
    }
}