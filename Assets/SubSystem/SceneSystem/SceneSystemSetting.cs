using UnityEngine;

namespace GameSystem.Setting
{
    [CreateAssetMenu(fileName = "SceneSystemSetting", menuName = "系统配置文件/SceneSystemSetting")]
    public class SceneSystemSetting : ScriptableObject
    {
        [MinsHeader("SceneSystem Setting", SummaryType.Title, -2)]
        [MinsHeader("场景系统，用于加载卸载转换场景", SummaryType.CommentCenter, -1)]

        [MinsHeader("游戏场景表列", SummaryType.Header)]
        public SceneCodeMap sceneCodeMap;
    }
}