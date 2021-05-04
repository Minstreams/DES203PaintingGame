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
            Cursor.visible = false;
            SceneSystem.LoadScene(SceneCode.logo);
            SceneSystem.ConfirmLoadScene();
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
            yield return SceneSystem.LoadSceneCoroutine(SceneCode.startMenu);

            GameplaySystem.IsPaused = false;
            Time.timeScale = 1;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            yield return 0;

            ResetGameMessage();
            while (true)
            {
                yield return 0;
                if (GetGameMessage(GameMessage.Start)) break;
            }

            yield return SceneSystem.LoadSceneCoroutine(SceneCode.museum);
            StartCoroutine(InGame());
        }

        static IEnumerator InGame()
        {
            yield return 0;

            GameplaySystem.IsPaused = false;
            Time.timeScale = 1;
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
                if (GetGameMessage(GameMessage.GameOver))
                {
                    StartCoroutine(GameOver());
                    break;
                }
            }
        }
        static IEnumerator Pause()
        {
            yield return 0;

            GameplaySystem.IsPaused = true;
            Time.timeScale = 0;
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
        static IEnumerator GameOver()
        {
            yield return SceneSystem.LoadSceneCoroutine(SceneCode.GameOver);
            yield return 0;

            GameplaySystem.IsPaused = false;
            Time.timeScale = 1;
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
                if (GetGameMessage(GameMessage.Portal))
                {
                    yield return SceneSystem.LoadSceneCoroutine(SceneCode.museum);
                    StartCoroutine(InGame());
                    break;
                }
            }
        }
    }
}