using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using painternya.Controls;

namespace painternya.Behaviors;

public static class SelectionChangedBehavior
{
    public static readonly AttachedProperty<ICommand> CommandProperty =
        AvaloniaProperty.RegisterAttached<Control, ICommand>(
            "Command",
            typeof(SelectionChangedBehavior),
            default(ICommand),
            true);

    public static ICommand GetCommand(Control control) => control.GetValue(CommandProperty);
    public static void SetCommand(Control control, ICommand value) => control.SetValue(CommandProperty, value);

    static SelectionChangedBehavior()
    {
        CommandProperty.Changed.AddClassHandler<ListBox>(HandleCommandChanged);
    }

    private static void HandleCommandChanged(ListBox control, AvaloniaPropertyChangedEventArgs e)
    {
        if (e.NewValue is ICommand newCommand)
        {
            // If the left mouse button is pressed, execute the command and send the position
            // of the pointer as a parameter.
            control.SelectionChanged += (sender, args) =>
            {
                if (args.AddedItems.Count == 0) return;
                newCommand.Execute(args.AddedItems[0]);
            };
        }
    }
}