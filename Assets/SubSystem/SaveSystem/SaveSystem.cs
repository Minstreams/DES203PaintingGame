using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSystem.Setting;

namespace GameSystem
{
    /// <summary>
    /// Save & Load function
    /// </summary>
    public class SaveSystem : SubSystem<SaveSystemSetting>
    {
        public static SaveSlot currentSlot = new SaveSlot();

        public static List<SaveSlot> autoSlots = new List<SaveSlot>();
        public static List<SaveSlot> quickSlots = new List<SaveSlot>();
        public static List<SaveSlot> saveSlots = new List<SaveSlot>();



        // Your code here


        [RuntimeInitializeOnLoadMethod]
        static void RuntimeInit()
        {
            TheMatrix.OnGameStart += OnGameStart;
        }
        static void OnGameStart()
        {
            // 在System场景加载后调用
        }


        // API---------------------------------
        //public static void SomeFunction(){}
    }
}
