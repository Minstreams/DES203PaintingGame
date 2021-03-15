using System.Collections;
using GameSystem.Setting;
using System;
using UnityEngine;

namespace GameSystem
{
    public partial class GameFlowSystem : SubSystem<GameFlowSystemSetting>
    {
        // 游戏流程 -------------
        public static event Action OnFlowStart;

#if UNITY_EDITOR
        static void QuickTest()
        {
            OnFlowStart?.Invoke();
            UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(0);
        }
#endif
        static IEnumerator Start()
        {
            OnFlowStart?.Invoke();
            yield return 0;
            StartCoroutine(CheckExit());
            StartCoroutine(Logo());
        }
        static IEnumerator CheckExit()
        {
            while (true)
            {
                yield return 0;
                if (GetGameMessage(GameMessage.Exit))
                {
                    TheMatrix.canQuit = true;
                    Application.Quit();
                    yield break;
                }
            }
        }
        static IEnumerator Logo()
        {
            SceneSystem.LoadScene(SceneCode.logo);
            yield return 0;

            ResetGameMessage();
            while (true)
            {
                yield return 0;
                if (GetGameMessage(GameMessage.Next)) break;
            }

            StartCoroutine(StartMenu());
        }
        static IEnumerator StartMenu()
        {
            SceneSystem.LoadScene(SceneCode.startMenu);
            yield return 0;

            ResetGameMessage();
            while (true)
            {
                yield return 0;
                if (GetGameMessage(GameMessage.Start)) break;
            }

            StartCoroutine(InGame());
        }

        static IEnumerator InGame()
        {
            yield return SceneSystem.LoadSceneCoroutine(SceneCode.museum);
            yield return 0;

            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;

            ResetGameMessage();
            while (true)
            {
                yield return 0;
                if (GetGameMessage(GameMessage.Return))
                {
                    StartCoroutine(StartMenu());
                    break;
                }
                if (GetGameMessage(GameMessage.Pause))
                {
                    StartCoroutine(Pause());
                    break;
                }
            }
        }
        static IEnumerator Pause()
        {
            yield return 0;

            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

            ResetGameMessage();
            while (true)
            {
                yield return 0;
                if (GetGameMessage(GameMessage.Return))
                {
                    StartCoroutine(StartMenu());
                    break;
                }
                if (GetGameMessage(GameMessage.Resume))
                {
                    StartCoroutine(InGame());
                    break;
                }
            }
        }
    }
}