using UnityEngine;
using UnityEditor;
using GameSystem;
using GameSystem.Setting;

[CustomEditor(typeof(SideScrollingZone))]
public class SideScrollingZoneEditor : Editor
{
    // References
    SideScrollingZone Zone => target as SideScrollingZone;
    Transform transform => Zone.transform;
    static GameplaySystemSetting Setting => GameplaySystem.Setting;
    static TheMatrixEditorSetting EditorSetting => TheMatrix.EditorSetting;

    // Static Properties
    public static SideScrollingCameraPoint CurrentEditingPoint { private get; set; } = null;
    static SceneView CurrentDrawingSceneView { get; set; } = null;

    // Fields
    Tool lastTool = Tool.None;  // used to hide the default handler
    SideScrollingCameraPoint[] camPoints;
    Vector3 mainCamPos;
    Quaternion mainCamRot;
    float mainCamFov;
    RenderTexture cameraPreviewTexture;

    void OnEnable()
    {
        mainCamPos = Camera.main.transform.position;
        mainCamRot = Camera.main.transform.rotation;
        mainCamFov = Camera.main.fieldOfView;

        lastTool = Tools.current;
        Tools.current = Tool.Custom;
        camPoints = Zone.GetComponentsInChildren<SideScrollingCameraPoint>();

        cameraPreviewTexture = new RenderTexture((int)(EditorSetting.camPreviewHeight * Camera.main.aspect), EditorSetting.camPreviewHeight, 24, RenderTextureFormat.Default);

        if (CurrentEditingPoint != null)
        {
            Camera.main.transform.position = CurrentEditingPoint.transform.position;
            Camera.main.transform.rotation = CurrentEditingPoint.transform.rotation;
            Camera.main.fieldOfView = CurrentEditingPoint.fov;
        }
    }

    void OnDisable()
    {
        Camera.main.transform.position = mainCamPos;
        Camera.main.transform.rotation = mainCamRot;
        Camera.main.fieldOfView = mainCamFov;
        Tools.current = lastTool;
    }

    // Inspector
    public override bool RequiresConstantRepaint()
    {
        return CurrentEditingPoint != null;
    }
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (EditorApplication.isPlaying) return;
        GUILayout.Space(16);
        GUILayout.Label("", "ProfilerDetailViewBackground");
        GUILayout.Label("Editor", "LODRendererRemove");

        GUILayout.BeginHorizontal("FrameBox");
        {
            GUILayout.BeginVertical();
            GUILayout.Space(14);
            if (GUILayout.Button(GUIContent.none, "OL Minus")) EditorSetting.snapValue *= 0.5f;
            GUILayout.EndVertical();

            GUILayout.BeginVertical();
            GUILayout.Label(EditorSetting.snapValue.ToString(), "LODLevelNotifyText");
            GUILayout.Label("Snap Value", "MeTimeLabel");
            GUILayout.EndVertical();

            GUILayout.BeginVertical();
            GUILayout.Space(14);
            if (GUILayout.Button(GUIContent.none, "OL Plus")) EditorSetting.snapValue *= 2;
            GUILayout.EndVertical();
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal("FrameBox");
        {
            GUILayout.BeginVertical();
            GUILayout.Space(14);
            if (GUILayout.Button(GUIContent.none, "OL Minus")) EditorSetting.angleSnapValue *= 0.5f;
            GUILayout.EndVertical();

            GUILayout.BeginVertical();
            GUILayout.Label(EditorSetting.angleSnapValue.ToString(), "LODLevelNotifyText");
            GUILayout.Label("Angle Snap Value", "MeTimeLabel");
            GUILayout.EndVertical();

            GUILayout.BeginVertical();
            GUILayout.Space(14);
            if (GUILayout.Button(GUIContent.none, "OL Plus")) EditorSetting.angleSnapValue *= 2;
            GUILayout.EndVertical();
        }
        GUILayout.EndHorizontal();

        GUILayout.Space(8);
        if (GUILayout.Button("New Camera Point"))
        {
            var newCam = GameObject.Instantiate(EditorSetting.sideScrollingCameraPointPrefab, Zone.cameraPoints);
            newCam.transform.localPosition = new Vector3(0, 2, -10);
            var newCamPoint = newCam.GetComponent<SideScrollingCameraPoint>();

            camPoints = Zone.GetComponentsInChildren<SideScrollingCameraPoint>();
            CurrentEditingPoint = newCamPoint;
        }
        if (CurrentEditingPoint != null)
        {
            GUILayout.Space(16);
            GUILayout.Label("Camera Panel", "LODRendererRemove");
            CameraPanel();
            GUILayout.Space(16);
            if (GUILayout.Button((EditorSetting.alwaysShowCameraPanelInSceneView ? "Hide" : "Show") + " Camera Panel in Scene View")) EditorSetting.alwaysShowCameraPanelInSceneView = !EditorSetting.alwaysShowCameraPanelInSceneView;
            if (GUILayout.Button((EditorSetting.alwaysShowCameraPreviewInSceneView ? "Hide" : "Show") + " Camera Preview in Scene View")) EditorSetting.alwaysShowCameraPreviewInSceneView = !EditorSetting.alwaysShowCameraPreviewInSceneView;
            if (GUILayout.Button((EditorSetting.cameraPitchAlwayEditable ? "Disable" : "Enable") + " Camera Pitch Editting")) EditorSetting.cameraPitchAlwayEditable = !EditorSetting.cameraPitchAlwayEditable;
        }
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
    Vector3 ToPlane(Vector3 val, Vector3 point, Vector3 normal)
    {
        var delta = val - Camera.current.transform.position;
        return val - delta * Vector3.Dot((val - point), normal) / Vector3.Dot(delta, normal);
    }
    float HalfFrac(float val) => val - Mathf.Floor(val + 0.5f);
    float HalfFracSnapped(float val) => EditorSetting.snapValue * HalfFrac(val / EditorSetting.snapValue);
    float Snap(float val) => Mathf.Floor(val / EditorSetting.snapValue + 0.5f) * EditorSetting.snapValue;
    float Snap(float val, float snapValue) => Mathf.Floor(val / snapValue + 0.5f) * snapValue;

    // GUI
    void CameraPanel()
    {
        var p = CurrentEditingPoint;
        GUIStyle labelStyle = "FrameBox";
        GUILayout.Label($"fov\t: {(int)p.fov}", labelStyle);
        GUILayout.Label($"pos\t: {(Vector2)p.transform.localPosition}", labelStyle);
        GUILayout.Label($"depth\t: {(p.Depth)}", labelStyle);
        GUILayout.Label($"pitch\t: {(p.transform.localRotation.eulerAngles.x)}", labelStyle);
        if (GUILayout.Button("Focus on Camera Point", "button"))
        {
            CurrentDrawingSceneView.LookAt(p.transform.position);
        }
        if (GUILayout.Button("Focus on Scroll View", "button"))
        {
            CurrentDrawingSceneView.LookAt(p.FocusPoint, transform.rotation);
        }
        if (GUILayout.Button("Reset Pitch", "button"))
        {
            Undo.RecordObject(p.transform, "Reset Camera Pitch");
            var targetPos = p.PlayerTargetPosition;
            p.transform.localRotation = Quaternion.identity;
            p.Offset = (Vector3)(p.transform.parent.worldToLocalMatrix * (targetPos - p.FocusPoint));
        }
        if (GUILayout.Button("Reset FOV", "button"))
        {
            Undo.RecordObject(p, "Reset Camera FOV");
            p.fov = Setting.camDefaultFov;
        }
        if (GUILayout.Button("Duplicate this Camera", "button"))
        {
            var newCam = Instantiate(EditorSetting.sideScrollingCameraPointPrefab, Zone.cameraPoints);
            newCam.transform.localPosition = p.transform.localPosition + Vector3.right;
            var newCamPoint = newCam.GetComponent<SideScrollingCameraPoint>();
            newCamPoint.Offset = p.Offset;
            newCamPoint.fov = p.fov;
            camPoints = Zone.GetComponentsInChildren<SideScrollingCameraPoint>();
            CurrentEditingPoint = newCamPoint;
        }
        if (GUILayout.Button("Destroy this Camera", "button"))
        {
            Undo.DestroyObjectImmediate(p.gameObject);
            camPoints = Zone.GetComponentsInChildren<SideScrollingCameraPoint>();
            CurrentEditingPoint = null;
        }
    }
    void DrawGrids(Vector3 targetPos, Vector3 planePos, Vector3 axisX, Vector3 axisY, float radius, Color color)
    {
        var alpha = color.a;
        var delta = targetPos - planePos;
        var center = targetPos - axisX * HalfFracSnapped(Vector3.Dot(delta, axisX)) - axisY * HalfFracSnapped(Vector3.Dot(delta, axisY));
        var segment = Mathf.FloorToInt(radius / EditorSetting.snapValue);

        Vector3 P(int x, int y) => center + x * axisX * EditorSetting.snapValue + y * axisY * EditorSetting.snapValue;

        for (int i = segment; i > 0; --i)
        {
            var a = (segment - i + 1) / (float)segment;
            color.a = alpha * a * a;
            Handles.color = color;

            Handles.DrawDottedLine(P(0, i), P(0, i - 1), 1);
            Handles.DrawDottedLine(P(i, 0), P(i - 1, 0), 1);
            Handles.DrawDottedLine(P(0, -i), P(0, -i + 1), 1);
            Handles.DrawDottedLine(P(-i, 0), P(-i + 1, 0), 1);

            int x = 1, y = i - 1;
            while (x < i)
            {
                Handles.DrawDottedLine(P(x, y), P(x - 1, y), 1);
                Handles.DrawDottedLine(P(x, y), P(x, y - 1), 1);
                ++x; --y;
            }

            x = i - 1; y = -1;
            while (x > 0)
            {
                Handles.DrawDottedLine(P(x, y), P(x - 1, y), 1);
                Handles.DrawDottedLine(P(x, y), P(x, y + 1), 1);
                --x; --y;
            }

            x = -1; y = -i + 1;
            while (x > -i)
            {
                Handles.DrawDottedLine(P(x, y), P(x + 1, y), 1);
                Handles.DrawDottedLine(P(x, y), P(x, y + 1), 1);
                --x; ++y;
            }

            x = -i + 1; y = 1;
            while (x < 0)
            {
                Handles.DrawDottedLine(P(x, y), P(x + 1, y), 1);
                Handles.DrawDottedLine(P(x, y), P(x, y - 1), 1);
                ++x; ++y;
            }

        }

    }

    Vector3 pos1;
    Vector3 pos2;

    void OnSceneGUI()
    {
        if (EditorApplication.isPlaying) return;
        if (CurrentDrawingSceneView == null) CurrentDrawingSceneView = SceneView.currentDrawingSceneView;

        Color c = Handles.color;

        // Entry & Exit Points
        if (CurrentEditingPoint == null)
        {
            Handles.color = EditorSetting.entryPointCaptureColor;

            EditorGUI.BeginChangeCheck();
            {
                pos1 = ToGround(Handles.FreeMoveHandle(transform.position, Quaternion.identity, EditorSetting.entryPointCaptureSize, Vector3.zero, EntryPointCapture));
                pos2 = ToGround(Handles.FreeMoveHandle(transform.position + transform.right * Zone.distance, Quaternion.identity, EditorSetting.entryPointCaptureSize, Vector3.zero, EntryPointCapture));
            }
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(Zone, "Move Scrolling Zone Point");
                Undo.RecordObject(Zone.transform, "Move Scrolling Zone Point");
                if (Event.current.control)
                {
                    pos1 = new Vector3(Snap(pos1.x), 0, Snap(pos1.z));
                    pos2 = new Vector3(Snap(pos2.x), 0, Snap(pos2.z));
                }
                transform.position = pos1;
                Zone.distance = Vector3.Distance(pos2, transform.position);
                transform.rotation = Quaternion.LookRotation(Vector3.Cross(pos2 - transform.position, Vector3.up), Vector3.up);
            }
        }
        else
        {
            Handles.color = EditorSetting.entryPointCaptureColor;
            if (Handles.Button(transform.position, Camera.current.transform.rotation, EditorSetting.entryPointCaptureSize, EditorSetting.entryPointCaptureSize, CircleCapture) ||
                Handles.Button(transform.position + transform.right * Zone.distance, Camera.current.transform.rotation, EditorSetting.entryPointCaptureSize, EditorSetting.entryPointCaptureSize, CircleCapture))
            {
                CurrentEditingPoint = null;
            }
        }

        // Drawings
        {
            // Draw Arrow connecting two points
            pos1 = transform.position;
            pos2 = transform.position + transform.right * Zone.distance;

            Handles.color = EditorSetting.entryPointArrowColor;
            float arrowSize = EditorSetting.entryPointArrowSize * HandleUtility.GetHandleSize(pos2);
            Handles.DrawLine(pos1, pos2);
            Handles.ConeHandleCap(-1, pos2 + (pos1 - pos2).normalized * arrowSize, Quaternion.LookRotation(pos2 - pos1, Vector3.up), arrowSize, EventType.Repaint);

            // Draw Camera Overflow Line
            Handles.color = EditorSetting.cameraOverflowLineColor;
            var camOverflow = transform.right * Zone.cameraOverflow;
            var yDir = Vector3.up * Setting.invisibleWallHeight;
            Handles.DrawDottedLine(transform.position - camOverflow - yDir, transform.position - camOverflow + yDir, 2);
            Handles.DrawDottedLine(transform.position + transform.right * Zone.distance + camOverflow - yDir, transform.position + transform.right * Zone.distance + camOverflow + yDir, 2);

            // Draw Grids on the ground
            if (CurrentEditingPoint == null && Event.current.control)
            {
                DrawGrids(pos1, Vector3.zero, Vector3.right, Vector3.forward, EditorSetting.gridGroundRadius, EditorSetting.gridGroundColor);
                DrawGrids(pos2, Vector3.zero, Vector3.right, Vector3.forward, EditorSetting.gridGroundRadius, EditorSetting.gridGroundColor);
            }
        }

        // Keys
        if (Event.current.type == EventType.KeyUp)
        {
            if (Event.current.keyCode == KeyCode.F)
            {
                if (CurrentEditingPoint == null)
                {
                    CurrentDrawingSceneView.LookAt(transform.position + 0.5f * Zone.distance * transform.right, Quaternion.LookRotation(Vector3.down, Vector3.forward), Zone.distance * 0.8f);
                }
                else
                {
                    CurrentDrawingSceneView.LookAt(CurrentEditingPoint.FocusPoint, transform.rotation, -CurrentEditingPoint.transform.localPosition.z);

                    Camera.main.transform.position = CurrentEditingPoint.transform.position;
                    Camera.main.transform.rotation = CurrentEditingPoint.transform.rotation;
                    Camera.main.fieldOfView = CurrentEditingPoint.fov;
                }
            }
        }

        // Camera Points
        foreach (var p in camPoints)
        {
            if (p == CurrentEditingPoint)
            {
                Vector3 pos;
                var sizeFactor = HandleUtility.GetHandleSize(p.transform.position);
                var focusPos = p.FocusPoint;
                var targetPos = p.PlayerTargetPosition;

                // Drawings
                if (Event.current.control)
                {
                    DrawGrids(focusPos, transform.position, transform.right, Vector3.up, EditorSetting.gridSideRadius, EditorSetting.gridSideColor);
                    DrawGrids(targetPos + Setting.playerFocusHeight * Vector3.down, transform.position, transform.right, Vector3.up, EditorSetting.gridSideRadius, EditorSetting.gridSideColor);

                    // Draw Grid Z Axis
                    {
                        var color = EditorSetting.gridZAxisColor;
                        var alpha = color.a;
                        var segment = Mathf.FloorToInt(EditorSetting.gridZAxisRadius / EditorSetting.snapValue);
                        var center = p.transform.position + transform.forward * HalfFracSnapped(p.Depth);

                        Vector3 P(int i) => center + i * transform.forward * EditorSetting.snapValue;
                        void CrossP(Vector3 point)
                        {
                            Handles.DrawLine(point, point + Vector3.up * 0.1f * sizeFactor);
                            Handles.DrawLine(point, point - Vector3.up * 0.1f * sizeFactor);
                            Handles.DrawLine(point, point + transform.right * 0.1f * sizeFactor);
                            Handles.DrawLine(point, point - transform.right * 0.1f * sizeFactor);
                        }

                        for (int i = segment; i > 0; --i)
                        {
                            var a = (segment - i + 1) / (float)segment;
                            color.a = alpha * a * a;
                            Handles.color = color;

                            Handles.DrawDottedLine(P(i), P(i - 1), 1);
                            CrossP(P(i - 1));
                            Handles.DrawDottedLine(P(-i), P(-i + 1), 1);
                            CrossP(P(-i + 1));
                        }
                    }
                }

                // edit target point
                {
                    Handles.color = EditorSetting.camTargetCaptureColor;

                    EditorGUI.BeginChangeCheck();
                    pos = ToPlane(Handles.FreeMoveHandle(targetPos, Quaternion.identity, 0.3f, Vector3.zero, CamTargetCapture), transform.position, transform.forward);
                    if (EditorGUI.EndChangeCheck())
                    {
                        Undo.RecordObject(p, "Move Camera Point");
                        if (Event.current.control)
                        {
                            pos -= transform.right * HalfFracSnapped(Vector3.Dot(transform.right, pos - transform.position));
                            pos.y = Snap(pos.y - Setting.playerFocusHeight) + Setting.playerFocusHeight;
                        }
                        p.Offset = (Vector3)(p.transform.parent.worldToLocalMatrix * (pos - focusPos));
                    }
                }

                // edit cam point
                {
                    Handles.color = EditorSetting.camPosCaptureColor;

                    EditorGUI.BeginChangeCheck();
                    pos = ToPlane(Handles.FreeMoveHandle(p.transform.position, Quaternion.identity, p.Depth, Vector3.zero, CamPosCapture), p.transform.position, transform.forward);
                    if (EditorGUI.EndChangeCheck())
                    {
                        Undo.RecordObject(p, "Move Camera Point");
                        Undo.RecordObject(p.transform, "Move Camera Point");
                        if (Event.current.control)
                        {
                            pos -= transform.right * HalfFracSnapped(Vector3.Dot(transform.right, pos - transform.position));
                            pos.y = Snap(pos.y);
                        }
                        var preLocalZ = p.transform.localPosition.z;
                        p.transform.position = pos;
                        var local = p.transform.localPosition;
                        if (local.x < -Zone.cameraOverflow) local.x = -Zone.cameraOverflow;
                        else if (local.x > Zone.distance + Zone.cameraOverflow) local.x = Zone.distance + Zone.cameraOverflow;
                        local.z = preLocalZ;
                        p.transform.localPosition = local;
                        p.Offset = (Vector3)(p.transform.parent.worldToLocalMatrix * (targetPos - p.FocusPoint));

                        Camera.main.transform.position = p.transform.position;
                        Camera.main.transform.rotation = p.transform.rotation;
                    }

                    Handles.color = EditorSetting.camFocusCaptureColor;

                    EditorGUI.BeginChangeCheck();
                    pos = ToPlane(Handles.FreeMoveHandle(focusPos, Quaternion.identity, p.Depth, Vector3.zero, CamFocusCapture), transform.position, transform.forward);
                    if (EditorGUI.EndChangeCheck())
                    {
                        Undo.RecordObject(p, "Move Camera Point");
                        Undo.RecordObject(p.transform, "Move Camera Point");
                        if (Event.current.control)
                        {
                            pos -= transform.right * HalfFracSnapped(Vector3.Dot(transform.right, pos - transform.position));
                            pos.y = Snap(pos.y);
                        }
                        var preLocalZ = p.transform.localPosition.z;
                        p.transform.position += pos - focusPos;
                        var local = p.transform.localPosition;
                        if (local.x < -Zone.cameraOverflow) local.x = -Zone.cameraOverflow;
                        else if (local.x > Zone.distance + Zone.cameraOverflow) local.x = Zone.distance + Zone.cameraOverflow;
                        local.z = preLocalZ;
                        p.transform.localPosition = local;
                        p.Offset = (Vector3)(p.transform.parent.worldToLocalMatrix * (targetPos - p.FocusPoint));

                        Camera.main.transform.position = p.transform.position;
                        Camera.main.transform.rotation = p.transform.rotation;
                    }
                    Handles.color = EditorSetting.camOffsetLineColor;
                    Handles.DrawLine(targetPos, focusPos);
                }

                // edit fov
                {
                    var angleRot = Quaternion.AngleAxis(p.fov * 2, -transform.forward);
                    var handlePos = p.transform.position + sizeFactor * EditorSetting.camFovCaptureRadius * (angleRot * Vector3.up);

                    Handles.color = EditorSetting.camFovCaptureColor;

                    EditorGUI.BeginChangeCheck();
                    pos = ToPlane(Handles.FreeMoveHandle(handlePos, Quaternion.identity, p.fov, Vector3.zero, CamFovCapture), p.transform.position, transform.forward);
                    if (EditorGUI.EndChangeCheck())
                    {
                        Undo.RecordObject(p, "Edit Camera fov");
                        var angle = Vector3.SignedAngle(Vector3.up, pos - p.transform.position, -transform.forward);
                        if (angle < 0) angle += 360;
                        if (Event.current.control)
                        {
                            angle = Snap(angle, EditorSetting.angleSnapValue * 2);
                        }
                        p.fov = angle * 0.5f;

                        Camera.main.fieldOfView = p.fov;
                    }

                    Handles.color = EditorSetting.camFovCaptureColor;
                    Handles.DrawSolidArc(p.transform.position, -transform.forward, Vector3.up, p.fov * 2, sizeFactor * EditorSetting.camFovArcRadius);
                    Handles.DrawLine(p.transform.position, handlePos);

                    // Draw Grid Fov
                    if (Event.current.control)
                    {
                        var color = EditorSetting.gridFovColor;
                        var alpha = color.a;
                        var segment = Mathf.FloorToInt(EditorSetting.gridFovRange / EditorSetting.angleSnapValue);
                        var centerDir = EditorSetting.gridFovRadius * sizeFactor * (Quaternion.AngleAxis(2 * Snap(p.fov, EditorSetting.angleSnapValue), -transform.forward) * Vector3.up);
                        var baseI = Mathf.FloorToInt(p.fov / EditorSetting.angleSnapValue);

                        for (int i = -segment; i <= segment; ++i)
                        {
                            var a = 1.1f - Mathf.Abs(i / (float)segment);
                            color.a = alpha * a * a;
                            if (((i + baseI) & 1) != 0) color.a *= 0.2f;
                            Handles.color = color;
                            Handles.DrawLine(p.transform.position, p.transform.position + Quaternion.AngleAxis(2 * EditorSetting.angleSnapValue * i, -transform.forward) * centerDir);
                        }
                    }
                }

                // edit cam Z
                {
                    Handles.color = EditorSetting.camZCaptureColor;

                    EditorGUI.BeginChangeCheck();
                    pos = ToPlane(Handles.FreeMoveHandle(p.transform.position, Quaternion.identity, p.Depth, Vector3.zero, CamZCapture), p.transform.position, Vector3.Cross(transform.forward, Vector3.Cross(p.transform.position - Camera.current.transform.position, transform.forward)).normalized);
                    if (EditorGUI.EndChangeCheck())
                    {
                        Undo.RecordObject(p.transform, "Move Camera Point");
                        var local = p.transform.localPosition;
                        local.z = Vector3.Dot(pos - transform.position, transform.forward);
                        if (Event.current.control)
                        {
                            local.z = Snap(local.z);
                        }
                        p.transform.localPosition = local;

                        Camera.main.transform.position = p.transform.position;
                    }
                    Handles.color = EditorSetting.camZCaptureColor;
                    Handles.DrawLine(p.transform.position, focusPos);
                }

                // edit Rotation
                if (EditorSetting.cameraPitchAlwayEditable || Event.current.control)
                {
                    Handles.color = EditorSetting.camPitchCaptureColor;
                    EditorGUI.BeginChangeCheck();
                    var rot = Handles.FreeRotateHandle(p.transform.rotation, p.transform.position, sizeFactor * 0.8f);
                    if (EditorGUI.EndChangeCheck())
                    {
                        Undo.RecordObject(p, "Edit Camera Pitch");
                        Undo.RecordObject(p.transform, "Edit Camera Pitch");

                        rot = Quaternion.Inverse(Zone.transform.rotation) * rot;
                        var pitch = rot.eulerAngles.x;
                        while (pitch > 180) pitch -= 360;
                        if (pitch > 72) pitch = 72;
                        if (pitch < -72) pitch = -72;

                        if (Event.current.control)
                        {
                            pitch = Snap(pitch, EditorSetting.angleSnapValue * 2);
                        }
                        rot = Quaternion.AngleAxis(pitch, Vector3.right);
                        p.transform.localRotation = rot;
                        p.Offset = (Vector3)(p.transform.parent.worldToLocalMatrix * (targetPos - p.FocusPoint));

                        Camera.main.transform.rotation = p.transform.rotation;
                    }
                }

                // Camera Panel
                if (EditorSetting.alwaysShowCameraPanelInSceneView || Event.current.shift)
                {
                    GUIStyle areaStyle = "window";
                    Rect areaRect = HandleUtility.WorldPointToSizedRect(p.transform.position + sizeFactor * (EditorSetting.camFovCaptureRadius + EditorSetting.camFovCaptureSize) * (-Camera.current.transform.right + Vector3.up * 0.5f), GUIContent.none, areaStyle);
                    areaRect.width = EditorSetting.camGUIPanelWidth;
                    areaRect.height = EditorSetting.camGUIPanelHeight;
                    areaRect.x += EditorSetting.camGUIPanelXOffset - EditorSetting.camGUIPanelWidth;
                    areaRect.y += EditorSetting.camGUIPanelYOffset;
                    Handles.BeginGUI();
                    {
                        GUILayout.BeginArea(areaRect, "Camera Point", areaStyle);
                        CameraPanel();
                        GUILayout.EndArea();
                    }
                    Handles.EndGUI();
                }
                if (CurrentEditingPoint == null) return;    // Handle the situation where the current editting point is destroyed

                // Preview Panel
                if (EditorSetting.alwaysShowCameraPreviewInSceneView || Event.current.control)
                {
                    Camera.main.targetTexture = cameraPreviewTexture;
                    Camera.main.Render();
                    Camera.main.targetTexture = null;

                    Rect previewRect = new Rect(4, 4, EditorSetting.camPreviewHeight * Camera.main.aspect, EditorSetting.camPreviewHeight);
                    Handles.BeginGUI();
                    {
                        GUI.DrawTexture(previewRect, cameraPreviewTexture);
                    }
                    Handles.EndGUI();
                }
            }
            else
            {
                Handles.color = EditorSetting.camPointSelectionColor;
                if (Handles.Button(p.transform.position, Camera.current.transform.rotation, 0.08f, 0.08f, CircleCapture))
                {
                    CurrentEditingPoint = p;
                    SceneView.currentDrawingSceneView.LookAt(p.transform.position);

                    Camera.main.transform.position = p.transform.position;
                    Camera.main.transform.rotation = p.transform.rotation;
                    Camera.main.fieldOfView = p.fov;
                }
            }
        }

        Handles.color = c;
    }
    /// <summary>
    /// This circle capture will keep a constant screen size
    /// </summary>
    void EntryPointCapture(int controlID, Vector3 position, Quaternion rotation, float size, EventType eventType)
    {
        var sizeFactor = HandleUtility.GetHandleSize(position);
        size *= sizeFactor;
        Handles.CircleHandleCap(controlID, position, rotation, size, eventType);
        Handles.CircleHandleCap(controlID, position, rotation, size * 0.9f, eventType);
    }
    void CircleCapture(int controlID, Vector3 position, Quaternion rotation, float size, EventType eventType)
    {
        Handles.CircleHandleCap(controlID, position, rotation, size, eventType);
    }
    void CamTargetCapture(int controlID, Vector3 position, Quaternion rotation, float size, EventType eventType)
    {
        var footPos = position + Vector3.down * Setting.playerFocusHeight;
        var upRot = Quaternion.LookRotation(Vector3.up, Vector3.forward);
        Handles.ArrowHandleCap(controlID, footPos, upRot, Setting.playerFocusHeight * 0.8f, eventType);
        //Handles.SphereHandleCap(controlID, position, rotation, size, eventType);
        Handles.CircleHandleCap(controlID, footPos, upRot, Setting.playerFocusHeight * 0.3f, eventType);
        Handles.CircleHandleCap(controlID, position + Setting.playerFocusHeight * 0.5f * Vector3.down, rotation, Setting.playerFocusHeight * 0.5f, eventType);
    }
    void CamPosCapture(int controlID, Vector3 position, Quaternion rotation, float z, EventType eventType)
    {
        var sizeFactor = HandleUtility.GetHandleSize(position);
        var size = EditorSetting.camPosSize * sizeFactor;
        Handles.CircleHandleCap(controlID, position, transform.rotation, size, eventType);
        Handles.CircleHandleCap(controlID, position, transform.rotation, size * 0.9f, eventType);
    }
    void CamFocusCapture(int controlID, Vector3 position, Quaternion rotation, float z, EventType eventType)
    {
        Handles.CircleHandleCap(controlID, CurrentEditingPoint.FocusPoint, transform.rotation, EditorSetting.focusCircleSize, eventType);
    }
    void CamZCapture(int controlID, Vector3 position, Quaternion rotation, float z, EventType eventType)
    {
        var sizeFactor = HandleUtility.GetHandleSize(position);

        Handles.ArrowHandleCap(controlID, position + CurrentEditingPoint.transform.forward * (sizeFactor * 0.4f), Quaternion.LookRotation(-CurrentEditingPoint.transform.forward), sizeFactor * EditorSetting.camZSize, eventType);
    }
    void CamFovCapture(int controlID, Vector3 position, Quaternion rotation, float fov, EventType eventType)
    {
        var sizeFactor = HandleUtility.GetHandleSize(position);
        var angleRot = Quaternion.AngleAxis(fov * 2, -transform.forward);

        Handles.CubeHandleCap(controlID, position, angleRot * transform.rotation, EditorSetting.camFovCaptureSize * sizeFactor, eventType);
    }
    [DrawGizmo(GizmoType.Selected | GizmoType.NonSelected | GizmoType.Pickable)]
    static void ZoneGizmos(SideScrollingZone zone, GizmoType gizmoType)
    {
        var yBias = Vector3.up * 0.01f;
        var p = zone.transform.position;
        var pp = zone.transform.position + zone.transform.right * zone.distance;

        Gizmos.color = EditorSetting.zonePointColor;
        Gizmos.DrawSphere(p, EditorSetting.entryPointCaptureSize);
        Gizmos.DrawSphere(pp, EditorSetting.entryPointCaptureSize);

        Gizmos.color = EditorSetting.zonePathColor;
        Gizmos.DrawLine(p + yBias, pp);

        if (gizmoType.HasFlag(GizmoType.NonSelected))
        {
            var offsetZ = Setting.sideScrollingPathWidth * 0.5f * zone.transform.forward;
            var wallSizeX = zone.distance * zone.transform.right;
            var wallSizeZ = Setting.invisibleWallDepth * zone.transform.forward;
            Gizmos.color = EditorSetting.zoneWallColor;
            Gizmos.DrawRay(p + yBias + offsetZ, wallSizeZ);
            Gizmos.DrawRay(p + yBias + offsetZ + wallSizeZ, wallSizeX);
            Gizmos.DrawRay(p + yBias + offsetZ, wallSizeX);
            Gizmos.DrawRay(p + yBias + offsetZ + wallSizeX, wallSizeZ);
            Gizmos.DrawRay(p + yBias - offsetZ, -wallSizeZ);
            Gizmos.DrawRay(p + yBias - offsetZ - wallSizeZ, wallSizeX);
            Gizmos.DrawRay(p + yBias - offsetZ, wallSizeX);
            Gizmos.DrawRay(p + yBias - offsetZ + wallSizeX, -wallSizeZ);
        }

        Gizmos.color = Color.white;
    }

    [DrawGizmo(GizmoType.NotInSelectionHierarchy | GizmoType.InSelectionHierarchy | GizmoType.Pickable)]
    static void CameraPointGizmos(SideScrollingCameraPoint p, GizmoType gizmoType)
    {
        if (Camera.main == null) return;
        var zone = p.GetComponentInParent<SideScrollingZone>();
        if (zone == null) return;

        bool error = p.transform.localPosition.z > 0;
        bool selected = gizmoType.HasFlag(GizmoType.InSelectionHierarchy) && p == CurrentEditingPoint;

        float y = Mathf.Tan(p.fov * Mathf.PI / 360f);
        float x = y * Camera.main.aspect;
        var pp = p.transform.position;

        Vector3 topLeft = p.transform.localRotation * new Vector3(-x, y, 1);
        Vector3 botLeft = p.transform.localRotation * new Vector3(-x, -y, 1);
        topLeft *= p.Depth / topLeft.z;
        botLeft *= p.Depth / botLeft.z;
        Vector3 topRight = new Vector3(-topLeft.x, topLeft.y, p.Depth);
        Vector3 botRight = new Vector3(-botLeft.x, botLeft.y, p.Depth);

        var rot = zone.transform.rotation;
        topLeft = pp + rot * topLeft;
        botLeft = pp + rot * botLeft;
        topRight = pp + rot * topRight;
        botRight = pp + rot * botRight;

        Gizmos.color = error ? EditorSetting.camPointGizmosErrorColor : selected ? EditorSetting.camPointGizmosModelColorSelected : EditorSetting.camPointGizmosModelColor;
        Gizmos.DrawWireMesh(EditorSetting.characterMesh, p.PlayerTargetPosition + Setting.playerFocusHeight * Vector3.down, zone.transform.rotation);

        Gizmos.color = error ? EditorSetting.camPointGizmosErrorColor : selected ? EditorSetting.camPointGizmosBorderColorSelected : EditorSetting.camPointGizmosBorderColor;
        Gizmos.DrawLine(topLeft, topRight);
        Gizmos.DrawLine(botLeft, botRight);
        Gizmos.DrawLine(topLeft, botLeft);
        Gizmos.DrawLine(topRight, botRight);

        if (!selected)
        {
            Gizmos.color = error ? EditorSetting.camPointGizmosErrorColor : EditorSetting.camPointGizmosSphereColor;
            Gizmos.DrawSphere(pp, 0.08f);
        }

        Gizmos.color = error ? EditorSetting.camPointGizmosErrorColor : selected ? EditorSetting.camPointGizmosLineColorSelected : EditorSetting.camPointGizmosLineColor;
        if (p.transform.localPosition.z > 0)
        {
            Gizmos.DrawLine(topLeft, botRight);
            Gizmos.DrawLine(topRight, botLeft);
        }
        else
        {
            Gizmos.DrawLine(topLeft, pp);
            Gizmos.DrawLine(botLeft, pp);
            Gizmos.DrawLine(topRight, pp);
            Gizmos.DrawLine(botRight, pp);
        }
    }
    [DrawGizmo(GizmoType.NonSelected | GizmoType.Selected)]
    static void CameraGizmos(CameraController cam, GizmoType gizmoType)
    {
        if (!cam.IsSideScrollingMode) return;

        float weight = (cam.CameraPointLeft == null ? 0 : cam.CameraPointLeft.weight) + (cam.CameraPointRight == null ? 0 : cam.CameraPointRight.weight);
        if (cam.CameraPointLeft == cam.CameraPointRight) weight *= 0.5f;
        if (weight > 0)
        {
            var pPlayer = GameplaySystem.CurrentPlayer.transform.position + Setting.playerFocusHeight * Vector3.up;
            if (cam.CameraPointLeft != null)
            {
                var w = cam.CameraPointLeft.weight / weight;
                var p = cam.CameraPointLeft.PlayerTargetPosition;
                Gizmos.color = EditorSetting.camOffsetLineColor;
                Gizmos.DrawLine(pPlayer, p + (1 - w) * Setting.playerFocusHeight * Vector3.down);
                Gizmos.color = EditorSetting.camPointGizmosModelColorSelected;
                Gizmos.DrawWireMesh(EditorSetting.characterMesh, p + Setting.playerFocusHeight * Vector3.down, GameplaySystem.CurrentPlayer.transform.rotation, Vector3.one * w);
            }
            if (cam.CameraPointRight != null)
            {
                var w = cam.CameraPointRight.weight / weight;
                var p = cam.CameraPointRight.PlayerTargetPosition;
                Gizmos.color = EditorSetting.camOffsetLineColor;
                Gizmos.DrawLine(pPlayer, p + (1 - w) * Setting.playerFocusHeight * Vector3.down);
                Gizmos.color = EditorSetting.camPointGizmosModelColorSelected;
                Gizmos.DrawWireMesh(EditorSetting.characterMesh, p + Setting.playerFocusHeight * Vector3.down, GameplaySystem.CurrentPlayer.transform.rotation, Vector3.one * w);
            }
        }
        Gizmos.color = Color.white;
    }
}
