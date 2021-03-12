using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace GameSystem.Requirements
{
    public class RequirementsManager : EditorWindow
    {
        const string windowOpenLocker = "RequirementsManagerLocker";
        public static RequirementsManagerData Data => data == null ? data = (RequirementsManagerData)EditorGUIUtility.Load("RequirementsManagerData.asset") : data;
        static RequirementsManagerData data;

        public static RequirementsManager activeManager;

        public static RequirementsManagerInspector Inspector
        {
            get
            {
                if (activeManager == null) return null;
                if (inspector == null)
                {
                    inspector = GetWindow<RequirementsManagerInspector>();
                    inspector.titleContent = new GUIContent(Data.inspectorTitle, Data.inspectorIcon);
                }
                return inspector;
            }
        }
        static RequirementsManagerInspector inspector;

        // Fields ==================================================
        public List<Requirement> reqList = new List<Requirement>();
        public Requirement selectedReq;

        Dictionary<Requirement, bool> fileExists = new Dictionary<Requirement, bool>();

        Vector2 scrollPos;
        const float topRectHeight = 64;

        void RequirementField(Rect pos, Requirement req)
        {
            var selectionPos = new Rect(pos.x, pos.y + Data.statusPaddingY, pos.width, pos.height - Data.statusPaddingY * 2);
            GUI.color = selectedReq == req ? Data.selectedColor : Data.unselectedColor;
            if (GUI.Button(selectionPos, GUIContent.none, Data.selectionStyle))
            {
                selectedReq = req;
                Inspector.Repaint();
            }


            var statusPos = new Rect(pos.x, pos.y + Data.statusPaddingY, Data.statusWidth, pos.height - Data.statusPaddingY * 2);
            GUI.color = GetStatusColor(req);
            GUI.Box(statusPos, new GUIContent(GetStatusTex(req)), Data.statusStyle);

            var pathPos = new Rect(pos.x + Data.statusWidth + Data.reqNameWidth, pos.y + Data.statusPaddingY, pos.width - Data.responsiblePersonWidth - Data.reqNameWidth - Data.statusWidth, statusPos.height);
            GUI.Box(pathPos, new GUIContent(req.path), Data.pathBoxStyle);

            var namePos = new Rect(pos.x + Data.statusWidth, pos.y, Data.reqNameWidth, pos.height);
            GUI.color = GetPriorityColor(req);
            GUI.Box(namePos, new GUIContent(req.name), Data.nameBoxStyle);


            var personPos = new Rect(pos.x + pos.width - Data.responsiblePersonWidth, pos.y, Data.responsiblePersonWidth, pos.height);
            GUI.color = Color.white;
            GUI.Box(personPos, string.IsNullOrWhiteSpace(req.responsiblePerson) ? new GUIContent(Data.responsiblePersonTex) : new GUIContent(req.responsiblePerson), Data.responsiblePersonStyle);

            GUI.color = Color.white;
        }

        void OnGUI()
        {

            var windowRect = new Rect(Vector2.zero, position.size);

            var topRect = new Rect(0, 0, windowRect.size.x, topRectHeight);



            var scrollRect = new Rect(0, topRectHeight, windowRect.size.x, windowRect.size.y - topRectHeight);
            GUI.Box(scrollRect, GUIContent.none, "window");
            scrollPos = GUI.BeginScrollView(scrollRect, scrollPos, new Rect(scrollPos, scrollRect.size));
            {
                var reqRect = new Rect(Data.reqRectMargin, new Vector2(scrollRect.width - Data.reqRectMargin.x * 2, Data.reqLabelHeight));
                foreach (var req in Data.requirementList)
                {
                    RequirementField(reqRect, req);
                    reqRect.y += Data.reqRectMargin.y + Data.reqLabelHeight;
                }
            }
            GUI.EndScrollView();
        }

        void OnEnable()
        {
            activeManager = this;
            RefreshList();
        }

        void OnSelectionChange()
        {
            Repaint();
        }

        #region API
        public Color GetPriorityColor(Requirement req)
        {
            switch (req.priority)
            {
                case RequirementPriority.optional: return Data.optionalColor;
                case RequirementPriority.normal: return Data.normalColor;
                default: return Data.urgentColor;
            }
        }
        public string GetPriorityText(Requirement req)
        {
            switch (req.priority)
            {
                case RequirementPriority.optional: return "Optional";
                case RequirementPriority.normal: return "Normal";
                default: return "Urgent";
            }
        }
        public Color GetStatusColor(Requirement req)
        {
            if (!fileExists.ContainsKey(req) || !fileExists[req]) return Data.pendingColor;
            switch (req.status)
            {
                case RequirementStatus.@unchecked: return Data.uncheckedColor;
                case RequirementStatus.@checked: return Data.checkedColor;
                default: return Data.stableColor;
            }
        }
        public Texture GetStatusTex(Requirement req)
        {
            if (!fileExists.ContainsKey(req) || !fileExists[req]) return Data.pendingTex;
            switch (req.status)
            {
                case RequirementStatus.@unchecked: return Data.uncheckedTex;
                case RequirementStatus.@checked: return Data.checkedTex;
                default: return Data.stableTex;
            }
        }
        public string GetStatusText(Requirement req)
        {
            if (!fileExists.ContainsKey(req) || !fileExists[req]) return "Unfinished";
            switch (req.status)
            {
                case RequirementStatus.@unchecked: return "Unchecked";
                case RequirementStatus.@checked: return "Checked";
                default: return "Stable";
            }
        }
        public void RefreshList()
        {
            fileExists.Clear();
            reqList = new List<Requirement>(Data.requirementList);

            // apply the filters


            // detect file
            foreach (var r in reqList)
            {
                fileExists.Add(r, System.IO.File.Exists("Assets" + r.path));
            }
        }
        public void NewRequirement()
        {
            var req = new Requirement();
            Data.requirementList.Add(req);
            selectedReq = req;
            RefreshList();
        }
        #endregion


        #region Static Window Functions
        [InitializeOnLoadMethod]
        static void OpenWindowOnLoad()
        {
            if (SessionState.GetBool(windowOpenLocker, false)) return;
            OpenWindow();
            SessionState.SetBool(windowOpenLocker, true);
        }

        [MenuItem("MatrixTool/Requirements Manager _F1", false, 92)]
        static void OpenWindow()
        {
            var window = GetWindow<RequirementsManager>();
            window.titleContent = new GUIContent(Data.managerTitle, Data.managerIcon);
        }
        #endregion

    }

}