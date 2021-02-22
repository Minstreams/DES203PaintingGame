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

            StartCoroutine(EnteringMuseum());
        }
        static IEnumerator EnteringMuseum()
        {
            yield return SceneSystem.LoadSceneCoroutine(SceneCode.enteringMuseum);
            yield return 0;

            ResetGameMessage();
            while (true)
            {
                yield return 0;
                if (GetGameMessage(GameMessage.Next)) break;
            }

            StartCoroutine(Museum());
        }
        static IEnumerator Museum()
        {
            yield return SceneSystem.LoadSceneCoroutine(SceneCode.museum);
            yield return 0;

            ResetGameMessage();
            while (true)
            {
                yield return 0;
                if (GetGameMessage(GameMessage.Start))
                {
                    StartCoroutine(EnteringPainting(1));
                    break;
                }
            }
        }

        static IEnumerator EnteringPainting(int index)
        {
            yield return SceneSystem.LoadSceneCoroutine(SceneCode.enteringPainting);
            yield return 0;

            ResetGameMessage();
            while (true)
            {
                yield return 0;
                if (GetGameMessage(GameMessage.Next)) break;
            }

            switch (index)
            {
                case 1:
                    StartCoroutine(Painting1());
                    break;
                default: break;
            }
        }
        static IEnumerator Painting1()
        {
            yield return SceneSystem.LoadSceneCoroutine(SceneCode.painting1);
            yield return 0;

            ResetGameMessage();
            while (true)
            {
                yield return 0;
            }
        }
    }
}