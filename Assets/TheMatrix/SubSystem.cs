using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameSystem
{
    /// <summary>
    /// 所有子系统的父类。
    /// </summary>
    /// <typeparam name="SubSetting">子系统设置类，必须继承自ScriptableObject</typeparam>
    public abstract class SubSystem<SubSetting> where SubSetting : ScriptableObject
    {
        static SubSetting _Setting;
        public static SubSetting Setting
        {
            get
            {
                if (_Setting == null)
                {
                    _Setting = Resources.Load<SubSetting>(typeof(SubSetting).ToString().Substring(19/*GameSystem.Setting.的长度*/));
                }
                return _Setting;
            }
        }

        /// <summary>
        /// 基于母体协程管理工程实现的功能，开始协程
        /// </summary>
        public static LinkedListNode<Coroutine> StartCoroutine(IEnumerator routine)
        {
            return TheMatrix.StartCoroutine(routine, typeof(SubSetting));
        }
        /// <summary>
        /// 基于母体协程管理工程实现的功能，停止所有协程
        /// </summary>
        public static void StopAllCoroutines()
        {
            TheMatrix.StopAllCoroutines(typeof(SubSetting));
        }
        /// <summary>
        /// 基于母体协程管理工程实现的功能，停止协程
        /// </summary>
        public static void StopCoroutine(LinkedListNode<Coroutine> node)
        {
            TheMatrix.StopCoroutine(node);
        }

        // debug -----------------------
        static string typeName;
        static string TypeName
        {
            get
            {
                if (string.IsNullOrEmpty(typeName))
                {
                    typeName = typeof(SubSetting).Name;
                    typeName = typeName.Substring(0, typeName.Length - 7);
                }
                return typeName;
            }
        }
        public static event Action<string> OnLog;
        public static void Log(string message)
        {
#if UNITY_EDITOR
            if (!TheMatrix.EditorSetting.debug) return;
#endif
            message = "【" + TypeName + "】" + message;
            Debug.Log(message);
            OnLog?.Invoke(message);
        }
        public static void LogError(string message)
        {
            message = "【" + TypeName + "】" + message;
            Debug.LogError(message);
            OnLog?.Invoke(message);
        }
        public static void LogError(Exception ex)
        {
            string message = "【" + TypeName + " | Exception】" + ex.GetType().Name + ":" + ex.Message + "\n" + ex.StackTrace;
            Debug.LogError(message);
            OnLog?.Invoke(message);
        }
        public static void LogAssertion(string message)
        {
            message = "【" + TypeName + "】" + message;
            Debug.LogAssertion(message);
            OnLog?.Invoke(message);
        }
        public static void LogWarning(string message)
        {
            message = "【" + TypeName + "】" + message;
            Debug.LogWarning(message);
            OnLog?.Invoke(message);
        }
        public static void Dialog(string message, string ok = "OK")
        {
            Log("Dialog:" + message);
            OnLog?.Invoke("Dialog:" + message);
#if UNITY_EDITOR
            UnityEditor.EditorUtility.DisplayDialog(TypeName, message, ok);
#endif
        }
    }
}