using UnityEngine;
using UnityEditor;
using GameSystem.Setting;

[CustomEditor(typeof(TheMatrixSetting))]
public class TheMatrixSettingEditor : Editor
{
    TheMatrixSetting Setting => target as TheMatrixSetting;
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("Set Frame Rate")) Application.targetFrameRate = Setting.targetFrameRate;
    }
}
