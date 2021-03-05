using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(MinsHeaderAttribute))]
public class MinsHeaderDrawer : DecoratorDrawer
{
    MinsHeaderAttribute Attr => attribute as MinsHeaderAttribute;
    float width = 512;
    GUIStyle style = null;
    GUIStyle Style
    {
        get
        {
            if (style == null)
            {
                style = new GUIStyle(Attr.Style);
                if (Attr.Style.StartsWith("flow"))
                {
                    style.fontSize = 14;
                    style.fontStyle = FontStyle.Bold;
                    style.padding = new RectOffset(0, 0, 8, 8);
                    style.contentOffset = Vector2.zero;
                }
                else if (Attr.Style == "MeTimeLabel")
                {
                    style.wordWrap = true;
                }
                else if (Attr.Style == "LODRendererRemove")
                {
                    const int space = 8;
                    style.padding = new RectOffset(0, 0, space, 1);
                    style.overflow = new RectOffset(0, 0, -space, -1);
                }
            }
            return style;
        }
    }
    public override float GetHeight()
    {
        float height = Style.CalcHeight(new GUIContent(Attr.Summary), width) + 1;
        return height;
    }
    public override void OnGUI(Rect position)
    {
        if (Event.current.type == EventType.Repaint && width != position.width) width = position.width;
        GUIContent summary = new GUIContent(Attr.Summary);
        var h = Style.CalcHeight(summary, width);
        Rect headerRect = new Rect(position.x, position.y + 1, position.width, h);
        EditorGUI.LabelField(headerRect, summary, Style);
    }
}
