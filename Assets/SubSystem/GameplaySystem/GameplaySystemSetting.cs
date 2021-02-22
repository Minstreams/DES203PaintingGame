using UnityEngine;

namespace GameSystem.Setting
{
    [CreateAssetMenu(fileName = "GameplaySystemSetting", menuName = "系统配置文件/GameplaySystemSetting")]
    public class GameplaySystemSetting : ScriptableObject
    {
        [MinsHeader("GameplaySystem Setting", SummaryType.Title, -2)]
        [MinsHeader("Gameplay System. Provides global settings for gameplay.", SummaryType.CommentCenter, -1)]

        [MinsHeader("Data", SummaryType.Header), Space(16)]
        [Label] public float groundDrag = 2f;
    }
}