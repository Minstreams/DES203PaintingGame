using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(SeparatorAttribute))]
public class SeparatorDrawer : DecoratorDrawer
{
    SeparatorAttribute Attr => attribute as SeparatorAttribute;

    static readonly GUIStyle style = "ProfilerDetailViewBackground";

    public override void OnGUI(Rect position)
    {
        const float overflow = 10;
        GUI.Label(new Rect(position.x - overflow, position.y+Attr.Space, position.width + overflow * 2, position.height - Attr.Space), GUIContent.none, style);
    }
    public override float GetHeight()
    {
        float height = style.CalcSize(GUIContent.none).y + Attr.Space;
        return height;
    }
}
