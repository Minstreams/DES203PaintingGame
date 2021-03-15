using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(LabelRangeAttribute))]
public class LabelRangeDrawer : PropertyDrawer
{
    LabelRangeAttribute Attr => attribute as LabelRangeAttribute;
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var tc = GUI.color;

        if (Attr.Const && EditorApplication.isPlaying)
        {
            // 常量在播放时只在Repaint绘制并上灰色
            if (Event.current.type == EventType.Repaint) GUI.color = Color.gray;
            else return;
        }

        // 开始绘制
        label = new GUIContent(string.IsNullOrEmpty(Attr.Label) ? property.displayName : Attr.Label);

        Rect windowRect = new Rect(position.x, position.y + 2, position.width, position.height - 4);
        GUI.Box(windowRect, GUIContent.none, "button");

        Rect propRect = new Rect(position.x + 4, position.y + 6, position.width - 8, position.height - 12);
        EditorGUI.Slider(propRect, property, Attr.Left, Attr.Right, label);

        GUI.color = tc;

        LabelClipBoard.PopupMenu(position, property);
    }
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUI.GetPropertyHeight(property, label) + 12;
    }
}
