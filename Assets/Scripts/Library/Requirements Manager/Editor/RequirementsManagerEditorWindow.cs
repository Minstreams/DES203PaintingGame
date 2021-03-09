using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace GameSystem.Requirements
{
    public class RequirementsManagerEditorWindow : EditorWindow
    {
        const string titleText = "Requirements Manager";
        const string windowOpenLocker = "RequirementsManagerLocker";

        public static RequirementsManagerData Data => data == null ? data = (RequirementsManagerData)EditorGUIUtility.Load("RequirementsManagerData.asset") : data;
        public static RequirementsManagerData data;

        [InitializeOnLoadMethod]
        static void OpenWindowOnLoad()
        {
            if (SessionState.GetBool(windowOpenLocker, false)) return;
            OpenWindow();
            SessionState.SetBool(windowOpenLocker, true);
        }

        [MenuItem("MatrixTool/Requirements Manager _F1", false, 92)]
        static void OpenWindow()
        {
            GetWindow<RequirementsManagerEditorWindow>(titleText);
        }


        // Fields ==================================================
        Vector2 scrollPos;


        void OnEnable()
        {
            autoRepaintOnSceneChange = true;
        }

        void OnGUI()
        {
            scrollPos = GUILayout.BeginScrollView(scrollPos);

            //foreach (var req in Data.requirementList)
            //{
            //    RequirementField(req);
            //}



            // Debug
            if (GUILayout.Button("Edit Data"))
            {
                Selection.activeObject = Data;
            }
            try
            {
                GUILayout.Label(JsonUtility.ToJson(Selection.activeObject, true), Data.debugLogStyle);
            }
            catch (System.Exception e)
            {
                GUILayout.Label(e.ToString(), Data.debugLogStyle);
                GUILayout.Label(Selection.activeObject.ToString(), Data.debugLogStyle);
            }


            GUILayout.EndScrollView();
        }

        void RequirementField(Requirement req)
        {
            GUILayout.BeginHorizontal(Data.reqBoxStyle);

            GUILayout.EndHorizontal();
        }
    }
}