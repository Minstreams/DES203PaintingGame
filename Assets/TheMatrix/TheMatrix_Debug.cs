using System;
using UnityEngine;

namespace GameSystem
{
    public partial class TheMatrix : MonoBehaviour
    {
        public static event Action<string> OnLog;
        public static void Log(string msg)
        {
#if UNITY_EDITOR
            if (!Setting.debug) return;
#endif
            msg = "【TheMatrix Debug】" + msg;
            Debug.Log(msg);
            OnLog?.Invoke(msg);
        }
        public static void Error(string msg)
        {
            msg = "【TheMatrix Debug】" + msg;
            Debug.LogError(msg);
            OnLog?.Invoke(msg);
        }
        public static void Dialog(string msg, string ok = "OK")
        {
            Log("Dialog:" + msg);
            OnLog?.Invoke("Dialog:" + msg);
#if UNITY_EDITOR
            UnityEditor.EditorUtility.DisplayDialog("The Matrix", msg, ok);
#endif
        }
    }
}