using UnityEngine;
using UnityEditor;
using GameSystem.UI;

[CustomEditor(typeof(UIBase), true), CanEditMultipleObjects]
public class UIBaseEditor : Editor
{
    static string styleName = "box";
    UIBase UI => (target as UIBase);
    public override void OnInspectorGUI()
    {
        UI.UIAction();
        GUILayout.BeginHorizontal();
        styleName = GUILayout.TextField(styleName);
        if (GUILayout.Button("Set Box Style"))
        {
            foreach (var ui in targets)
            {
                (ui as UIBase).boxStyle = new GUIStyle(EditorGUIUtility.GetBuiltinSkin(EditorSkin.Game).GetStyle(styleName));
            }
        }
        GUILayout.EndHorizontal();
        base.OnInspectorGUI();
    }
}
