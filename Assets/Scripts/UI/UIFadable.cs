using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameSystem.UI
{
    /// <summary>
    /// Fadable Unit
    /// </summary>
    public abstract class UIFadable : MonoBehaviour
    {
        public abstract void Fadein();
        public abstract void Fadeout();
        public abstract void Hide();
    }
}