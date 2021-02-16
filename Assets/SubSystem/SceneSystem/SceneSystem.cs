using GameSystem.Setting;
using UnityEngine.SceneManagement;
using System.Collections;

namespace GameSystem
{
    /// <summary>
    /// 场景系统，用于加载卸载转换场景
    /// </summary>
    public class SceneSystem : SubSystem<SceneSystemSetting>
    {
        static bool loadConfirmed;
        public static event System.Action OnPendingLoadScene;
        public static SceneCode SceneToLoad { get; private set; }


        // API---------------------------------
        public static string GetScene(SceneCode sceneCode) => Setting.sceneCodeMap[sceneCode];
        public static void LoadScene(SceneCode sceneCode) => SceneManager.LoadScene(GetScene(sceneCode));
        public static void ConfirmLoadScene() => loadConfirmed = true;
        public static IEnumerator LoadSceneCoroutine(SceneCode sceneCode)
        {
            loadConfirmed = false;
            SceneToLoad = sceneCode;
            OnPendingLoadScene?.Invoke();
            Log("Pending Load Scene:" + sceneCode);
            while (!loadConfirmed) yield return 0;
            Log("Load Confirmed!");
            SceneManager.LoadScene(GetScene(sceneCode));
        }
    }
}
