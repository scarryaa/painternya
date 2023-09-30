using System;
using painternya.Interfaces;
using painternya.Tools;

namespace painternya.Services;

public class ToolManager
{
    private ITool _currentTool;
    private static int _globalCurrentToolSize = 4;
    private ITool _pencil = new PencilTool(_globalCurrentToolSize);
    private ITool _eraser = new EraserTool(_globalCurrentToolSize);
    private ITool _brush = new BrushTool(_globalCurrentToolSize);

    public ITool CurrentTool
    {
        get => _currentTool;
        set => _currentTool = value;
    }

    public ITool Pencil => _pencil;
    public ITool Eraser => _eraser;
    public ITool Brush => _brush;

    public int GlobalCurrentToolSize
    {
        get => _globalCurrentToolSize;
        set
        {
            _globalCurrentToolSize = value;
            _currentTool.Size = value;
        }
    }

    public void SelectTool(string tool)
    {
        switch (tool)
        {
            case "Pencil":
                CurrentTool = Pencil;
                break;
            case "Eraser":
                CurrentTool = Eraser;
                break;
            case "Brush":
                CurrentTool = Brush;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(tool), tool, null);
        }
    }
}
