using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SideScrollingZone))]
public class SideScrollingZoneEditor : Editor
{
    // References
    SideScrollingZone Zone => target as SideScrollingZone;
    Transform transform => Zone.transform;

    // Fields
    Tool lastTool = Tool.None;  // used to hide the default handler
    static float snapValue = 1f;

    void OnEnable()
    {
        lastTool = Tools.current;
        Tools.current = Tool.Custom;
    }
    void OnDisable()
    {
        Tools.current = lastTool;
    }

    // Inspector
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        GUILayout.Space(32);
        GUILayout.Label("Editor Parameters", "LODRendererRemove");
        snapValue = EditorGUILayout.FloatField("Snap Value",snapValue);
    }


    // Useful calculation shortcuts

    /// <summary>
    /// put a vector to ground
    /// </summary>
    Vector3 ToGround(Vector3 val)
    {
        var delta = val - Camera.current.transform.position;
        return val - delta * val.y / delta.y;
    }

    // GUI
    void DrawArrow(Vector3 p1, Vector3 p2, Color color)
    {
        Color c = Handles.color;
        Handles.color = color;

        float arrowSize = 0.1f * HandleUtility.GetHandleSize(p2);
        Handles.DrawLine(p1, p2);
        Handles.ConeHandleCap(-1, p2 + (p1 - p2).normalized * arrowSize, Quaternion.LookRotation((p2 - p1), Vector3.up), arrowSize, EventType.Repaint);

        Handles.color = c;
    }
    void DoSnap()
    {
        Undo.RecordObject(Zone, "Snap Scrolling Zone Point");
        Undo.RecordObject(Zone.transform, "Snap Scrolling Zone Point");
        pos1 = new Vector3(Mathf.Floor(pos1.x / snapValue + 0.5f) * snapValue, 0, Mathf.Floor(pos1.z / snapValue + 0.5f) * snapValue);
        pos2 = new Vector3(Mathf.Floor(pos2.x / snapValue + 0.5f) * snapValue, 0, Mathf.Floor(pos2.z / snapValue + 0.5f) * snapValue);
        ApplyPos();
    }

    Vector3 pos1;
    Vector3 pos2;

    void ApplyPos()
    {
        transform.position = pos1;
        Zone.distance = Vector3.Distance(pos2, transform.position);
        transform.rotation = Quaternion.LookRotation(Vector3.Cross(pos2 - transform.position, Vector3.up), Vector3.up);
    }


    void OnSceneGUI()
    {
        EditorGUI.BeginChangeCheck();
        {
            pos1 = ToGround(Handles.FreeMoveHandle(transform.position, Quaternion.identity, 0.2f, new Vector3(snapValue, 65535, snapValue), PointCapture));
            pos2 = ToGround(Handles.FreeMoveHandle(transform.position + transform.right * Zone.distance, Quaternion.identity, 0.2f, new Vector3(snapValue, 65535, snapValue), PointCapture));
        }
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(Zone, "Move Scrolling Zone Point");
            Undo.RecordObject(Zone.transform, "Move Scrolling Zone Point");
            ApplyPos();
        }
        DrawArrow(pos1, pos2, Color.green);


        if(Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.LeftControl)
        {
            DoSnap();
        }
    }
    void PointCapture(int controlID, Vector3 position, Quaternion rotation, float size, EventType eventType)
    {
        var sizeFactor = HandleUtility.GetHandleSize(position);
        size *= sizeFactor;
        Handles.CircleHandleCap(controlID, position, rotation, size, eventType);
    }



}
