using UnityEngine;
using UnityEditor;
using GameSystem.Setting;

[CustomEditor(typeof(TheMatrixSetting))]
public class TheMatrixSettingEditor : Editor
{
    const string toggleStyle = "BoldToggle";

    TheMatrixSetting Setting => target as TheMatrixSetting;
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("Set Frame Rate")) Application.targetFrameRate = Setting.targetFrameRate;

        EditorGUI.BeginChangeCheck();
        GUILayout.BeginVertical("NotificationBackground");
        GUILayout.Label("Debug Options", "SettingsHeader");
        Setting.fullTest = GUILayout.Toggle(Setting.fullTest, "Full Test\t进行完整测试", toggleStyle);
        Setting.saveData = GUILayout.Toggle(Setting.saveData, "Save Data\t测试文件保存", toggleStyle);
        Setting.debug = GUILayout.Toggle(Setting.debug, "Debug Log", toggleStyle);
        GUILayout.EndVertical();
        if (EditorGUI.EndChangeCheck()) EditorUtility.SetDirty(target);
    }
}
