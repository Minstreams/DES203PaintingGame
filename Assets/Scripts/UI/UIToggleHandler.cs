using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameSystem.UI
{
    public class UIToggleHandler : MonoBehaviour
    {
        [Label] public Toggle toggle;

        public void Bind(bool val, System.Action<bool> valueSetAction)
        {
            toggle.isOn = val;
            toggle.onValueChanged.AddListener(v => valueSetAction?.Invoke(v));
        }
    }
}