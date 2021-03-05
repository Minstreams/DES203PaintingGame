using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSystem.Setting;

namespace GameSystem
{
    /// <summary>
    /// Gameplay System. Provides global settings for gameplay.
    /// </summary>
    public class GameplaySystem : SubSystem<GameplaySystemSetting>
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
        //public static void SomeFunction(){}


        public static PlayerAvatar CurrentPlayer { get; set; } = null;
    }
}
