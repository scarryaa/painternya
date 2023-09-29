using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace painternya.Controls;

public partial class ReverseStackPanel : Panel
{
    protected override Size MeasureOverride(Size availableSize)
    {
        Size resultSize = new Size(0, 0);

        foreach (var child in Children)
        {
            child.Measure(availableSize);
            resultSize = new Size(Math.Max(resultSize.Width, child.DesiredSize.Width),
                resultSize.Height + child.DesiredSize.Height);
        }

        return resultSize;
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        double offset = finalSize.Height;

        foreach (var child in Children)
        {
            offset -= child.DesiredSize.Height;
            Rect rect = new Rect(new Point(0, offset), child.DesiredSize);
            child.Arrange(rect);
        }

        return finalSize;
    }
}