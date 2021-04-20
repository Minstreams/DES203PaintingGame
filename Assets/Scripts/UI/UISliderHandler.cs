using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameSystem.UI
{
    public class UISliderHandler : MonoBehaviour
    {
        [Label] public Slider slider;
        [Label] public Text valueLabel;
        [Label] public Vector2 range = Vector2.up;

        System.Action<float> onValueChanged = null;

        public void Bind(float val, System.Action<float> valueSetAction)
        {
            slider.minValue = range.x;
            slider.maxValue = range.y;
            slider.value = val;
            slider.onValueChanged.AddListener(OnValueChanged);
            valueLabel.text = val.ToString("0.0");
            onValueChanged = valueSetAction;
        }

        void OnValueChanged(float val)
        {
            valueLabel.text = val.ToString("0.0");
            onValueChanged?.Invoke(val);
        }
    }
}
