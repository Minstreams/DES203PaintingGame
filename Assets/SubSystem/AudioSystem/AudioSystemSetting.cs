using UnityEngine;
using UnityEngine.Audio;

namespace GameSystem.Setting
{
    [CreateAssetMenu(fileName = "AudioSystemSetting", menuName = "系统配置文件/AudioSystemSetting")]
    public class AudioSystemSetting : ScriptableObject
    {
        [MinsHeader("AudioSystem Setting", SummaryType.Title, -2)]
        [MinsHeader("Audio System", SummaryType.CommentCenter, -1)]

        [MinsHeader("Data", SummaryType.Header)]
        [Label] public AudioMixer mixer;
    }
}