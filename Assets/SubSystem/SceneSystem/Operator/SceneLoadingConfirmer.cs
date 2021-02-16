using UnityEngine;

namespace GameSystem.Operator
{
    /// <summary>
    /// 用来响应场景加载事件
    /// </summary>
    [AddComponentMenu("[SceneSystem]/Operator/SceneLoadingConfirmer")]
    public class SceneLoadingConfirmer : MonoBehaviour
    {
        [MinsHeader("Operator of SceneSystem", SummaryType.PreTitleOperator, -1)]
        [MinsHeader("场景加载确认器", SummaryType.TitleOrange, 0)]
        [MinsHeader("用来响应场景加载事件", SummaryType.CommentCenter, 1)]

        // Data
        [MinsHeader("Data", SummaryType.Header, 2)]
        [Label(true)] public SceneCode sceneToLoad;

        // Output
        public SimpleEvent onPendingLoadScene;

        // Input
        public void Confirm() => SceneSystem.ConfirmLoadScene();

        void OnPendingLoadScene()
        {
            if (SceneSystem.SceneToLoad == sceneToLoad) onPendingLoadScene?.Invoke();
        }

        void Start()
        {
            SceneSystem.OnPendingLoadScene += OnPendingLoadScene;
        }
        void OnDestroy()
        {
            SceneSystem.OnPendingLoadScene -= OnPendingLoadScene;
        }
    }
}