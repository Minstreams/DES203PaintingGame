using UnityEngine;

namespace GameSystem.UI
{
    [AddComponentMenu("|UI/DebugPrinter")]
    public class DebugPrinter : UIBase
    {
        [MinsHeader("调试器节点", SummaryType.TitleYellow, 0)]
        [MinsHeader("调用 Print 方法，可以在控制台输出各种类型的数据", SummaryType.CommentCenter, 1)]

        [ConditionalShow(AlwaysShow = true, Label = "输出")]
        public string debugStr = "";

        // Input
        public void Print(string val) { Debug.Log(val); debugStr = val.ToString(); }
        public void Print(int val) { Print(val.ToString()); }
        public void Print(float val) { Print(val.ToString()); }
        public void Print(Vector2 val) { Print(val.ToString()); }
        public void Print(Vector3 val) { Print(val.ToString()); }
        public void Print(Color val) { Print(val.ToString()); }

        string preStr = "";
        void Start()
        {
            var g = transform;
            while (g != null)
            {
                preStr = "/" + g.name + preStr;
                g = g.parent;
            }
            preStr = gameObject.scene.name + preStr;
        }
        protected override void OnUI()
        {
            GUILayout.Label("[" + preStr + "]" + debugStr);
        }
    }
}