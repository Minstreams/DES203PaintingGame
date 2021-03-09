using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class RequirementsManagerEditorWindow : EditorWindow
{
    const string titleText = "Requirements Manager";


    // 这里是一些编辑器方法
    [MenuItem("MatrixTool/Requirements Manager _F1", false, 92)]
    static void OpenWindow()
    {
        var comfirmWindow = EditorWindow.GetWindow<RequirementsManagerEditorWindow>(titleText);
    }


}
