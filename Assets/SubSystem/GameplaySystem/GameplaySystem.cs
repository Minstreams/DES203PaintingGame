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
            // TODO: 设定不同物理层的碰撞权限
        }



        public static PlayerAvatarController CurrentPlayer { get; set; } = null;
        public static CameraController CurrentCamera { get; set; } = null;


        public static float CalculateCameraPointWeight(float distance)
        {
            return 1 / (distance + 0.1f);
        }
    }
}
