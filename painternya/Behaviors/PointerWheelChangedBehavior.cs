using System;
using System.Linq;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.LogicalTree;
using painternya.Models;
using Canvas = painternya.Controls.Canvas;

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
            control.PointerWheelChanged += (sender, args) =>
            {
                if (args.KeyModifiers == KeyModifiers.Control)
                {
                    args.Handled = true;
                    // Calculate the zoom factor based on wheel direction.
                    double zoomFactor = args.Delta.Y;
                
                    // Get the position relative to the content inside the ScrollViewer.
                    var positionRelativeToContent = args.GetPosition((sender as Control).GetLogicalChildren().OfType<Canvas>().FirstOrDefault());
                
                    newCommand.Execute(new PointerWheelChangedArgs(zoomFactor, positionRelativeToContent, true, false));
                }
            };
        }
    }
}