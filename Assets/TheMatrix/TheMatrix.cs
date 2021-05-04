using UnityEngine;
using GameSystem.Setting;
using GameSystem.Savable;

namespace GameSystem
{
    /// <summary>
    /// 母体，基本框架
    /// </summary>
    [DisallowMultipleComponent]
    public partial class TheMatrix : MonoBehaviour
    {
        static TheMatrix instance;
        public static TheMatrix Instance
        {
            get
            {
#if UNITY_EDITOR
                if (!UnityEditor.EditorApplication.isPlaying)
                {
                    Dialog("在编辑器状态下调用游戏代码很危险，请在游戏中调用");
                    return null;
                }
#endif
                if (instance == null)
                {
                    Error("没有加载TheMatrix！");
                }
                return instance;
            }
        }
        /// <summary>
        /// 配置引用
        /// </summary>
        public static TheMatrixSetting Setting => setting == null ? setting = Resources.Load<TheMatrixSetting>("TheMatrixSetting") : setting;
        static TheMatrixSetting setting;

#if UNITY_EDITOR
        /// <summary>
        /// References for editor setting
        /// </summary>
        public static TheMatrixEditorSetting EditorSetting => editorSetting == null ? editorSetting = (TheMatrixEditorSetting)UnityEditor.EditorGUIUtility.Load("TheMatrixEditorSetting.asset") : editorSetting;
        static TheMatrixEditorSetting editorSetting;
#endif

        /// <summary>
        /// 游戏初始化委托，在进入System场景时调用
        /// </summary>
        public static event System.Action OnGameStart;
        public static event System.Action OnQuitting;

        public static bool canQuit;

        void Awake()
        {
            instance = this;
            _ = Setting;
            Application.targetFrameRate = Setting.targetFrameRate;
            Application.wantsToQuit += () =>
            {
                OnQuitting?.Invoke();
                return canQuit;
            };
        }
        void Start()
        {
            DontDestroyOnLoad(gameObject);
            OnGameStart?.Invoke();
#if UNITY_EDITOR
            if (!EditorSetting.saveData)
            {
                foreach (SavableObject so in Setting.dataAutoSave)
                {
                    so.ApplyData();
                    Log("All Savable applied.");
                }
            }
            else
#endif
                LoadAll();
        }
        void OnDestroy()
        {
#if UNITY_EDITOR
            if (!EditorSetting.saveData)
            {
                foreach (SavableObject so in Setting.dataAutoSave)
                {
                    so.UpdateData();
                    UnityEditor.EditorUtility.SetDirty(so);
                }
                UnityEditor.AssetDatabase.SaveAssets();
                Log("All Savable updated. And assets saved.");
            }
            else
#endif
                SaveAll();
        }
    }
}
