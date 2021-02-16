using UnityEngine;
using UnityEditor;
using GameSystem;

[CustomEditor(typeof(TheMatrix))]
public class TheMatrixEditor : Editor
{
    public override void OnInspectorGUI()
    {
        GUILayout.Space(8);
        GUILayout.Label("The Matrix 母体系统", "WarningOverlay");
        GUILayout.Space(8);
        GUILayout.Label("By Minstreams. The Matrix组件的核心，只能有一个。", "MeTimeLabel");
    }
}
