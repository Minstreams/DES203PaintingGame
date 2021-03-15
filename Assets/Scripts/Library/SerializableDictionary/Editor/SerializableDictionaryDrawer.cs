using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public abstract class SerializableDictionaryDrawer : PropertyDrawer
{
    private bool enabled;

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        int count = property.FindPropertyRelative("count").intValue;
        return enabled ? (count == 0 ? 2 : count + 1) * lh : lh;
    }

    float lh => EditorGUIUtility.singleLineHeight;
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        var foldRect = new Rect(position.x, position.y, position.width, lh);
        enabled = EditorGUI.Foldout(foldRect, enabled, label);

        if (enabled)
        {
            int count = property.FindPropertyRelative("count").intValue;
            SerializedProperty keyListProperty = property.FindPropertyRelative("keyList");
            SerializedProperty valueListProperty = property.FindPropertyRelative("valueList");
            if (count == 0)
            {
                position.y += lh;
                EditorGUI.LabelField(new Rect(position.x, position.y + lh, position.width, lh), "Empty!");
            }
            else
            {
                var labelRect = new Rect(position.x, position.y + lh, position.width, lh);
                for (int i = 0; i < count; i++)
                {
                    var valuePosition = EditorGUI.PrefixLabel(labelRect, GUIUtility.GetControlID(FocusType.Passive), new GUIContent(GetKeyString(keyListProperty.GetArrayElementAtIndex(i))));
                    EditorGUI.LabelField(valuePosition, GetValueString(valueListProperty.GetArrayElementAtIndex(i)));
                    labelRect.y += lh;
                }
            }
        }

        EditorGUI.EndProperty();
    }

    //针对不同Type，类似ToString，返回想显示在面板上的内容
    public abstract string GetKeyString(SerializedProperty property);
    public abstract string GetValueString(SerializedProperty property);
}


[CustomPropertyDrawer(typeof(TimestampDictionary))]
public class TimestampDictionaryDrawer : SerializableDictionaryDrawer
{
    public override string GetKeyString(SerializedProperty property)
    {
        return property.stringValue;
    }

    public override string GetValueString(SerializedProperty property)
    {
        return property.doubleValue.ToString();
    }
}
