using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameSystem.Operator
{
    /// <summary>
    /// Change Music on scene loaded
    /// </summary>
    [AddComponentMenu("[AudioSystem]/Operator/MusicChanger")]
    public class MusicChanger : MonoBehaviour
    {
        [MinsHeader("Operator of AudioSystem", SummaryType.PreTitleOperator, -1)]
        [MinsHeader("MusicChanger", SummaryType.TitleOrange, 0)]
        [MinsHeader("Change Music on scene loaded", SummaryType.CommentCenter, 1)]
        [Label] public AudioCode audioCode;

        void Start()
        {
            AudioSystem.ChangeMusic(audioCode);
        }
    }
}