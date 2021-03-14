using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace GameSystem.Requirements
{
    public class RequirementsManagerAdminWindow : EditorWindow
    {
        static RequirementsManager Manager => RequirementsManager.ActiveManager;
        static Requirement SelectedRequirement => RequirementsManager.ActiveManager?.selectedReq;
        static RequirementsManagerData Data => RequirementsManager.Data;

        const float margin = 4;

        Vector2 scrollPos;
        string currentPath = "/";

        void OnSelectionChange()
        {
            var path = AssetDatabase.GetAssetPath(Selection.activeObject);
            if (path.StartsWith("Assets/")) currentPath = path.Substring(6);
            if (!AssetDatabase.IsValidFolder(path)) currentPath = currentPath.Substring(0, currentPath.LastIndexOf("/"));
            Repaint();
        }

        void OnGUI()
        {
            GUILayout.Label("Admin Terminal", Data.nameStyle);
            if (Manager != null)
            {
                GUILayout.BeginHorizontal();
                {
                    GUILayout.Box("Req Manager Control", Data.headerStyle);
                    if (GUILayout.Button("New", Data.miniButtonSytle))
                    {
                        Manager.NewRequirement();
                    }
                    if (GUILayout.Button("Refresh", Data.miniButtonSytle))
                    {
                        Manager.RefreshFilters();
                        Manager.RefreshList();
                        Manager.Repaint();
                    }
                }
                GUILayout.EndHorizontal();

                GUILayout.Space(2);
                GUILayout.Box(GUIContent.none, "ProfilerDetailViewBackground");
                GUILayout.Space(-12);

                if (SelectedRequirement != null)
                {
                    scrollPos = GUILayout.BeginScrollView(scrollPos, GUILayout.ExpandHeight(true));
                    Undo.RecordObject(Data, "Edit Requirements Data");
                    EditorGUI.BeginChangeCheck();

                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Name", Data.miniHeaderStyle);
                    SelectedRequirement.name = EditorGUILayout.TextField(SelectedRequirement.name);
                    GUILayout.EndHorizontal();
                    GUILayout.Space(margin);

                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Description", Data.miniHeaderStyle);
                    SelectedRequirement.description = EditorGUILayout.TextArea(SelectedRequirement.description, Data.multilineAreaStyle);
                    GUILayout.EndHorizontal();
                    GUILayout.Space(margin);

                    GUILayout.BeginHorizontal();
                    GUILayout.Space(11);
                    if (GUILayout.Button("Paste Current Path", GUILayout.ExpandWidth(false)))
                    {
                        SelectedRequirement.path = currentPath + "/";
                    }
                    GUILayout.Label(currentPath);
                    GUILayout.EndHorizontal();
                    GUILayout.Space(margin);

                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Path", Data.miniHeaderStyle);
                    var oldPath = SelectedRequirement.path;
                    SelectedRequirement.path = EditorGUILayout.TextField(SelectedRequirement.path);
                    GUILayout.EndHorizontal();
                    GUILayout.Space(margin);

                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Priority", Data.miniHeaderStyle);
                    SelectedRequirement.priority = (RequirementPriority)EditorGUILayout.EnumPopup(SelectedRequirement.priority);
                    GUILayout.EndHorizontal();
                    GUILayout.Space(margin);

                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Status", Data.miniHeaderStyle);
                    SelectedRequirement.status = (RequirementStatus)EditorGUILayout.EnumPopup(SelectedRequirement.status);
                    GUILayout.EndHorizontal();
                    GUILayout.Space(margin);

                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Responsible Person", Data.miniHeaderStyle);
                    SelectedRequirement.responsiblePerson = EditorGUILayout.TextField(SelectedRequirement.responsiblePerson);
                    GUILayout.EndHorizontal();
                    GUILayout.Space(margin);

                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Comment", Data.miniHeaderStyle);
                    SelectedRequirement.comment = EditorGUILayout.TextArea(SelectedRequirement.comment, Data.multilineAreaStyle);
                    GUILayout.EndHorizontal();
                    GUILayout.Space(margin);

                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Feedback", Data.miniHeaderStyle);
                    SelectedRequirement.feedback = EditorGUILayout.TextArea(SelectedRequirement.feedback, Data.multilineAreaStyle);
                    GUILayout.EndHorizontal();

                    if (EditorGUI.EndChangeCheck())
                    {
                        Manager.UpdateSelectedTimestamp(oldPath);
                        SelectedRequirement.UpdateTimestamp();
                        Manager.RefreshFilters();
                        Manager.Repaint();
                    }
                    GUILayout.EndScrollView();
                }
                else
                {
                    GUILayout.FlexibleSpace();
                }

                GUILayout.BeginVertical("GroupBox");
                GUILayout.Label("Danger Zone", Data.headerStyle);

                if (GUILayout.Button("Editor Setting"))
                {
                    Selection.activeObject = Data;
                }
                if (GUILayout.Button("Local Data Setting"))
                {
                    Selection.activeObject = RequirementsManager.LocalData;
                }
                if (SelectedRequirement != null)
                {
                    if (GUILayout.Button("Delete"))
                    {
                        if (Data.requirementList.Contains(SelectedRequirement))
                        {
                            Data.requirementList.Remove(SelectedRequirement);
                            Manager.RefreshList();
                            Manager.Repaint();
                            return;
                        }
                    }
                }
                GUILayout.EndVertical();
            }
            else
            {
                GUILayout.Label("Press F1 to open Requirements Manager", Data.headerStyle);
            }
        }

        #region Static Window Functions
        [MenuItem("MatrixTool/Requirements Manager Admin", false, 94)]
        static void OpenWindow()
        {
            GetWindow<RequirementsManagerAdminWindow>("Requirements Manager Admin");
        }
        #endregion
    }
}