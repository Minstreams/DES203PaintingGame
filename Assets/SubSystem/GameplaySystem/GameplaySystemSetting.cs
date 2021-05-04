using UnityEngine;

namespace GameSystem.Setting
{
    [CreateAssetMenu(fileName = "GameplaySystemSetting", menuName = "系统配置文件/GameplaySystemSetting")]
    public class GameplaySystemSetting : ScriptableObject
    {
        [MinsHeader("GameplaySystem Setting", SummaryType.Title, -2)]
        [MinsHeader("Gameplay System. Provides global settings for gameplay.", SummaryType.CommentCenter, -1)]

        [MinsHeader("Camera Relevant")]
        [Label(true)] public float sideScrollingPathWidth;
        [Label(true)] public float invisibleWallDepth;
        [Label(true)] public float invisibleWallHeight;
        [Label] public float playerFocusHeight;
        [Label] public float playerBattleHeight;
        [LabelRange] public float camInterpolatingRate;
        [LabelRange] public float camInterpolatingRateMouseRotation;
        [Label(true)] public Vector3 camDefaultRelativePostiion;
        [Label(true)] public float camDefaultFov;
        [Label] public float camReactDefaultPower;

        [MinsHeader("Combat")]
        [LabelRange] public float attackPowerRateY;
        [Label] public float brushAppearTime;

        [MinsHeader("Journal")]
        [Label(true)] public Sprite[] journalPages;

        // Temp
        [MinsHeader("Data", SummaryType.Header)]
        [Label] public float groundDrag = 2f;


        [MinsHeader("Player Data")]
        [Label] public bool hasPinkCrystal;
        [Label] public bool hasRedCrystal;
        [Label] public bool hasGreenCrystal;
        [Label] public bool hasYellowCrystal;
    }
}