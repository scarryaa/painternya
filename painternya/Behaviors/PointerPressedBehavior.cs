using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;

namespace painternya.Behaviors;

public class PointerPressedBehavior
{
    public static readonly AttachedProperty<ICommand> CommandProperty =
        AvaloniaProperty.RegisterAttached<Control, ICommand>(
            "Command",
            typeof(PointerPressedBehavior),
            default(ICommand),
            true);

    public static ICommand GetCommand(Control control) => control.GetValue(CommandProperty);
    public static void SetCommand(Control control, ICommand value) => control.SetValue(CommandProperty, value);

    static PointerPressedBehavior()
    {
        CommandProperty.Changed.AddClassHandler<Control>(HandleCommandChanged);
    }

    private static void HandleCommandChanged(Control control, AvaloniaPropertyChangedEventArgs e)
    {
        control.PointerPressed -= Control_PointerPressed;

        if (e.NewValue is ICommand newCommand)
        {
            control.Tag = newCommand;
            control.PointerPressed += Control_PointerPressed;
        }
    }

    private static void Control_PointerPressed(object sender, PointerPressedEventArgs e)
    {
        if (sender is Control control && control.Tag is ICommand command)
        {
            command.Execute(e.GetPosition(control));
            e.Handled = true;
        }
    }
}