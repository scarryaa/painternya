using System;
using System.Collections.Generic;
using painternya.Services;

namespace painternya.Models;

public class Layer
{
    public TileManager TileManager { get; private set; }
    public string Name { get; set; }
    public bool IsVisible { get; set; }

    public Layer(string name, int width, int height)
    {
        TileManager = new TileManager(width, height);
        Name = name;
    }
}