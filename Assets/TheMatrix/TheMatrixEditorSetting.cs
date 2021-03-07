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

        [Label("进行完整测试")] public bool fullTest;
        [Label("测试文件保存")] public bool saveData;
        [Label("调试日志")] public bool debug;
    }
}