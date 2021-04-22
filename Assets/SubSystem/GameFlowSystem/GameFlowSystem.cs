using UnityEngine;
using GameSystem.Setting;

namespace GameSystem
{
    /// <summary>
    /// 游戏流程控制与游戏消息系统
    /// </summary>
    public partial class GameFlowSystem : SubSystem<GameFlowSystemSetting>
    {
        /// <summary>
        /// 记录游戏控制信息
        /// </summary>
        static readonly bool[] gameMessageReciver = new bool[System.Enum.GetValues(typeof(GameMessage)).Length];


        [RuntimeInitializeOnLoadMethod]
        static void RuntimeInit()
        {
            TheMatrix.OnGameStart += OnGameStart;
            TheMatrix.OnQuitting += BeforeQuit;
        }
        static void OnGameStart()
        {
            Time.timeScale = 1;
            // 在System场景加载后调用
#if UNITY_EDITOR
            if (!TheMatrix.EditorSetting.fullTest) QuickTest();
            else
            {
#endif
                // full flow
                StartCoroutine(Start());
#if UNITY_EDITOR
            }
#endif
        }
        static void BeforeQuit()
        {
            Time.timeScale = 1;
            SendGameMessage(GameMessage.Exit);
        }


        // API---------------------------------
        /// <summary>
        /// 检查游戏控制信息，收到则返回true
        /// </summary>
        /// <param name="message">要检查的信息</param>
        /// <param name="reset">是否在接收后重置</param>
        /// <returns>检查按钮信息，收到则返回true</returns>
        public static bool GetGameMessage(GameMessage message, bool reset = true)
        {
            if (gameMessageReciver[(int)message])
            {
                if (reset)
                    gameMessageReciver[(int)message] = false;
                return true;
            }
            return false;
        }
        /// <summary>
        /// 发送 游戏控制信息
        /// </summary>
        /// <param name="message">信息</param>
        public static void SendGameMessage(GameMessage message)
        {
            Log("Receive Message: " + message);
            gameMessageReciver[(int)message] = true;
        }
        /// <summary>
        /// 重置
        /// </summary>
        public static void ResetGameMessage()
        {
            for (int i = 0; i < gameMessageReciver.Length; ++i) gameMessageReciver[i] = false;
        }
    }
}
