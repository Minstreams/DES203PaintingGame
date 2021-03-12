using UnityEngine;
using UnityEditor;

namespace GameSystem.Requirements
{
    public class RequirementUtility
    {
        public static RequirementsManagerData Data => RequirementsManager.Data;

    }
    public enum GUIPosition
    {
        settingPanel,
        displayPanel,
    }
}