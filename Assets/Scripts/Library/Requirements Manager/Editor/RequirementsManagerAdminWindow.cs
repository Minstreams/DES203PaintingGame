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
            if (Manager == null) return;
            if (GUILayout.Button("New"))
            {
                Manager.NewRequirement();
            }
            if(GUILayout.Button("Refresh Manually"))
            {
                Manager.RefreshList();
            }
            if (SelectedRequirement == null) return;
            {
                scrollPos = GUILayout.BeginScrollView(scrollPos);
                GUILayout.Label("Name:");
                SelectedRequirement.name = GUILayout.TextField(SelectedRequirement.name);
                GUILayout.Label("Description:");
                SelectedRequirement.description = GUILayout.TextArea(SelectedRequirement.description);
                GUILayout.Label("Path:");
                SelectedRequirement.path = GUILayout.TextField(SelectedRequirement.path);
                GUILayout.Label("Priority:");
                SelectedRequirement.priority = (RequirementPriority)EditorGUILayout.EnumPopup(SelectedRequirement.priority);
                GUILayout.Label("Status:");
                SelectedRequirement.status = (RequirementStatus)EditorGUILayout.EnumPopup(SelectedRequirement.status);
                GUILayout.Label("Responsible Person:");
                SelectedRequirement.responsiblePerson = GUILayout.TextField(SelectedRequirement.responsiblePerson);
                GUILayout.Label("Comment:");
                SelectedRequirement.comment = GUILayout.TextArea(SelectedRequirement.comment);
                GUILayout.Label("Feedback:");
                SelectedRequirement.feedback = GUILayout.TextArea(SelectedRequirement.feedback);
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