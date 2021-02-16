using System;
using UnityEditor;
using UnityEngine;

public sealed class GUIStyleExampleWindow : EditorWindow
{
    readonly string[] dList =
    {
        "box",
        "button",
        "label",
        "scrollView",
        "textArea",
        "textField",
        "toggle",
        "window",
        "horizontalScrollbar",
        "horizontalScrollbarThumb",
        "horizontalScrollbarLeftButton",
        "horizontalScrollbarRightButton",
        "verticalScrollbar",
        "verticalScrollbarThumb",
        "verticalScrollbarUpButton",
        "verticalScrollbarDownButton",
        "horizontalSlider",
        "horizontalSliderThumb",
        "verticalSlider",
        "verticalSliderThumb"
    };
    GUISkin skin;
    GUIStyle[] sList = null;
    int length = 0;
    public void Init(bool inGameSkin)
    {
        skin = EditorGUIUtility.GetBuiltinSkin(inGameSkin ? EditorSkin.Game : EditorSkin.Scene);
        sList = skin.customStyles;
        length = dList.Length + sList.Length;
    }

    Vector2 mScrollPos;

    [MenuItem("MatrixTool/GUIStyle 样例窗口")]
    static void Example()
    {
        var w = GetWindow<GUIStyleExampleWindow>();
        w.wantsMouseEnterLeaveWindow = false;
        w.wantsMouseMove = false;
        w.autoRepaintOnSceneChange = false;
        w.titleContent = new GUIContent("GUIStyle 样例窗口");
        w.Init(false);
    }
    [MenuItem("MatrixTool/GUIStyle 样例窗口 - InGameSkin")]
    static void Example_InGameSkin()
    {
        var w = GetWindow<GUIStyleExampleWindow>();
        w.wantsMouseEnterLeaveWindow = false;
        w.wantsMouseMove = false;
        w.autoRepaintOnSceneChange = false;
        w.titleContent = new GUIContent("GUIStyle 样例窗口 - InGameSkin");
        w.Init(true);
    }

    int page = 0;
    int itemsPerPage = 30;
    string text = "Test测试";
    bool expandWidth = false;
    bool isButton = false;
    void OnGUI()
    {
        mScrollPos = EditorGUILayout.BeginScrollView(mScrollPos);
        for (int i = page * itemsPerPage; i < length && i < (page + 1) * itemsPerPage; ++i)
        {
            GUIStyle style = i < dList.Length ? skin.GetStyle(dList[i]) : sList[i - dList.Length];
            string name = style.name;
            GUILayout.BeginHorizontal();
            EditorGUILayout.SelectableLabel(name, GUILayout.MaxWidth(250));
            if (isButton) GUILayout.Button(text, style, GUILayout.ExpandWidth(expandWidth));
            else GUILayout.Label(text, style, GUILayout.ExpandWidth(expandWidth));
            GUILayout.EndHorizontal();
            GUILayout.Box(
                string.Empty,
                GUILayout.Width(position.width - 24),
                GUILayout.Height(1)
            );
        }
        EditorGUILayout.EndScrollView();
        GUILayout.BeginVertical("In Footer");
        {
            string stt = "FrameBox";
            GUILayout.BeginHorizontal(stt, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(false));
            {
                expandWidth = GUILayout.Toggle(expandWidth, "Expand Width", GUILayout.MaxWidth(236));
                GUILayout.Label("|", "DefaultCenteredText", GUILayout.ExpandWidth(false));
                itemsPerPage = Math.Max(1, EditorGUILayout.IntField(itemsPerPage, GUILayout.MaxWidth(26)));
                GUILayout.Label("items per page");
            }
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal(stt);
            {
                isButton = GUILayout.Toggle(isButton, "Is Button", GUILayout.MaxWidth(236));
                GUILayout.Label("|", "DefaultCenteredText", GUILayout.ExpandWidth(false));
                text = EditorGUILayout.TextField(text);
            }
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            {
                if (GUILayout.Button("prev", "LargeButton", GUILayout.MaxWidth(164)))
                {
                    page--;
                    if (page < 0) page = 0;
                }
                GUILayout.Label(page.ToString(), "DropzoneStyle");

                if (GUILayout.Button("next", "LargeButton", GUILayout.MaxWidth(164)))
                {
                    page++;
                    while (page * itemsPerPage > length) page--;
                }
            }
            GUILayout.EndHorizontal();
        }
        GUILayout.EndVertical();
    }
}

