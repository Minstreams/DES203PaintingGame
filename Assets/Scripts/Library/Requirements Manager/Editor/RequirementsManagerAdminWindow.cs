using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace GameSystem.Requirements
{
    public class RequirementsManagerAdminWindow : EditorWindow
    {
        static RequirementsManager Manager => RequirementsManager.activeManager;
        static Requirement SelectedRequirement => RequirementsManager.activeManager?.selectedReq;
        static RequirementsManagerData Data => RequirementsManager.Data;

        Vector2 scrollPos;

        void OnGUI()
        {
            if (GUILayout.Button("Editor Setting"))
            {
                Selection.activeObject = Data;
            }
            if (GUILayout.Button("See Local Data"))
            {
                Selection.activeObject = RequirementsManager.LocalData;
            }
            if (Manager == null) return;
            if (GUILayout.Button("New"))
            {
                Manager.NewRequirement();
            }
            if (GUILayout.Button("Refresh Manually"))
            {
                Manager.RefreshList();
            }
            if (SelectedRequirement == null) return;
            {
                if (GUILayout.Button("Delete"))
                {
                    if (Data.requirementList.Contains(SelectedRequirement))
                    {
                        Data.requirementList.Remove(SelectedRequirement);
                        Manager.RefreshList();
                        return;
                    }
                }

                scrollPos = GUILayout.BeginScrollView(scrollPos);
                Undo.RecordObject(Data, "Edit Requirements Data");
                EditorGUI.BeginChangeCheck();
                GUILayout.Label("Name:");
                SelectedRequirement.name = EditorGUILayout.TextField(SelectedRequirement.name);
                GUILayout.Label("Description:");
                SelectedRequirement.description = EditorGUILayout.TextArea(SelectedRequirement.description);
                GUILayout.Label("Path:");
                var oldPath = SelectedRequirement.path;
                SelectedRequirement.path = EditorGUILayout.TextField(SelectedRequirement.path);
                GUILayout.Label("Priority:");
                SelectedRequirement.priority = (RequirementPriority)EditorGUILayout.EnumPopup(SelectedRequirement.priority);
                GUILayout.Label("Status:");
                SelectedRequirement.status = (RequirementStatus)EditorGUILayout.EnumPopup(SelectedRequirement.status);
                GUILayout.Label("Responsible Person:");
                SelectedRequirement.responsiblePerson = EditorGUILayout.TextField(SelectedRequirement.responsiblePerson);
                GUILayout.Label("Comment:");
                SelectedRequirement.comment = EditorGUILayout.TextArea(SelectedRequirement.comment);
                GUILayout.Label("Feedback:");
                SelectedRequirement.feedback = EditorGUILayout.TextArea(SelectedRequirement.feedback);
                if (EditorGUI.EndChangeCheck())
                {
                    Manager.UpdateSelectedTimestamp(oldPath);
                    SelectedRequirement.UpdateTimestamp();
                    Manager.Repaint();
                }
                GUILayout.EndScrollView();
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