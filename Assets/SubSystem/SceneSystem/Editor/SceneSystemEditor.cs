using UnityEngine;
using UnityEditor;
using GameSystem;
using GameSystem.Setting;

[CustomEditor(typeof(SceneSystemSetting))]
public class SceneSystemEditor : Editor
{
    void OnEnable()
    {
        UpdateScenesInSceneFolder();
    }
    public override void OnInspectorGUI()
    {
        EditorGUI.BeginChangeCheck();
        base.OnInspectorGUI();
        if (EditorGUI.EndChangeCheck())
        {
            SetBuildScenes();
        }
        GUILayout.Space(16);
        GUILayout.Label("Scenes in build:", "LODRendererRemove");
        var scenes = EditorBuildSettings.scenes;
        for (int i = 0; i < scenes.Length; ++i)
        {
            GUILayout.BeginHorizontal("TextField");
            GUILayout.Label(scenes[i].path);
            GUILayout.Label(i.ToString(), GUILayout.Width(24));
            GUILayout.EndHorizontal();
        }
        if (GUILayout.Button("SetBuildScenes"))
        {
            SetBuildScenes();
            EditorUtility.DisplayDialog("SceneSystem", "Scenes in build Setted!", "Cool~");
        }
        if (GUILayout.Button("Edit Scene List"))
        {
            Selection.activeObject = AssetDatabase.LoadAssetAtPath("Assets/SubSystem/SceneSystem/SceneCode.cs", typeof(Object));
        }
    }
    static string[] scenesInSceneFolder;
    public static void UpdateScenesInSceneFolder()
    {
        if (!AssetDatabase.IsValidFolder("Assets/Scenes"))
        {
            scenesInSceneFolder = new string[0];
            return;
        }
        scenesInSceneFolder = AssetDatabase.FindAssets("t:Scene", new string[] { "Assets/Scenes" });
        for (int i = 0; i < scenesInSceneFolder.Length; ++i)
        {
            string path = AssetDatabase.GUIDToAssetPath(scenesInSceneFolder[i]);
            int start = path.LastIndexOf('/') + 1;
            int length = path.LastIndexOf('.') - start;
            scenesInSceneFolder[i] = path.Substring(start, length);
        }
    }
    public static string[] GetScenesInSceneFolder()
    {
        return scenesInSceneFolder;
    }
    public static void SetBuildScenes()
    {
        var scenes = new EditorBuildSettingsScene[System.Enum.GetNames(typeof(SceneCode)).Length + 1];
        scenes[0] = new EditorBuildSettingsScene(GameEditorExtension.SystemScene, true);
        for (int i = 1; i < scenes.Length; ++i)
        {
            scenes[i] = new EditorBuildSettingsScene("Assets/Scenes/" + SceneSystem.GetScene((SceneCode)(i - 1)) + ".unity", true);
        }
        EditorBuildSettings.scenes = scenes;
    }
}
