using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameSystem.Operator
{

    [AddComponentMenu("[InputSystem]/Operator/InputKeyTrigger")]
    public class InputKeyTrigger : MonoBehaviour
    {
        [MinsHeader("Operator of InputSystem", SummaryType.PreTitleOperator, -1)]
        [MinsHeader("InputKeyTrigger", SummaryType.TitleOrange, 0)]
        [MinsHeader("", SummaryType.CommentCenter, 1)]
        [Label] public InputKey key;
        [Label] public SimpleEvent onKey;
        [Label] public SimpleEvent onKeyDown;
        [Label] public SimpleEvent onKeyUp;

        void Update()
        {
            if (InputSystem.GetKey(key)) onKey?.Invoke();
            if (InputSystem.GetKeyDown(key)) onKeyDown?.Invoke();
            if (InputSystem.GetKeyUp(key)) onKeyUp?.Invoke();
        }

    }
}