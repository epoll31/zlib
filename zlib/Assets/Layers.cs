using System;
using System.Collections.Generic;
using zlib.UI;

namespace zlib;

public static class SortingLayers
{
    public static Dictionary<string, float> Layers { get; set; }

    static SortingLayers()
    {
        Layers = new Dictionary<string, float>
        {
            { "None", 0 },
            { "Game", 0.1f },
            { "UI", 0.2f },
        };
    }

    public static float Get(string layer, float detail = 0)
    {
        return Layers[layer] + detail;
    }

    public static float Get(Element e)
    {
        float depth = Layers["UI"];
        Element parent = e.Parent;
        while (parent != null)
        {
            depth += 0.001f;
            parent = parent.Parent;
        }
        return depth;
    }
}
