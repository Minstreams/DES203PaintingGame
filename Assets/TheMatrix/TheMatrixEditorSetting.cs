using UnityEngine;

namespace GameSystem.Setting
{
    /// <summary>
    /// Editor only settings
    /// </summary>
    [CreateAssetMenu(fileName = "TheMatrixEditorSetting", menuName = "系统配置文件/TheMatrixEditorSetting")]
    public class TheMatrixEditorSetting : ScriptableObject
    {
        [MinsHeader("TheMatrix Editor Setting", SummaryType.Title, -2)]
        [MinsHeader("Editor only Settings", SummaryType.CommentCenter, -1)]

        [MinsHeader("Debug")]
        [Label("进行完整测试")] public bool fullTest;
        [Label("测试文件保存")] public bool saveData;
        [Label("调试日志")] public bool debug;

        [Separator]
        [MinsHeader("Side Scrolling Zone Editor", SummaryType.SubTitle, -1)]
        [MinsHeader("Resources")]
        [Label] public GameObject sideScrollingCameraPointPrefab;
        [Label] public Mesh characterMesh;
        [MinsHeader("Status")]
        [Label] public bool alwaysShowCameraPanelInSceneView = false;
        [Label] public bool alwaysShowCameraPreviewInSceneView = true;
        [Label] public bool cameraPitchAlwayEditable = false;
        [Label] public float snapValue = 1f;
        [Label] public float angleSnapValue = 5f;
        [MinsHeader("GUI Parameters")]
        [Label] public float entryPointCaptureSize = 0.2f;
        [Label] public float entryPointArrowSize = 0.1f;
        [Label] public float focusCircleSize = 0.1f;
        [Label] public float camPosSize = 0.5f;
        [Label] public float camZSize = 0.7f;
        [Label] public float camFovArcRadius = 0.35f;
        [Label] public float camFovCaptureRadius = 0.7f;
        [Label] public float camFovCaptureSize = 0.1f;
        [Label] public float camGUIPanelWidth = 156;
        [Label] public float camGUIPanelHeight = 276;
        [Label] public int camPreviewHeight = 100;
        [Label] public int gridGroundRadius = 10;
        [Label] public int gridSideRadius = 6;
        [Label] public int gridZAxisRadius = 4;
        [Label] public int gridFovRange = 60;
        [MinsHeader("GUI Colors")]
        [Label] public Color entryPointCaptureColor = new Color(0, 1, 0, 1);
        [Label] public Color entryPointArrowColor = new Color(0, 1, 0, 1);
        [Label] public Color cameraOverflowLineColor = new Color(1f, 0.3f, 0, 0.6f);
        [Label] public Color gridGroundColor = new Color(0.8f, 0.8f, 0.6f, 1);
        [Label] public Color gridSideColor = new Color(0.8f, 0.8f, 0.6f, 1);
        [Label] public Color gridZAxisColor = new Color(1, 1, 1, 1);
        [Label] public Color gridFovColor = new Color(1, 1, 1, 1);
    }
}