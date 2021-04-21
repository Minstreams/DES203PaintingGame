using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameSystem.UI
{
    public class UIKeybinderHandler : MonoBehaviour
    {
        [Label] public InputKey key;
        [Label(true)] public UIKeybinder mainKey;
        [Label(true)] public UIKeybinder sideKey;
        void Awake()
        {
            mainKey.key = key;
            sideKey.key = key;
        }
    }
}