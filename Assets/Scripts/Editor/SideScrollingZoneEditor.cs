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
    GameplaySystemSetting Setting => GameplaySystem.Setting;

    // Static Properties
    public static SideScrollingCameraPoint CurrentEditingPoint { get; set; } = null;
    static SceneView CurrentDrawingSceneView { get; set; } = null;
    static bool ShowCameraPanelInSceneView { get; set; } = false;
    static bool ShowCameraPreviewInSceneView { get; set; } = true;
    static float SnapValue { get; set; } = 1f;
    static float FovSnapValue { get; set; } = 5;

    // Consts
    const float entryPointCaptureSize = 0.2f;
    const float entryPointArrowSize = 0.1f;
    const float focusCircleSize = 0.1f;
    const float camPosSize = 0.5f;
    const float camZSize = 0.7f;
    const float camFovArcRadius = 0.35f;
    const float camFovCaptureRadius = 0.7f;
    const float camFovCaptureSize = 0.1f;
    const float camGUIPanelWidth = 156;
    const float camGUIPanelHeight = 202;
    const int camPreviewHeight = 100;

    readonly Color entryPointCaptureColor = new Color(0, 1, 0, 1);
    readonly Color entryPointArrowColor = new Color(0, 1, 0, 1);
    readonly Color cameraOverflowLineColor = new Color(1f, 0.3f, 0, 0.6f);
    readonly Color gridGroundColor = new Color(0.8f, 0.8f, 0.6f, 1);
    readonly Color gridSideColor = new Color(0.8f, 0.8f, 0.6f, 1);
    readonly Color gridZAxisColor = new Color(1, 1, 1, 1);
    readonly Color gridFovColor = new Color(1, 1, 1, 1);

    const int gridGroundRadius = 10;
    const int gridSideRadius = 6;
    const int gridZAxisRadius = 4;
    const int gridFovRange = 60;

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

        cameraPreviewTexture = new RenderTexture((int)(camPreviewHeight * Camera.main.aspect), camPreviewHeight, 24, RenderTextureFormat.Default);

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
        GUILayout.Space(16);
        GUILayout.Label("", "ProfilerDetailViewBackground");
        GUILayout.Label("Editor", "LODRendererRemove");

        GUILayout.BeginHorizontal("FrameBox");
        {
            GUILayout.BeginVertical();
            GUILayout.Space(14);
            if (GUILayout.Button(GUIContent.none, "OL Minus")) SnapValue *= 0.5f;
            GUILayout.EndVertical();

            GUILayout.BeginVertical();
            GUILayout.Label(SnapValue.ToString(), "LODLevelNotifyText");
            GUILayout.Label("Snap Value", "MeTimeLabel");
            GUILayout.EndVertical();

            GUILayout.BeginVertical();
            GUILayout.Space(14);
            if (GUILayout.Button(GUIContent.none, "OL Plus")) SnapValue *= 2;
            GUILayout.EndVertical();
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal("FrameBox");
        {
            GUILayout.BeginVertical();
            GUILayout.Space(14);
            if (GUILayout.Button(GUIContent.none, "OL Minus")) FovSnapValue *= 0.5f;
            GUILayout.EndVertical();

            GUILayout.BeginVertical();
            GUILayout.Label(FovSnapValue.ToString(), "LODLevelNotifyText");
            GUILayout.Label("FOV Snap Value", "MeTimeLabel");
            GUILayout.EndVertical();

            GUILayout.BeginVertical();
            GUILayout.Space(14);
            if (GUILayout.Button(GUIContent.none, "OL Plus")) FovSnapValue *= 2;
            GUILayout.EndVertical();
        }
        GUILayout.EndHorizontal();

        GUILayout.Space(8);
        if (GUILayout.Button("New Camera Point"))
        {
            var newCam = GameObject.Instantiate(Setting.sideScrollingCameraPointPrefab, Zone.cameraPoints);
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
            if (GUILayout.Button((ShowCameraPanelInSceneView ? "Hide" : "Show") + " Camera Panel in Scene View")) ShowCameraPanelInSceneView = !ShowCameraPanelInSceneView;
            if (GUILayout.Button((ShowCameraPreviewInSceneView ? "Hide" : "Show") + " Camera Preview in Scene View")) ShowCameraPreviewInSceneView = !ShowCameraPreviewInSceneView;
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
    float HalfFracSnapped(float val) => SnapValue * HalfFrac(val / SnapValue);
    float Snap(float val) => Mathf.Floor(val / SnapValue + 0.5f) * SnapValue;
    float Snap(float val, float snapValue) => Mathf.Floor(val / snapValue + 0.5f) * snapValue;

    // GUI
    void CameraPanel()
    {
        var p = CurrentEditingPoint;
        GUIStyle labelStyle = "FrameBox";
        p.fov = EditorGUILayout.FloatField("fov\t:", p.fov, labelStyle);
        GUILayout.Label($"pos\t: {(Vector2)p.transform.localPosition}", labelStyle);
        GUILayout.Label($"depth\t: {(-p.transform.localPosition.z)}", labelStyle);
        if (GUILayout.Button("Focus on Camera Point", "button"))
        {
            CurrentDrawingSceneView.LookAt(p.transform.position);
        }
        if (GUILayout.Button("Focus on Scroll View", "button"))
        {
            CurrentDrawingSceneView.LookAt(p.transform.position - transform.forward * p.transform.localPosition.z, transform.rotation);
        }
        if (GUILayout.Button("Duplicate this Camera", "button"))
        {
            var newCam = GameObject.Instantiate(Setting.sideScrollingCameraPointPrefab, Zone.cameraPoints);
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
        var segment = Mathf.FloorToInt(radius / SnapValue);

        Vector3 P(int x, int y) => center + x * axisX * SnapValue + y * axisY * SnapValue;

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
        if (CurrentDrawingSceneView == null) CurrentDrawingSceneView = SceneView.currentDrawingSceneView;

        Color c = Handles.color;

        // Entry & Exit Points
        if (CurrentEditingPoint == null)
        {

            EditorGUI.BeginChangeCheck();
            {
                pos1 = ToGround(Handles.FreeMoveHandle(transform.position, Quaternion.identity, entryPointCaptureSize, Vector3.zero, EntryPointCapture));
                pos2 = ToGround(Handles.FreeMoveHandle(transform.position + transform.right * Zone.distance, Quaternion.identity, entryPointCaptureSize, Vector3.zero, EntryPointCapture));
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
            Handles.color = entryPointCaptureColor;
            if (Handles.Button(transform.position, Camera.current.transform.rotation, entryPointCaptureSize, entryPointCaptureSize, CircleCapture) ||
                Handles.Button(transform.position + transform.right * Zone.distance, Camera.current.transform.rotation, entryPointCaptureSize, entryPointCaptureSize, CircleCapture))
            {
                CurrentEditingPoint = null;
            }
        }

        // Drawings
        {
            // Draw Arrow connecting two points
            pos1 = transform.position;
            pos2 = transform.position + transform.right * Zone.distance;

            Handles.color = entryPointArrowColor;
            float arrowSize = entryPointArrowSize * HandleUtility.GetHandleSize(pos2);
            Handles.DrawLine(pos1, pos2);
            Handles.ConeHandleCap(-1, pos2 + (pos1 - pos2).normalized * arrowSize, Quaternion.LookRotation(pos2 - pos1, Vector3.up), arrowSize, EventType.Repaint);

            // Draw Camera Overflow Line
            Handles.color = cameraOverflowLineColor;
            var camOverflow = transform.right * Zone.cameraOverflow;
            var yDir = Vector3.up * Setting.invisibleWallHeight;
            Handles.DrawDottedLine(transform.position - camOverflow - yDir, transform.position - camOverflow + yDir, 2);
            Handles.DrawDottedLine(transform.position + transform.right * Zone.distance + camOverflow - yDir, transform.position + transform.right * Zone.distance + camOverflow + yDir, 2);

            // Draw Grids on the ground
            if (CurrentEditingPoint == null && Event.current.control)
            {
                DrawGrids(pos1, Vector3.zero, Vector3.right, Vector3.forward, gridGroundRadius, gridGroundColor);
                DrawGrids(pos2, Vector3.zero, Vector3.right, Vector3.forward, gridGroundRadius, gridGroundColor);
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
                    CurrentDrawingSceneView.LookAt(CurrentEditingPoint.transform.position - transform.forward * CurrentEditingPoint.transform.localPosition.z, transform.rotation, -CurrentEditingPoint.transform.localPosition.z);

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
                var focusPos = p.transform.position - transform.forward * p.transform.localPosition.z;
                var targetPos = p.GetPlayerTargetPosition();

                // edit target point

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
                    p.Offset = (Vector3)(p.transform.parent.worldToLocalMatrix * (pos - p.transform.position));
                }

                // edit cam point
                EditorGUI.BeginChangeCheck();
                pos = ToPlane(Handles.FreeMoveHandle(p.transform.position, Quaternion.identity, -p.transform.localPosition.z, Vector3.zero, CamPosCapture), p.transform.position, transform.forward);
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
                    p.Offset = (Vector3)(p.transform.parent.worldToLocalMatrix * (targetPos - p.transform.position));

                    Camera.main.transform.position = p.transform.position;
                    Camera.main.transform.rotation = p.transform.rotation;
                }

                // edit cam Z
                Handles.DrawLine(p.transform.position, focusPos);
                Handles.DrawLine(targetPos, focusPos);

                EditorGUI.BeginChangeCheck();
                pos = ToPlane(Handles.FreeMoveHandle(p.transform.position, Quaternion.identity, -p.transform.localPosition.z, Vector3.zero, CamZCapture), p.transform.position, Vector3.Cross(transform.forward, Vector3.Cross(p.transform.position - Camera.current.transform.position, transform.forward)).normalized);
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
                    Camera.main.transform.rotation = p.transform.rotation;
                }

                // edit fov
                var angleRot = Quaternion.AngleAxis(p.fov * 2, -transform.forward);
                var handlePos = p.transform.position + sizeFactor * camFovCaptureRadius * (angleRot * Vector3.up);

                Handles.DrawSolidArc(p.transform.position, -transform.forward, Vector3.up, p.fov * 2, sizeFactor * camFovArcRadius);
                Handles.DrawLine(p.transform.position, handlePos);

                EditorGUI.BeginChangeCheck();
                pos = ToPlane(Handles.FreeMoveHandle(handlePos, Quaternion.identity, p.fov, Vector3.zero, CamFovCapture), p.transform.position, transform.forward);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(p, "Edit Camera fov");
                    var angle = Vector3.SignedAngle(Vector3.up, pos - p.transform.position, -transform.forward);
                    if (angle < 0) angle += 360;
                    if (Event.current.control)
                    {
                        angle = Snap(angle, FovSnapValue * 2);
                    }
                    p.fov = angle * 0.5f;

                    Camera.main.fieldOfView = p.fov;
                }

                // Camera Panel
                if (ShowCameraPanelInSceneView || Event.current.control)
                {
                    GUIStyle areaStyle = "window";
                    Rect areaRect = HandleUtility.WorldPointToSizedRect(p.transform.position + sizeFactor * (camFovCaptureRadius + camFovCaptureSize + 0.3f) * (Camera.current.transform.right + Vector3.up * 0.5f), GUIContent.none, areaStyle);
                    areaRect.width = camGUIPanelWidth;
                    areaRect.height = camGUIPanelHeight;
                    Handles.BeginGUI();
                    {
                        GUILayout.BeginArea(areaRect, "Camera Point", areaStyle);
                        CameraPanel();
                        GUILayout.EndArea();
                    }
                    Handles.EndGUI();
                }

                // Preview Panel
                if (ShowCameraPreviewInSceneView || Event.current.control)
                {
                    Camera.main.targetTexture = cameraPreviewTexture;
                    Camera.main.Render();
                    Camera.main.targetTexture = null;

                    Rect previewRect = new Rect(4, 4, camPreviewHeight * Camera.main.aspect, camPreviewHeight);
                    Handles.BeginGUI();
                    {
                        GUI.DrawTexture(previewRect, cameraPreviewTexture);
                    }
                    Handles.EndGUI();
                }


                if (CurrentEditingPoint == null) return;    // Handle the situation where the current editting point is destroyed

                // Drawings
                if (Event.current.control)
                {
                    DrawGrids(focusPos, transform.position, transform.right, Vector3.up, gridSideRadius, gridSideColor);
                    DrawGrids(targetPos + Setting.playerFocusHeight * Vector3.down, transform.position, transform.right, Vector3.up, gridSideRadius, gridSideColor);

                    // Draw Grid Z Axis
                    {
                        var color = gridZAxisColor;
                        var alpha = color.a;
                        var segment = Mathf.FloorToInt(gridZAxisRadius / SnapValue);
                        var center = p.transform.position + transform.forward * HalfFracSnapped(-p.transform.localPosition.z);

                        Vector3 P(int i) => center + i * transform.forward * SnapValue;
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

                    // Draw Grid Fov
                    {
                        var color = gridFovColor;
                        var alpha = color.a;
                        var segment = Mathf.FloorToInt(gridFovRange / FovSnapValue);
                        var centerDir = camFovCaptureRadius * sizeFactor * 1.5f * (Quaternion.AngleAxis(2 * Snap(p.fov, FovSnapValue), -transform.forward) * Vector3.up);
                        var baseI = Mathf.FloorToInt(p.fov / FovSnapValue);

                        for (int i = -segment; i <= segment; ++i)
                        {
                            var a = 1.1f - Mathf.Abs(i / (float)segment);
                            color.a = alpha * a * a;
                            if (((i + baseI) & 1) != 0) color.a *= 0.2f;
                            Handles.color = color;
                            Handles.DrawLine(p.transform.position, p.transform.position + Quaternion.AngleAxis(2 * FovSnapValue * i, -transform.forward) * centerDir);
                        }
                    }
                }
            }
            else
            {
                Handles.color = new Color(1, 0.5f, 0, 0.7f);
                if (Handles.Button(p.transform.position, Camera.current.transform.rotation, 0.2f, 0.2f, CircleCapture))
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
        Handles.color = entryPointCaptureColor;
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
        Handles.SphereHandleCap(controlID, position, rotation, size, eventType);
        Handles.CircleHandleCap(controlID, footPos, upRot, Setting.playerFocusHeight * 0.3f, eventType);
        Handles.CircleHandleCap(controlID, position + Setting.playerFocusHeight * 0.5f * Vector3.down, rotation, Setting.playerFocusHeight * 0.5f, eventType);
    }
    void CamPosCapture(int controlID, Vector3 position, Quaternion rotation, float z, EventType eventType)
    {
        var sizeFactor = HandleUtility.GetHandleSize(position);
        var size = camPosSize * sizeFactor;
        Handles.CircleHandleCap(controlID, position, transform.rotation, size, eventType);
        Handles.CircleHandleCap(controlID, position, transform.rotation, size * 0.9f, eventType);
        Handles.CircleHandleCap(controlID, position + transform.forward * z, transform.rotation, sizeFactor * focusCircleSize, eventType);
    }
    void CamZCapture(int controlID, Vector3 position, Quaternion rotation, float z, EventType eventType)
    {
        var sizeFactor = HandleUtility.GetHandleSize(position);

        Handles.ArrowHandleCap(controlID, position + transform.forward * (sizeFactor * 0.4f), Quaternion.LookRotation(-transform.forward), sizeFactor * camZSize, eventType);
    }
    void CamFovCapture(int controlID, Vector3 position, Quaternion rotation, float fov, EventType eventType)
    {
        var sizeFactor = HandleUtility.GetHandleSize(position);
        var angleRot = Quaternion.AngleAxis(fov * 2, -transform.forward);

        Handles.CubeHandleCap(controlID, position, angleRot * transform.rotation, camFovCaptureSize * sizeFactor, eventType);
    }
}
