using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using GameSystem;
using System.IO;
using GameSystem.Setting;

public class GameEditorExtension : EditorWindow
{
    const string SubSystemRootPath = "Assets";
    const string SubSystemPath = SubSystemRootPath + "/SubSystem";
    const string DialogTitle = "Minstreams工具箱";
    public const string SystemScene = "Assets/TheMatrix/Scene/System.unity";


    // 这里是一些编辑器方法
    [MenuItem("MatrixTool/Open Tool Window 打开工具箱 #F1", false, 0)]
    public static void OpenToolWindow()
    {
        var comfirmWindow = EditorWindow.GetWindow<GameEditorExtension>(DialogTitle);
    }
    /// <summary>
    /// 导航到系统配置文件
    /// </summary>
    [MenuItem("MatrixTool/System Setting 系统配置 _F2", false, 1)]
    public static void NavToSystemConfig()
    {
        Selection.activeObject = TheMatrix.Setting;
    }
    /// <summary>
    /// 测试当前场景
    /// </summary>
    [MenuItem("MatrixTool/Debug Current Scene 测试当前场景 #F5", false, 2)]
    public static void DebugCurrent()
    {
        if (EditorApplication.isPlaying)
        {
            EditorApplication.isPlaying = false;
            return;
        }
        EditorSceneManager.SaveOpenScenes();
        var sysScene = EditorSceneManager.OpenScene(SystemScene, OpenSceneMode.Additive);

        TheMatrix.EditorSetting.fullTest = false;
        EditorUtility.SetDirty(TheMatrix.EditorSetting);

        EditorApplication.isPlaying = true;
    }
    /// <summary>
    /// 测试全部场景
    /// </summary>
    [MenuItem("MatrixTool/Debug All Scenea 测试全部场景 _F5", false, 3)]
    public static void DebugAll()
    {
        if (EditorApplication.isPlaying)
        {
            EditorApplication.isPlaying = false;
            return;
        }
        EditorSceneManager.SaveOpenScenes();
        EditorSceneManager.OpenScene(SystemScene, OpenSceneMode.Single);

        TheMatrix.EditorSetting.fullTest = true;
        EditorUtility.SetDirty(TheMatrix.EditorSetting);

        var scenes = EditorBuildSettings.scenes;
        for (int i = 1; i < scenes.Length; ++i)
        {
            EditorSceneManager.OpenScene(scenes[i].path, OpenSceneMode.AdditiveWithoutLoading);
        }
        EditorApplication.isPlaying = true;
    }
    /// <summary>
    /// 添加子系统
    /// </summary>
    public void AddSubSystem(string name, string comment)
    {
        if (!wordReg.IsMatch(name))
        {
            EditorUtility.DisplayDialog(DialogTitle, "Invalid Name", "~");
            return;
        }
        if (string.IsNullOrWhiteSpace(comment))
        {
            EditorUtility.DisplayDialog(DialogTitle, "Please write some comment!", "Ok~");
            return;
        }
        if (!name.EndsWith("System")) name += "System";
        var sysPath = SubSystemPath + "/" + name;
        if (AssetDatabase.IsValidFolder(sysPath))
        {
            EditorUtility.DisplayDialog(DialogTitle, name + " already Exists!", "Oh~");
            return;
        }
        if (!AssetDatabase.IsValidFolder(SubSystemPath)) AssetDatabase.CreateFolder(SubSystemRootPath, "SubSystem");
        AssetDatabase.CreateFolder(SubSystemPath, name);
        // Setting-------------------------------
        var fSetting = File.CreateText(sysPath + "/" + name + "Setting.cs");
        fSetting.Write(
@"using UnityEngine;

namespace GameSystem.Setting
{
    [CreateAssetMenu(fileName = " + "\"" + name + "Setting\", menuName = \"系统配置文件/" + name + "Setting\"" + @")]
    public class " + name + @"Setting : ScriptableObject
    {
        //[MinsHeader(" + "\"" + name + " Setting\", SummaryType.Title, -2)]" + @"
        //[MinsHeader(" + "\"" + comment + "\", SummaryType.CommentCenter, -1)]" + @"

        //[MinsHeader(" + "\"Data\", SummaryType.Header)]" + @"

    }
}");
        fSetting.Close();

        var fSystem = File.CreateText(sysPath + "/" + name + ".cs");
        fSystem.Write(
@"using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSystem.Setting;

namespace GameSystem
{
    /// <summary>
    /// " + comment + @"
    /// </summary>
    public class " + name + @" : SubSystem<" + name + @"Setting>
    {
        // Your code here


        [RuntimeInitializeOnLoadMethod]
        static void RuntimeInit()
        {
            TheMatrix.OnGameStart += OnGameStart;
        }
        static void OnGameStart()
        {
            // 在System场景加载后调用
        }


        // API---------------------------------
        //public static void SomeFunction(){}
    }
}
");
        fSystem.Close();

        Selection.activeObject = AssetDatabase.LoadAssetAtPath<Object>(sysPath);
        AssetDatabase.Refresh();

        EditorUtility.DisplayDialog(DialogTitle, name + " created!", "Cool~");
    }
    public void CreateSettingAsset()
    {
        if (string.IsNullOrWhiteSpace(subSystemName)) return;
        if (!subSystemName.EndsWith("System")) subSystemName += "System";
        var sysPath = SubSystemPath + "/" + subSystemName;
        var settingPath = sysPath + "/Resources";
        var settingAssetPath = settingPath + "/" + subSystemName + "Setting.asset";
        if (!AssetDatabase.IsValidFolder(sysPath))
        {
            EditorUtility.DisplayDialog(DialogTitle, subSystemName + " doesn't exist!", "Oh");
            return;
        }
        if (!AssetDatabase.IsValidFolder(settingPath))
        {
            AssetDatabase.CreateFolder(sysPath, "Resources");
        }
        if (File.Exists(settingPath + "/" + subSystemName + "Setting.asset"))
        {
            EditorUtility.DisplayDialog(DialogTitle, subSystemName + " Setting Asset already exist!", "Oh");
            Selection.activeObject = AssetDatabase.LoadAssetAtPath<Object>(settingAssetPath);
            return;
        }
        var settingAsset = ScriptableObject.CreateInstance(subSystemName + "Setting");
        AssetDatabase.CreateAsset(settingAsset, settingAssetPath);
        Selection.activeObject = settingAsset;

        EditorUtility.DisplayDialog(DialogTitle, subSystemName + "  Setting Asset created!", "Cool~");
    }
    void ScanSubSystem()
    {
        subSystemSettings = AssetDatabase.FindAssets("SystemSetting t:ScriptableObject");
        for (int i = 0; i < subSystemSettings.Length; ++i)
        {
            subSystemSettings[i] = AssetDatabase.GUIDToAssetPath(subSystemSettings[i]);
        }
    }
    /// <summary>
    /// 添加Linker
    /// </summary>
    public static void AddLinker(string name, string title, string comment, string subSystemName = "")
    {
        if (!wordReg.IsMatch(name))
        {
            EditorUtility.DisplayDialog(DialogTitle, "Invalid Name", "~");
            return;
        }
        string path;
        StreamWriter f;
        bool isOfSubSys = !string.IsNullOrWhiteSpace(subSystemName);
        if (isOfSubSys)
        {
            if (!subSystemName.EndsWith("System")) subSystemName += "System";

            var sysPath = SubSystemPath + "/" + subSystemName;
            var linkerPath = sysPath + "/Linker";

            if (!AssetDatabase.IsValidFolder(sysPath))
            {
                EditorUtility.DisplayDialog(DialogTitle, subSystemName + " doesn't exist!", "Oh");
                return;
            }
            if (!AssetDatabase.IsValidFolder(linkerPath)) AssetDatabase.CreateFolder(sysPath, "Linker");
            path = linkerPath + "/" + name + ".cs";
        }
        else
        {
            path = "Assets/Scripts/Linker/" + name + ".cs";
        }
        f = File.CreateText(path);
        f.Write(
@"using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameSystem.Linker
{
    " + (string.IsNullOrEmpty(comment) ? "" : (
    @"/// <summary>
    /// " + comment + @"
    /// </summary>")) + @"
    [AddComponentMenu(" + "\"" + (isOfSubSys ? ("[" + subSystemName + "]/") : "|") + "Linker/" + name + "\"" + @")]
    public class " + name + @" : MonoBehaviour
    {
#if UNITY_EDITOR" + (isOfSubSys ? @"
        [MinsHeader(" + "\"Linker of " + subSystemName + "\", SummaryType.PreTitleLinker, -1)]" : "") + @"
        [MinsHeader(" + "\"" + title + "\"" + @", SummaryType.Title" + (isOfSubSys ? "Blue" : "Cyan") + @", 0)]
        [MinsHeader(" + "\"" + comment + "\"" + @", SummaryType.CommentCenter, 1)]
        [ConditionalShow, SerializeField] bool useless; //在没有数据的时候让标题正常显示
#endif

        // Data
        [MinsHeader(" + "\"Data\"" + @", SummaryType.Header, 2)]

        // Inner code here

        // Output
        [MinsHeader(" + "\"Output\"" + @", SummaryType.Header, 3)]
        public SimpleEvent output;

        // Input
        [ContextMenu(" + "\"Invoke\"" + @")]
        public void Invoke()
        {
            output?.Invoke();
        }
    }
}");
        f.Close();
        AssetDatabase.Refresh();
        Selection.activeObject = AssetDatabase.LoadAssetAtPath<Object>(path);

        EditorUtility.DisplayDialog(DialogTitle, (isOfSubSys ? (subSystemName + "/") : "") + name + " created!", "Cool~");
    }
    /// <summary>
    /// 添加Operator
    /// </summary>
    public static void AddOperator(string name, string title, string comment, string subSystemName = "")
    {
        if (!wordReg.IsMatch(name))
        {
            EditorUtility.DisplayDialog(DialogTitle, "Invalid Name", "~");
            return;
        }
        string path;
        StreamWriter f;
        bool isOfSubSys = !string.IsNullOrWhiteSpace(subSystemName);
        if (isOfSubSys)
        {
            if (!subSystemName.EndsWith("System")) subSystemName += "System";

            var sysPath = SubSystemPath + "/" + subSystemName;
            var operatorPath = sysPath + "/Operator";

            if (!AssetDatabase.IsValidFolder(sysPath))
            {
                EditorUtility.DisplayDialog(DialogTitle, subSystemName + " doesn't exist!", "Oh");
                return;
            }
            if (!AssetDatabase.IsValidFolder(operatorPath)) AssetDatabase.CreateFolder(sysPath, "Operator");

            path = operatorPath + "/" + name + ".cs";
        }
        else
        {
            path = "Assets/Scripts/Operator/" + name + ".cs";
        }
        f = File.CreateText(path);
        f.Write(
@"using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameSystem.Operator
{
    " + (string.IsNullOrEmpty(comment) ? "" : (
    @"/// <summary>
    /// " + comment + @"
    /// </summary>")) + @"
    [AddComponentMenu(" + "\"" + (isOfSubSys ? ("[" + subSystemName + "]/") : "|") + "Operator/" + name + "\"" + @")]
    public class " + name + @" : MonoBehaviour
    {
#if UNITY_EDITOR" + (isOfSubSys ? @"
        [MinsHeader(" + "\"Operator of " + subSystemName + "\", SummaryType.PreTitleOperator, -1)]" : "") + @"
        [MinsHeader(" + "\"" + title + "\"" + @", SummaryType.Title" + (isOfSubSys ? "Orange" : "Yellow") + @", 0)]
        [MinsHeader(" + "\"" + comment + "\"" + @", SummaryType.CommentCenter, 1)]
        [ConditionalShow, SerializeField] bool useless; //在没有数据的时候让标题正常显示
#endif

        // Data
        //[MinsHeader(" + "\"Data\"" + @", SummaryType.Header, 2)]

        // Inner code here

        // Input
        //[ContextMenu(" + "\"SomeFuntion\"" + @")]
        //public void SomeFuntion(){}
    }
}");
        f.Close();
        AssetDatabase.Refresh();
        Selection.activeObject = AssetDatabase.LoadAssetAtPath<Object>(path);

        EditorUtility.DisplayDialog(DialogTitle, (isOfSubSys ? (subSystemName + "/") : "") + name + " created!", "Cool~");
    }
    /// <summary>
    /// 添加Savable
    /// </summary>
    public static void AddSavable(string name, string title, string comment, string subSystemName = "")
    {
        if (!wordReg.IsMatch(name))
        {
            EditorUtility.DisplayDialog(DialogTitle, "Invalid Name", "~");
            return;
        }
        string path;
        StreamWriter f;
        bool isOfSubSys = !string.IsNullOrWhiteSpace(subSystemName);
        if (isOfSubSys)
        {
            if (!subSystemName.EndsWith("System")) subSystemName += "System";

            var sysPath = SubSystemPath + "/" + subSystemName;
            var savablePath = sysPath + "/Savable";

            if (!AssetDatabase.IsValidFolder(sysPath))
            {
                EditorUtility.DisplayDialog(DialogTitle, subSystemName + " doesn't exist!", "Oh");
                return;
            }
            if (!AssetDatabase.IsValidFolder(savablePath)) AssetDatabase.CreateFolder(sysPath, "Savable");
            path = savablePath + "/" + name + ".cs";
        }
        else
        {
            path = "Assets/Scripts/Savable/" + name + ".cs";
        }
        f = File.CreateText(path);
        f.Write(
@"using UnityEngine;

namespace GameSystem.Savable
{
    " + (string.IsNullOrEmpty(comment) ? "" : (
    @"/// <summary>
    /// " + comment + @"
    /// </summary>")) + @"
    [CreateAssetMenu(fileName = " + "\"" + name + "\", menuName = \"Savable/" + name + "\"" + @")]
    public class " + name + @" : SavableObject
    {" + (isOfSubSys ? @"
        [MinsHeader(" + "\"SavableObject of " + subSystemName + "\", SummaryType.PreTitleSavable, -1)]" : "") + @"
        [MinsHeader(" + "\"" + title + "\"" + @", SummaryType.TitleGreen, 0)]
        [MinsHeader(" + "\"" + comment + "\"" + @", SummaryType.CommentCenter, 1)]

        // Your Data here
        [Label] public float data1;

        // APPLY the data to game
        public override void ApplyData()
        {
            //apply(data1);
        }

        // Collect and UPDATE data
        public override void UpdateData()
        {
            //data1 = ...
        }
    }
}");
        f.Close();
        AssetDatabase.Refresh();
        Selection.activeObject = AssetDatabase.LoadAssetAtPath<Object>(path);

        EditorUtility.DisplayDialog(DialogTitle, (isOfSubSys ? (subSystemName + "/") : "") + name + " created!", "Cool~");
    }
    /// <summary>
    /// 添加UI
    /// </summary>
    public static void AddUI(string name, string title, string comment, string subSystemName = "")
    {
        if (!wordReg.IsMatch(name))
        {
            EditorUtility.DisplayDialog(DialogTitle, "Invalid Name", "~");
            return;
        }
        string path;
        StreamWriter f;
        bool isOfSubSys = !string.IsNullOrWhiteSpace(subSystemName);
        if (isOfSubSys)
        {
            if (!subSystemName.EndsWith("System")) subSystemName += "System";

            var sysPath = SubSystemPath + "/" + subSystemName;
            var savablePath = sysPath + "/UI";

            if (!AssetDatabase.IsValidFolder(sysPath))
            {
                EditorUtility.DisplayDialog(DialogTitle, subSystemName + " doesn't exist!", "Oh");
                return;
            }
            if (!AssetDatabase.IsValidFolder(savablePath)) AssetDatabase.CreateFolder(sysPath, "UI");
            path = savablePath + "/" + name + ".cs";
        }
        else
        {
            path = "Assets/Scripts/UI/" + name + ".cs";
        }
        f = File.CreateText(path);
        f.Write(
@"using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameSystem.UI
{
    " + (string.IsNullOrEmpty(comment) ? "" : (
    @"/// <summary>
    /// " + comment + @"
    /// </summary>")) + @"
    [AddComponentMenu(" + "\"" + (isOfSubSys ? ("[" + subSystemName + "]/") : "|") + "UI/" + name + "\"" + @")]
    public class " + name + @" : UIBase
    {
#if UNITY_EDITOR" + (isOfSubSys ? @"
        [MinsHeader(" + "\"" + title + " UI of " + subSystemName + "\", SummaryType.PreTitleOperator, -2)]" : "") + @"
        [MinsHeader(" + "\"" + comment + "\"" + @", SummaryType.CommentCenter, -1)]
        [ConditionalShow, SerializeField] bool useless; //在没有数据的时候让标题正常显示
#endif

        protected override void OnUI()
        {
            GUILayout.Label(" + "\"" + title + "\"" + @");
        }
}
}");
        f.Close();
        AssetDatabase.Refresh();
        Selection.activeObject = AssetDatabase.LoadAssetAtPath<Object>(path);

        EditorUtility.DisplayDialog(DialogTitle, (isOfSubSys ? (subSystemName + "/") : "") + name + " created!", "Cool~");
    }

    static System.Text.RegularExpressions.Regex wordReg = new System.Text.RegularExpressions.Regex("^\\w+$");
    string[] subSystemSettings = null;
    public enum EditorMode
    {
        SubSystem,
        Linker,
        Operator,
        Savable,
        UI,
    }
    EditorMode editorMode;
    string subSystemName = "";
    string subSystemComment = "";
    string linkerName = "";
    string linkerTitle = "";
    string linkerComment = "";
    string operatorName = "";
    string operatorTitle = "";
    string operatorComment = "";
    string savableName = "";
    string savableTitle = "";
    string savableComment = "";
    string uiName = "";
    string uiTitle = "";
    string uiComment = "";

    GUIStyle headerStyle;
    GUIStyle HeaderStyle
    {
        get
        {
            if (headerStyle == null)
            {
                headerStyle = new GUIStyle("ProfilerBadge")
                {
                    alignment = TextAnchor.MiddleCenter,
                    fontSize = 18,
                    fixedHeight = 32,
                    margin = new RectOffset(2, 2, 2, 2)
                };
            }
            return headerStyle;
        }
    }
    GUIStyle btnStyle;
    GUIStyle BtnStyle
    {
        get
        {
            if (btnStyle == null)
            {
                btnStyle = new GUIStyle("toolbarbutton")
                {
                    alignment = TextAnchor.MiddleCenter,
                    fixedHeight = 20,
                    margin = new RectOffset(4, 4, 4, 4)
                };
            }
            return btnStyle;
        }
    }
    GUIStyle labelStyle;
    GUIStyle LabelStyle
    {
        get
        {
            if (labelStyle == null)
            {
                labelStyle = new GUIStyle("ColorPickerBackground")
                {
                    padding = new RectOffset(8, 8, 4, 4)
                };
                labelStyle.normal.textColor = new Color(0.6f, 0.6f, 0.6f);
            }
            return labelStyle;
        }
    }
    GUIStyle tabBGStyle;
    GUIStyle TabBGStyle
    {
        get
        {
            if (tabBGStyle == null)
            {
                tabBGStyle = new GUIStyle("IN ThumbnailShadow")
                {
                    fixedHeight = 0
                };
            }
            return tabBGStyle;
        }
    }
    GUIStyle tabStyle;
    GUIStyle TabStyle
    {
        get
        {
            if (tabStyle == null)
            {
                tabStyle = new GUIStyle("PreLabel")
                {
                    padding = TabLabelStyle.padding,
                    stretchHeight = false,
                    alignment = TextAnchor.MiddleCenter,
                    contentOffset = Vector2.zero,
                    margin = new RectOffset(2, 2, 2, 2),
                    fontStyle = FontStyle.Normal
                };
                tabStyle.normal.textColor = Color.black;
            }
            return tabStyle;
        }
    }
    GUIStyle tabLabelStyle;
    GUIStyle TabLabelStyle
    {
        get
        {
            if (tabLabelStyle == null)
            {
                tabLabelStyle = new GUIStyle("ShurikenEffectBg")
                {
                    stretchHeight = false,
                    alignment = TextAnchor.MiddleCenter,
                    contentOffset = Vector2.zero,
                    margin = new RectOffset(2, 2, 2, 2),
                    fontStyle = FontStyle.Bold
                };
                tabLabelStyle.normal.textColor = Color.white;
            }
            return tabLabelStyle;
        }
    }
    /// <summary>
    /// 分隔符
    /// </summary>
    void Separator()
    {
        GUILayout.Label("", "RL DragHandle", GUILayout.ExpandWidth(true));
    }
    void SectionHeader(string title)
    {
        Separator();
        GUILayout.Label(title, HeaderStyle, GUILayout.ExpandWidth(true));
    }
    string TextArea(string name, string target, int maxLength = 24)
    {
        string result;
        GUILayout.BeginHorizontal();
        {
            GUILayout.Label(name, "ProfilerSelectedLabel", GUILayout.ExpandWidth(false), GUILayout.Height(EditorGUIUtility.singleLineHeight * 1.2f));
            result = GUILayout.TextField(target, maxLength, "SearchTextField");
        }
        GUILayout.EndHorizontal();
        return result;
    }
    void Label(string text)
    {
        GUILayout.Label(text, LabelStyle);
    }



    void OnEnable()
    {
        ScanSubSystem();
        Input.imeCompositionMode = IMECompositionMode.On;
    }

    void OnGUI()
    {
        GUILayout.BeginVertical("GameViewBackground", GUILayout.ExpandHeight(true));
        SectionHeader("数据导航");
        if (GUILayout.Button("System Setting", BtnStyle)) NavToSystemConfig();
        if (GUILayout.Button("System Setting", BtnStyle)) Selection.activeObject = TheMatrix.EditorSetting;
        if (subSystemSettings != null)
        {
            for (int i = 0; i < subSystemSettings.Length; ++i)
            {
                int start = Mathf.Max(subSystemSettings[i].LastIndexOf('/') + 1, 0);
                string subName = subSystemSettings[i].Substring(start, subSystemSettings[i].Length - start - 19);
                if (GUILayout.Button(subName, BtnStyle))
                {
                    Selection.activeObject = AssetDatabase.LoadAssetAtPath<Object>(subSystemSettings[i]);
                    subSystemName = subName;
                    subSystemComment = "";
                }
            }
        }
        if (GUILayout.Button("刷新", BtnStyle)) ScanSubSystem();

        SectionHeader("自动测试");
        if (GUILayout.Button("测试全部场景", BtnStyle)) DebugAll();
        if (GUILayout.Button("测试当前场景", BtnStyle)) DebugCurrent();

        SectionHeader("自动化代码生成");
        GUILayout.BeginHorizontal(TabBGStyle);
        {
            if (GUILayout.Button("SubSystem", editorMode == EditorMode.SubSystem ? TabLabelStyle : TabStyle)) editorMode = EditorMode.SubSystem;
            if (GUILayout.Button("Linker", editorMode == EditorMode.Linker ? TabLabelStyle : TabStyle)) editorMode = EditorMode.Linker;
            if (GUILayout.Button("Operator", editorMode == EditorMode.Operator ? TabLabelStyle : TabStyle)) editorMode = EditorMode.Operator;
            if (GUILayout.Button("Savable", editorMode == EditorMode.Savable ? TabLabelStyle : TabStyle)) editorMode = EditorMode.Savable;
            if (GUILayout.Button("UI", editorMode == EditorMode.UI ? TabLabelStyle : TabStyle)) editorMode = EditorMode.UI;
        }
        GUILayout.EndHorizontal();
        GUILayout.Space(4);
        switch (editorMode)
        {
            case EditorMode.SubSystem:
                subSystemName = TextArea("Sub System Name", subSystemName);
                subSystemComment = TextArea("Comment", subSystemComment, 64);
                Label("自动生成一个子系统 Sub System");
                Label("由于实在无法把生成代码与生成配置文件功能做到一起，生成新系统时，请依次点这两个按钮。");
                if (GUILayout.Button("Add", BtnStyle)) AddSubSystem(subSystemName, subSystemComment);
                if (GUILayout.Button("Create Setting Asset", BtnStyle)) { CreateSettingAsset(); ScanSubSystem(); }
                break;
            case EditorMode.Linker:
                EditorGUI.BeginChangeCheck();
                linkerName = TextArea("Linker Name", linkerName);
                if (EditorGUI.EndChangeCheck()) linkerTitle = linkerName;
                linkerTitle = TextArea("Linker Title", linkerTitle);
                linkerComment = TextArea("Comment", linkerComment, 64);
                Label("自动生成一个连接节点 Linker，用于连接和处理数据。");
                if (GUILayout.Button("Add", BtnStyle)) AddLinker(linkerName, linkerTitle, linkerComment);
                if (GUILayout.Button("Add To Current SubSystem", BtnStyle))
                {
                    if (string.IsNullOrWhiteSpace(subSystemName)) EditorUtility.DisplayDialog(DialogTitle, "请先在‘数据导航’一栏中选择子系统", "好的");
                    else AddLinker(linkerName, linkerTitle, linkerComment, subSystemName);
                }
                break;
            case EditorMode.Operator:
                EditorGUI.BeginChangeCheck();
                operatorName = TextArea("Operator Name", operatorName);
                if (EditorGUI.EndChangeCheck()) operatorTitle = operatorName;
                operatorTitle = TextArea("Operator Title", operatorTitle);
                operatorComment = TextArea("Comment", operatorComment, 64);
                Label("自动生成一个操作节点 Operator，用于执行具体动作。");
                if (GUILayout.Button("Add", BtnStyle)) AddOperator(operatorName, operatorTitle, operatorComment);
                if (GUILayout.Button("Add To Current SubSystem", BtnStyle))
                {
                    if (string.IsNullOrWhiteSpace(subSystemName)) EditorUtility.DisplayDialog(DialogTitle, "请先在‘数据导航’一栏中选择子系统", "好的");
                    else AddOperator(operatorName, operatorTitle, operatorComment, subSystemName);
                }
                break;
            case EditorMode.Savable:
                EditorGUI.BeginChangeCheck();
                savableName = TextArea("Savable Name", savableName);
                if (EditorGUI.EndChangeCheck()) savableTitle = savableName;
                savableTitle = TextArea("Savable Title", savableTitle);
                savableComment = TextArea("Comment", savableComment, 64);
                Label("自动生成一个可存储对象SavableObject，用于持久化存储数据。");
                if (GUILayout.Button("Add", BtnStyle)) AddSavable(savableName, savableTitle, savableComment);
                if (GUILayout.Button("Add To Current SubSystem", BtnStyle))
                {
                    if (string.IsNullOrWhiteSpace(subSystemName)) EditorUtility.DisplayDialog(DialogTitle, "请先在‘数据导航’一栏中选择子系统", "好的");
                    else AddSavable(savableName, savableTitle, savableComment, subSystemName);
                }
                break;
            case EditorMode.UI:
                EditorGUI.BeginChangeCheck();
                uiName = TextArea("UI Name", uiName);
                if (EditorGUI.EndChangeCheck()) uiTitle = uiName;
                uiTitle = TextArea("UI Title", uiTitle);
                uiComment = TextArea("Comment", uiComment, 64);
                Label("自动生成一个UI组件。");
                if (GUILayout.Button("Add", BtnStyle)) AddUI(uiName, uiTitle, uiComment);
                if (GUILayout.Button("Add To Current SubSystem", BtnStyle))
                {
                    if (string.IsNullOrWhiteSpace(subSystemName)) EditorUtility.DisplayDialog(DialogTitle, "请先在‘数据导航’一栏中选择子系统", "好的");
                    else AddUI(uiName, uiTitle, uiComment, subSystemName);
                }
                break;
        }
        Separator();
        GUILayout.EndVertical();
    }
}