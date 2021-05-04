using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSystem.Setting;

namespace GameSystem
{
    /// <summary>
    /// Audio System
    /// </summary>
    public class AudioSystem : SubSystem<AudioSystemSetting>
    {
        [RuntimeInitializeOnLoadMethod]
        static void RuntimeInit()
        {
            TheMatrix.OnGameStart += OnGameStart;
        }
        static void OnGameStart()
        {
            musicSource = TheMatrix.Instance.GetComponent<AudioSource>();
        }

        static AudioSource musicSource;


        // API---------------------------------
        public static void SetVolume(string name, float value)
        {
            Setting.mixer.SetFloat(name, value * 0.8f - 80);
        }
        public static float GetVolume(string name)
        {
            float res;
            Setting.mixer.GetFloat(name, out res);
            return (res + 80) / 0.8f;
        }
        public static void ChangeMusic(AudioCode audioCode)
        {
            var clip = Setting.audioMap[audioCode];
            if (musicSource == null) musicSource = TheMatrix.Instance.GetComponent<AudioSource>();
            if (audioCode == AudioCode.None)
            {
                musicSource.Stop();
                return;
            }
            if (musicSource.clip == clip) return;
            musicSource.clip = clip;
            musicSource.Play();
        }
    }
}
