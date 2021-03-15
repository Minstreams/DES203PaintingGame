using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;

namespace GameSystem.Requirements
{
    [CustomEditor(typeof(RequirementsManagerData))]
    public class RequirementsManagerDataEditor : Editor
    {
        int fieldIndex;
        string styleString;
        RequirementsManagerData Data => target as RequirementsManagerData;

        List<FieldInfo> fieldList;
        string[] fieldListTextArray;
        private void OnEnable()
        {
            fieldList = new List<FieldInfo>(typeof(RequirementsManagerData).GetFields());
            for (int i = fieldList.Count - 1; i >= 0; --i)
            {
                if (fieldList[i].FieldType != typeof(GUIStyle)) fieldList.RemoveAt(i);
            }
            var fieldListText = new List<string>();
            foreach (var f in fieldList)
            {
                fieldListText.Add(f.Name);
            }
            fieldListTextArray = fieldListText.ToArray();
        }
        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();
            base.OnInspectorGUI();
            if (EditorGUI.EndChangeCheck())
            {
                RequirementsManager.ActiveManager?.Repaint();
                RequirementsManager.Inspector?.Repaint();
            }

            GUILayout.Space(16);

            fieldIndex = EditorGUILayout.Popup(fieldIndex, fieldListTextArray);
            styleString = GUILayout.TextField(styleString);

            if (GUILayout.Button("Set"))
            {
                try
                {
                    fieldList[fieldIndex].SetValue(Data, new GUIStyle(styleString) {/* name = fieldListTextArray[fieldIndex]*/ });
                    RequirementsManager.ActiveManager?.Repaint();
                    RequirementsManager.Inspector?.Repaint();
                }
                catch
                {
                    RequirementsManager.ActiveManager?.ShowNotification(new GUIContent("Invalid"));
                }
            }
        }
    }
}