using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using painternya.Controls;

namespace painternya.Behaviors;

public static class IsCheckedChangedBehavior
{
    public static readonly AttachedProperty<ICommand> CommandProperty =
        AvaloniaProperty.RegisterAttached<CheckBox, ICommand>(
            "Command",
            typeof(IsCheckedChangedBehavior),
            default(ICommand),
            true);

    public static ICommand GetCommand(CheckBox control) => control.GetValue(CommandProperty);
    public static void SetCommand(CheckBox control, ICommand value) => control.SetValue(CommandProperty, value);

    static IsCheckedChangedBehavior()
    {
        CommandProperty.Changed.AddClassHandler<CheckBox>(HandleCommandChanged);
    }

    private static void HandleCommandChanged(CheckBox control, AvaloniaPropertyChangedEventArgs e)
    {
        if (e.NewValue is ICommand newCommand)
        {
            // If the left mouse button is pressed, execute the command and send the position
            // of the pointer as a parameter.
            control.IsCheckedChanged += (sender, args) =>
            {
                newCommand.Execute(null);
            };
        }
    }
}