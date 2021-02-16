using UnityEditor;
using GameSystem;
using UnityEngine;

[CustomPropertyDrawer(typeof(SceneCodeMap), true)]
public class GameSceneMapDrawer : EnumMapDrawer<SceneCode>
{
    readonly int[] selectedIndex = new int[System.Enum.GetNames(typeof(SceneCode)).Length];
    readonly bool[] initialized = new bool[System.Enum.GetNames(typeof(SceneCode)).Length];
    public override void OnItemGUI(Rect propRect, SerializedProperty item, GUIContent label, int index)
    {
        var scenes = SceneSystemEditor.GetScenesInSceneFolder();
        if (scenes.Length == 0)
        {
            EditorGUI.Popup(propRect, label.text, 0, new string[] { "Empty…" });
            return;
        }
        if (!initialized[index])
        {
            bool match = false;
            for (int i = 0; i < scenes.Length; ++i)
            {
                if (scenes[i] == item.stringValue)
                {
                    selectedIndex[index] = i;
                    match = true;
                    break;
                }
            }
            if (!match) item.stringValue = scenes[0];
            initialized[index] = true;
        }
        EditorGUI.BeginChangeCheck();
        selectedIndex[index] = EditorGUI.Popup(propRect, label.text, selectedIndex[index], scenes);
        if (EditorGUI.EndChangeCheck()) item.stringValue = scenes[selectedIndex[index]];
    }
}