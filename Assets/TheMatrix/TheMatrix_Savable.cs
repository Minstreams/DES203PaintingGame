using UnityEngine;
using GameSystem.Savable;

namespace GameSystem
{
    public partial class TheMatrix : MonoBehaviour
    {
        // 存档控制----------------------------
        static void SaveTemporary(SavableObject data)
        {
            // 此方法将数据保存到内存，但不保存到磁盘
            data.UpdateData();
            string stream = JsonUtility.ToJson(data);
            PlayerPrefs.SetString(data.ToString(), stream);
            Log(data.name + " \tsaved!");
        }

        [ContextMenu("Save All Data")]
        public void SaveAll()
        {
            if (Setting.dataAutoSave == null || Setting.dataAutoSave.Length == 0) return;
            foreach (SavableObject so in Setting.dataAutoSave)
            {
                SaveTemporary(so);
            }
            PlayerPrefs.Save();
            Log("Data saved to disc.");
        }
        public void LoadAll()
        {
            foreach (SavableObject so in Setting.dataAutoSave)
            {
                Load(so);
            }
        }
        [ContextMenu("Delete All Data")]
        public void DeleteAll()
        {
            PlayerPrefs.DeleteAll();
            PlayerPrefs.Save();
            Dialog("All saved data deleted!");
        }

        /// <summary>
        /// 手动保存一个对象
        /// </summary>
        public static void Save(SavableObject data)
        {
            SaveTemporary(data);
            PlayerPrefs.Save();
            Log("Data saved to disc.");
        }
        /// <summary>
        /// 手动读取一个对象
        /// </summary>
        public static void Load(SavableObject data)
        {
            if (!PlayerPrefs.HasKey(data.ToString()))
            {
                Log("No data found for " + data.name);
                data.LoadDefault();
                data.ApplyData();
                return;
            }
            string stream = PlayerPrefs.GetString(data.ToString());
            JsonUtility.FromJsonOverwrite(stream, data);
            data.ApplyData();
            Log(data.name + " \tloaded!");
        }
    }
}
