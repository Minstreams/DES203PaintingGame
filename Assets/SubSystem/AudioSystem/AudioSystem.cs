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
        // Your code here


        [RuntimeInitializeOnLoadMethod]
        static void RuntimeInit()
        {
            TheMatrix.OnGameStart += OnGameStart;
        }
        static void OnGameStart()
        {
            // 在System场景加载后调用
        }


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
    }
}
