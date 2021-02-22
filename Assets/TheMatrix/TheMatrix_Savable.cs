using UnityEngine;
using GameSystem.Savable;
using System.IO;

namespace GameSystem
{
    public partial class TheMatrix : MonoBehaviour
    {
        // File operation----------------------
        public static string LoadFromFile(string path)
        {
            string res = "";
            try
            {
                StreamReader fr = new StreamReader(path);
                res = fr.ReadToEnd();
                fr.Close();
                Log($"data 【{res}】 loaded from 【{path}】");
            }
            catch (FileNotFoundException)
            {
                Error("File not found: " + path);
            }
            catch (DirectoryNotFoundException)
            {
                Error("Directory not found: " + path);
            }
            return res;
        }
        public static void SaveToFile(string path, string data)
        {
            StreamWriter fw = new StreamWriter(path);
            fw.Write(data);
            fw.Close();
            Log($"data 【{data}】 saved to 【{path}】");
        }
        public static void DeleteFile(string path)
        {
            if (!File.Exists(path))
            {
                Log($"data at path 【{path}】 already deleted");
                return;
            }
            File.Delete(path);
            Log($"data at path 【{path}】 deleted");
        }

        // 存档控制----------------------------
        /// <summary>
        /// 手动保存一个对象
        /// </summary>
        public static void Save(SavableObject data)
        {
            data.UpdateData();
            string stream = JsonUtility.ToJson(data);
            SaveToFile(data.GetPath(), stream);
            Log(data.name + " \tsaved!");
        }
        /// <summary>
        /// 手动读取一个对象
        /// </summary>
        public static void Load(SavableObject data)
        {
            if (!File.Exists(data.GetPath()))
            {
                Log("No data found for " + data.name);
                data.LoadDefault();
                data.ApplyData();
                return;
            }
            string stream = LoadFromFile(data.GetPath());
            JsonUtility.FromJsonOverwrite(stream, data);
            data.ApplyData();
            Log(data.name + " \tloaded!");
        }
        public static void Clear(SavableObject data)
        {
            DeleteFile(data.GetPath());
            Log(data.name + " \tcleared!");
        }

        [ContextMenu("Save All Data")]
        public void SaveAll()
        {
            if (Setting.dataAutoSave == null || Setting.dataAutoSave.Length == 0) return;
            foreach (SavableObject so in Setting.dataAutoSave)
            {
                Save(so);
            }
        }
        [ContextMenu("Load All Data")]
        public void LoadAll()
        {
            foreach (SavableObject so in Setting.dataAutoSave)
            {
                Load(so);
            }
        }
        [ContextMenu("Clear All Data")]
        public void ClearAll()
        {
            foreach (SavableObject so in Setting.dataAutoSave)
            {
                Clear(so);
            }
        }
    }
}
