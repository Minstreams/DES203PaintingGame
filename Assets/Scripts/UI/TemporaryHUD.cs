using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSystem;
using GameSystem.UI;

public class TemporaryHUD : UIBase
{
    public GUIStyle pink;
    public GUIStyle red;
    public GUIStyle green;
    public GUIStyle yellow;
    protected override void OnUI()
    {
        GUILayout.Label("Health: " + (GameplaySystem.CurrentPlayer == null ? 0 : GameplaySystem.CurrentPlayer.Health));
        GUILayout.BeginHorizontal(boxStyle,GUILayout.MinHeight(32));
        {
            GUILayout.Label("");
            if (GameplaySystem.Setting.hasPinkCrystal) GUILayout.Label("P", pink);
            if (GameplaySystem.Setting.hasRedCrystal) GUILayout.Label("R", red);
            if (GameplaySystem.Setting.hasGreenCrystal) GUILayout.Label("G", green);
            if (GameplaySystem.Setting.hasYellowCrystal) GUILayout.Label("Y", yellow);
        }
        GUILayout.EndHorizontal();
    }
}
