using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace GameSystem.Requirements
{
    [CustomEditor(typeof(RequirementsManagerData))]
    public class RequirementsManagerDataEditor : Editor
    {
        RequirementsManagerData Data => target as RequirementsManagerData;
        public override void OnInspectorGUI()
        {
            foreach (var req in Data.requirementSet)
            {

            }
            base.OnInspectorGUI();
        }
    }
}