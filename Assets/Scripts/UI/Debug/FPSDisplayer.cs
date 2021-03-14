using UnityEngine;

namespace GameSystem.UI
{
    [AddComponentMenu("|UI/FPSDisplayer")]
    public class FPSDisplayer : UIBase
    {
        int counter = 0;
        float timer = 0;
        int fps = 0;

        void Update()
        {
            ++counter;
            timer -= Time.deltaTime;
            if (timer < 0)
            {
                timer = 1;
                fps = counter;
                counter = 0;
            }
        }
        protected override void OnUI()
        {
            GUILayout.Label("FPS: " + fps);
        }
    }
}