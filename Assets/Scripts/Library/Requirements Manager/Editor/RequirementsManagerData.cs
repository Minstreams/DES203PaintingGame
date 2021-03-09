using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameSystem.Requirements
{
    [CreateAssetMenu(menuName = "Requirements Manager/RequirementsManagerData", fileName = "RequirementsManagerData")]
    public class RequirementsManagerData : ScriptableObject
    {
        [MinsHeader("Requirements")]
        [Label] public SortedSet<Requirement> requirementSet = new SortedSet<Requirement>();

        [MinsHeader("GUIStyle")]
        [Label] public GUIStyle debugLogStyle;
        [Label] public string reqBoxStyle;
    }
}