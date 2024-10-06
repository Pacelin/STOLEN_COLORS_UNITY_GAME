using UnityEngine;
using UnityEngine.UI;

namespace UniOwl.UI
{
    public static class GraphicUtils
    {
        public static void SetAlpha(this Graphic graphic, float value)
        {
            Color color = graphic.color;
            color.a = value;
            graphic.color = color;
        }
    }
}