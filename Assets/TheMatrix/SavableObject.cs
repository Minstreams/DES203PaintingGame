﻿using UnityEngine;

namespace GameSystem.Savable
{
    /// <summary>
    /// 可保存的文件，用于和TheMatrix的存档控制功能交互
    /// </summary>
    public abstract class SavableObject : ScriptableObject
    {
        /// <summary>
        /// The relative path of data on disk
        /// </summary>
        public abstract string GetPath();
        /// <summary>
        /// 读取时将调用此方法，用于将对象中的数据应用到游戏中
        /// </summary>
        [ContextMenu("ApplyData")]
        public abstract void ApplyData();
        /// <summary>
        /// 保存时将调用此方法，用于生成对象中的数据
        /// </summary>
        [ContextMenu("UpdateData")]
        public abstract void UpdateData();
        [ContextMenu("LoadDefault")]
        public abstract void LoadDefault();
    }
}