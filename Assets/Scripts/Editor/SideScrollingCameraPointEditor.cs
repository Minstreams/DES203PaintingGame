using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SideScrollingCameraPoint))]
public class SideScrollingCameraPointEditor : Editor
{
    SideScrollingCameraPoint Point => target as SideScrollingCameraPoint;

    void OnSceneGUI()
    {
        var zone = Point.GetComponentInParent<SideScrollingZone>();
        if (zone == null)
        {
            GUIContent warning = new GUIContent("Error: This component must be in a side scrolling zone!");
            GUIStyle style = new GUIStyle("window");
            Handles.BeginGUI();
            {
                GUILayout.BeginArea(HandleUtility.WorldPointToSizedRect(Point.transform.position, warning, style), warning, style);
                {
                    if(GUILayout.Button("Destroy this component."))
                    {
                        Editor.DestroyImmediate(Point);
                    }
                }
                GUILayout.EndArea();
            }
            Handles.EndGUI();

        }
        else
        {
            SideScrollingZoneEditor.CurrentEditingPoint = Point;
            Selection.activeObject = zone;
        }
    }

    Tool lastTool = Tool.None;  // used to hide the default handler

    void OnEnable()
    {
        lastTool = Tools.current;
        Tools.current = Tool.Custom;
    }
    void OnDisable()
    {
        Tools.current = lastTool;
    }
}
