using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace GameSystem.Requirements
{
    public class RequirementsManagerInspector : EditorWindow
    {
        static RequirementsManager Manager => RequirementsManager.ActiveManager;
        static Requirement SelectedRequirement => RequirementsManager.ActiveManager?.selectedReq;
        static RequirementsManagerData Data => RequirementsManager.Data;
        static RequirementsManagerLocalData LocalData => RequirementsManager.LocalData;

        Vector2 scrollPos;
        bool edittingComment;

        const string priorityTooltip = @"Priorities:
    【Optional】
    【Normal】
    【Urgent】";
        const string statusTooltip = @"Status：
    【Unfinished】: No file at the expected path.
    【Unchecked】: Wait for Minke to check it.
    【Checked】: The asset is already applied to the game. There is still room for further iterative optimization.
    【Stable】: Final version. No need to change.";
        void OnGUI()
        {
            if (SelectedRequirement == null)
            {
                GUILayout.Label("No requirement selected.", Data.singlelineStyle);
            }
            else
            {
                GUILayout.Space(4);
                GUILayout.BeginHorizontal();
                {
                    GUILayout.Label(SelectedRequirement.name, Data.nameStyle);
                    GUILayout.BeginVertical();
                    {
                        GUILayout.BeginHorizontal();
                        {
                            GUILayout.Label("Priority", Data.miniHeaderStyle, GUILayout.MinWidth(74));
                            GUI.color = Manager.GetPriorityColor(SelectedRequirement);
                            GUILayout.Label(new GUIContent(Manager.GetPriorityText(SelectedRequirement), Data.managerIcon, priorityTooltip), Data.singlelineStyle);
                            GUI.color = Color.white;
                        }
                        GUILayout.EndHorizontal();
                        GUILayout.BeginHorizontal();
                        {
                            GUILayout.Label("Status", Data.miniHeaderStyle, GUILayout.MinWidth(74));
                            GUI.color = Manager.GetStatusColor(SelectedRequirement);
                            GUILayout.Label(new GUIContent(
                                Manager.GetStatusText(SelectedRequirement),
                                Manager.GetStatusTex(SelectedRequirement),
                                statusTooltip),
                                Data.singlelineStyle);
                            GUI.color = Color.white;
                        }
                        GUILayout.EndHorizontal();
                    }
                    GUILayout.EndVertical();
                }
                GUILayout.EndHorizontal();

                GUILayout.Box(GUIContent.none, "ProfilerDetailViewBackground");
                GUILayout.Space(-12);

                scrollPos = GUILayout.BeginScrollView(scrollPos);
                {
                    GUILayout.Label("Description", Data.headerStyle);
                    GUILayout.Label(SelectedRequirement.description, Data.multilineStyle);

                    GUILayout.Label("Comment", Data.headerStyle);
                    if (edittingComment)
                    {
                        Undo.RecordObject(Data, "Edit Requirement Comment");
                        EditorGUI.BeginChangeCheck();
                        SelectedRequirement.comment = EditorGUILayout.TextArea(SelectedRequirement.comment, Data.multilineAreaStyle);
                        if (EditorGUI.EndChangeCheck())
                        {
                            SelectedRequirement.UpdateTimestamp();
                            Manager.UpdateSelectedTimestamp();
                            EditorUtility.SetDirty(Data);
                        }
                    }
                    else
                    {
                        if (GUILayout.Button(SelectedRequirement.comment, Data.multilineAreaStyle))
                        {
                            edittingComment = true;
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(SelectedRequirement.feedback))
                    {
                        GUILayout.Label("Feedback", Data.headerStyle);
                        GUILayout.Label(SelectedRequirement.feedback, Data.multilineStyle);
                    }
                }
                GUILayout.EndScrollView();

                GUILayout.FlexibleSpace();
                GUILayout.Box(GUIContent.none, "ProfilerDetailViewBackground");
                GUILayout.Space(-12);

                GUILayout.BeginHorizontal();
                {
                    if (GUILayout.Button("Navigate", Data.miniButtonSytle))
                    {
                        edittingComment = false;
                        string path = "Assets" + SelectedRequirement.path;
                        if (!path.Contains("/"))
                        {
                            ShowNotification(new GUIContent("Invalid Path"));
                        }
                        else if (System.IO.File.Exists(path))
                        {
                            EditorUtility.FocusProjectWindow();
                            Selection.activeObject = AssetDatabase.LoadAssetAtPath(path, typeof(object));
                        }
                        else
                        {
                            var folder = path.Substring(0, path.LastIndexOf('/'));
                            if (AssetDatabase.IsValidFolder(folder))
                            {
                                EditorUtility.FocusProjectWindow();
                                Selection.activeObject = AssetDatabase.LoadAssetAtPath(folder, typeof(DefaultAsset));
                            }
                            else
                            {
                                ShowNotification(new GUIContent("Invalid Path"));
                            }
                        }
                    };
                    GUILayout.Label("Path", Data.miniHeaderStyle);
                    GUILayout.Label(SelectedRequirement.path, Data.singlelineStyle);
                }
                GUILayout.EndHorizontal();

                GUILayout.Space(2);
                GUILayout.BeginHorizontal();
                {
                    if (GUILayout.Button("Volunteer", Data.miniButtonSytle))
                    {
                        edittingComment = false;
                        if (SelectedRequirement.responsiblePerson != LocalData.localName)
                        {
                            Undo.RecordObject(Data, "Volunteer to be responsible person");
                            SelectedRequirement.responsiblePerson = LocalData.localName;
                            ShowNotification(new GUIContent("Success. Ctrl + Z to undo."));
                            Manager.RefreshFilters();
                            SelectedRequirement.UpdateTimestamp();
                            Manager.UpdateSelectedTimestamp();
                            Manager.Repaint();
                            EditorUtility.SetDirty(Data);
                        }
                    }
                    GUILayout.Label("Responsible Person", Data.miniHeaderStyle);
                    GUILayout.Label(string.IsNullOrWhiteSpace(SelectedRequirement.responsiblePerson) ? "NOBODY" : SelectedRequirement.responsiblePerson, Data.singlelineStyle);
                }
                GUILayout.EndHorizontal();
                GUILayout.Space(4);
            }
        }

        void OnEnable()
        {
            autoRepaintOnSceneChange = true;
        }
    }
}