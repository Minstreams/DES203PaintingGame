using UnityEngine;

namespace GameSystem
{
    /// <summary>
    /// 游戏音频枚举，定义了游戏中的所有音频
    /// All scenes in this game
    /// </summary>
    public enum AudioCode
    {
        None,
        StartMenu,
        Serenity,
    }
    // EnumMap Class Definition (必须以Map作为名称结尾)
    [System.Serializable]
    public class AudioCodeMap : EnumMap<AudioCode, AudioClip> { }
}