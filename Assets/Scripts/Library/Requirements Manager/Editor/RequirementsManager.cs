using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Text.RegularExpressions;

namespace GameSystem.Requirements
{
    public class RequirementsManager : EditorWindow
    {
        const string windowOpenLocker = "RequirementsManagerLocker";
        const string allStr = "All";

        public static RequirementsManagerData Data => data == null ? data = (RequirementsManagerData)EditorGUIUtility.Load("RequirementsManagerData.asset") : data;
        static RequirementsManagerData data;

        public static RequirementsManagerLocalData LocalData
        {
            get
            {
                if (localData == null)
                {
                    if (!AssetDatabase.IsValidFolder("Assets/Editor Default Resources/Local"))
                    {
                        AssetDatabase.CreateFolder("Assets/Editor Default Resources", "Local");
                    }
                    if (!System.IO.File.Exists("Assets/Editor Default Resources/Local/RequirementsManagerLocalData.asset"))
                    {
                        localData = CreateInstance<RequirementsManagerLocalData>();
                        AssetDatabase.CreateAsset(localData, "Assets/Editor Default Resources/Local/RequirementsManagerLocalData.asset");
                    }
                    else
                    {
                        localData = (RequirementsManagerLocalData)EditorGUIUtility.Load("Local/RequirementsManagerLocalData.asset");
                    }
                }
                return localData;
            }
        }
        static RequirementsManagerLocalData localData;

        public static RequirementsManager ActiveManager { get; set; }

        public static RequirementsManagerInspector Inspector
        {
            get
            {
                if (ActiveManager == null) return null;
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
        public Requirement selectedReq;

        List<Requirement> reqList = new List<Requirement>();
        Dictionary<Requirement, bool> fileExists = new Dictionary<Requirement, bool>();

        Vector2 scrollPos;

        float topRectHeight => Data.basicHeight + (filterFoldout ? Data.filterHeight : 0) + (sorterFoldout ? Data.sorterHeight : 0);

        bool filterFoldout;
        string filterPath;
        bool ifFilterLocalPath;
        string filterKeyword;
        List<string> filterPersons = new List<string>();
        int filterPersonIndex;
        RequirementStatus filterStatus;
        RequirementPriority filterPriority;
        readonly Regex extEx = new Regex(".*\\.([^\\.]+)$");
        List<string> filterExtensions = new List<string>();
        int filterExtensionIndex;

        bool sorterFoldout;
        LinkedList<System.Comparison<Requirement>> sorters = new LinkedList<System.Comparison<Requirement>>();
        bool sorterStatusInverted;
        LinkedListNode<System.Comparison<Requirement>> sorterStatus;
        bool sorterPriorityInverted;
        LinkedListNode<System.Comparison<Requirement>> sorterPriority;
        bool sorterNameInverted;
        LinkedListNode<System.Comparison<Requirement>> sorterName;
        bool sorterPathInverted;
        LinkedListNode<System.Comparison<Requirement>> sorterPath;


        void RequirementField(Rect pos, Requirement req)
        {
            var selectionPos = new Rect(pos.x, pos.y + Data.statusPaddingY, pos.width, pos.height - Data.statusPaddingY * 2);
            GUI.color = selectedReq == req ? Data.selectedColor : Data.unselectedColor;
            if (GUI.Button(selectionPos, GUIContent.none, Data.selectionStyle))
            {
                selectedReq = req;
                UpdateSelectedTimestamp();
                Inspector.Repaint();
            }


            var statusPos = new Rect(pos.x, pos.y + Data.statusPaddingY, Data.statusWidth, pos.height - Data.statusPaddingY * 2);
            GUI.color = GetStatusColor(req);
            GUI.Box(statusPos, new GUIContent(GetStatusTex(req)), Data.statusStyle);

            var pathPos = new Rect(pos.x + Data.statusWidth + Data.reqNameWidth, pos.y + Data.statusPaddingY, pos.width - Data.responsiblePersonWidth - Data.reqNameWidth - Data.statusWidth, statusPos.height);
            GUI.Label(pathPos, new GUIContent(req.path), Data.pathBoxStyle);

            var namePos = new Rect(pos.x + Data.statusWidth, pos.y, Data.reqNameWidth, pos.height);
            GUI.color = GetPriorityColor(req);
            GUI.Label(namePos, new GUIContent(req.name), Data.nameBoxStyle);

            if (!LocalData.timestampDictionary.ContainsKey(req.path) || LocalData.timestampDictionary[req.path] < req.timestamp)
            {
                var notiPos = new Rect(namePos.x + namePos.width - Data.notificationPointPos.x, namePos.y + Data.notificationPointPos.y, Data.notificationPointPos.z, Data.notificationPointPos.z);
                GUI.color = Data.notificationPointColor;
                GUI.Box(notiPos, GUIContent.none, Data.notificationPointStyle);
            }

            var personPos = new Rect(pos.x + pos.width - Data.responsiblePersonWidth, pos.y, Data.responsiblePersonWidth, pos.height);
            GUI.color = Color.white;
            GUI.Label(personPos, string.IsNullOrWhiteSpace(req.responsiblePerson) ? new GUIContent(Data.responsiblePersonTex) : new GUIContent(req.responsiblePerson), Data.responsiblePersonStyle);

            GUI.color = Color.white;
        }

        void OnEnable()
        {
            ActiveManager = this;
            RefreshFilters();
            RefreshList();
            ClearInvalidTimestamp();

            sorterStatus = new LinkedListNode<System.Comparison<Requirement>>((l, r) =>
            {
                var res = l.status.CompareTo(r.status) + (fileExists[l] ? 0 : -1) + (fileExists[r] ? 0 : 1);
                return sorterStatusInverted ? -res : res;
            });
            sorters.AddFirst(sorterStatus);
            sorterPriority = new LinkedListNode<System.Comparison<Requirement>>((l, r) =>
            {
                var res = l.priority.CompareTo(r.priority);
                return sorterPriorityInverted ? -res : res;
            });
            sorters.AddFirst(sorterPriority);
            sorterName = new LinkedListNode<System.Comparison<Requirement>>((l, r) =>
            {
                var res = l.name.CompareTo(r.name);
                return sorterNameInverted ? -res : res;
            });
            sorters.AddFirst(sorterName);
            sorterPath = new LinkedListNode<System.Comparison<Requirement>>((l, r) =>
            {
                var res = l.path.CompareTo(r.path);
                return sorterPathInverted ? -res : res;
            });
            sorters.AddFirst(sorterPath);
        }

        void OnSelectionChange()
        {
            var path = AssetDatabase.GetAssetPath(Selection.activeObject);
            if (path.StartsWith("Assets/")) filterPath = path.Substring(6);
            if (!AssetDatabase.IsValidFolder(path)) filterPath = filterPath.Substring(0, filterPath.LastIndexOf("/"));
            if (ifFilterLocalPath) RefreshList();
            Repaint();
        }
        bool OnFilter(Requirement req)
        {
            if (ifFilterLocalPath && filterPath.StartsWith("/") && !req.path.StartsWith(filterPath)) return false;
            if (!string.IsNullOrWhiteSpace(filterKeyword) && !req.name.Contains(filterKeyword)) return false;
            if (filterPersonIndex > 0 && !string.IsNullOrWhiteSpace(req.responsiblePerson) && filterPersons[filterPersonIndex] != req.responsiblePerson) return false;
            if (filterStatus > 0 && !filterStatus.HasFlag(req.status)) return false;
            if (filterPriority > 0 && !filterPriority.HasFlag(req.priority)) return false;
            if (filterExtensionIndex > 0)
            {
                var match = extEx.Match(req.path);
                if (!match.Success || filterExtensions[filterExtensionIndex] != match.Groups[1].Value)
                {
                    return false;
                }
            }
            return true;
        }

        int OnCompare(Requirement l, Requirement r)
        {
            foreach (var s in sorters)
            {
                var res = s.Invoke(l, r);
                if (res == 0) continue;
                return res;
            }
            return 0;
        }

        void SorterField(string label, ref bool inverted, LinkedListNode<System.Comparison<Requirement>> sorter)
        {
            GUILayout.BeginHorizontal(Data.sorterButtonStyle);
            {
                if (GUILayout.Button(label, Data.sorterLabelStyle))
                {
                    inverted = !inverted;
                    sorters.Remove(sorter);
                    sorters.AddFirst(sorter);
                }
                GUI.color = sorters.First == sorter ? Data.sorterFirstColor : Data.sorterNormalColor;
                GUILayout.Label(GUIContent.none, inverted ? Data.sorterInvertedStyle : Data.sorterNormalStyle);
                GUI.color = Color.white;
            }
            GUILayout.EndHorizontal();
        }

        void OnGUI()
        {
            var windowRect = new Rect(Vector2.zero, position.size);

            var topRect = new Rect(0, 0, windowRect.size.x, topRectHeight);
            GUILayout.BeginArea(topRect);
            {
                GUILayout.Label("Requirements Manager", Data.nameStyle);
                GUILayout.BeginHorizontal();
                {
                    GUILayout.Label("Your name", Data.miniHeaderStyle);
                    LocalData.localName = GUILayout.TextField(LocalData.localName);
                }
                GUILayout.EndHorizontal();

                GUILayout.Space(6);
                GUILayout.Box(GUIContent.none, "ProfilerDetailViewBackground");
                GUILayout.Space(-12);

                GUILayout.BeginHorizontal();
                {
                    filterFoldout = EditorGUILayout.Foldout(filterFoldout, "Filters", true, Data.foldoutStyle);
                    EditorGUI.BeginChangeCheck();

                    if (ifFilterLocalPath)
                    {
                        GUI.color = Data.filterLocalColor;
                        GUILayout.Label(filterPath, Data.filterPathStyle);
                    }
                    else GUILayout.FlexibleSpace();

                    if (GUILayout.Button(GUIContent.none, ifFilterLocalPath ? Data.filterLocalStyle : Data.filterWorldStyle))
                    {
                        ifFilterLocalPath = !ifFilterLocalPath;
                        RefreshList();
                    }
                    GUI.color = Color.white;

                    if (GUILayout.Button("Reset", Data.miniButtonSytle))
                    {
                        ifFilterLocalPath = false;
                        filterKeyword = "";
                        filterPersonIndex = 0;
                        filterStatus = 0;
                        filterPriority = 0;
                        filterExtensionIndex = 0;
                    }
                    GUILayout.Space(3);
                }
                GUILayout.EndHorizontal();
                if (filterFoldout)
                {
                    GUILayout.BeginHorizontal();
                    {
                        GUILayout.Space(Data.indentWidth);
                        GUILayout.Label("Keyword", GUILayout.ExpandWidth(false));
                        filterKeyword = GUILayout.TextField(filterKeyword, GUILayout.ExpandWidth(true));
                        GUILayout.Label("Person", GUILayout.ExpandWidth(false));
                        filterPersonIndex = EditorGUILayout.Popup(filterPersonIndex, filterPersons.ToArray(), GUILayout.MaxWidth(Data.filterOptionWidth));
                    }
                    GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal();
                    {
                        GUILayout.Space(Data.indentWidth);
                        GUILayout.Label("Status", GUILayout.ExpandWidth(false));
                        filterStatus = (RequirementStatus)EditorGUILayout.EnumFlagsField((RequirementStatusFilter)filterStatus, GUILayout.MaxWidth(Data.filterOptionWidth));
                        GUILayout.FlexibleSpace();
                        GUILayout.Label("Priority", GUILayout.ExpandWidth(false));
                        filterPriority = (RequirementPriority)EditorGUILayout.EnumFlagsField((RequirementPriorityFilter)filterPriority, GUILayout.MaxWidth(Data.filterOptionWidth));
                        GUILayout.FlexibleSpace();
                        GUILayout.Label("Format", GUILayout.ExpandWidth(false));
                        filterExtensionIndex = EditorGUILayout.Popup(filterExtensionIndex, filterExtensions.ToArray(), GUILayout.MaxWidth(Data.filterOptionWidth));
                    }
                    GUILayout.EndHorizontal();

                    if (EditorGUI.EndChangeCheck()) RefreshList();
                }

                GUILayout.Space(4);
                GUILayout.Box(GUIContent.none, "ProfilerDetailViewBackground");
                GUILayout.Space(-12);
                sorterFoldout = EditorGUILayout.Foldout(sorterFoldout, "Sorters", true, Data.foldoutStyle);
                if (sorterFoldout)
                {
                    EditorGUI.BeginChangeCheck();
                    GUILayout.BeginHorizontal();
                    {
                        GUILayout.Space(Data.indentWidth);
                        SorterField("Status", ref sorterStatusInverted, sorterStatus);
                        SorterField("Priority", ref sorterPriorityInverted, sorterPriority);
                        SorterField("Name", ref sorterNameInverted, sorterName);
                        SorterField("Path", ref sorterPathInverted, sorterPath);
                    }
                    GUILayout.EndHorizontal();
                    if (EditorGUI.EndChangeCheck()) RefreshList();
                }
            }
            GUILayout.EndArea();


            var scrollRect = new Rect(0, topRectHeight, windowRect.size.x, windowRect.size.y - topRectHeight);
            var viewHeight = reqList.Count * (Data.reqRectMargin.y + Data.reqLabelHeight);
            var viewRect = new Rect(0, 0, scrollRect.width - (scrollRect.height > viewHeight ? 0 : 13), viewHeight);
            GUI.Box(new Rect(scrollRect.x, scrollRect.y, viewRect.width, scrollRect.height), GUIContent.none, "window");
            scrollPos = GUI.BeginScrollView(scrollRect, scrollPos, viewRect);
            {
                var reqRect = new Rect(Data.reqRectMargin, new Vector2(viewRect.width - Data.reqRectMargin.x * 2, Data.reqLabelHeight));
                foreach (var req in reqList)
                {
                    RequirementField(reqRect, req);
                    reqRect.y += Data.reqRectMargin.y + Data.reqLabelHeight;
                }
            }
            GUI.EndScrollView();
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
        public void RefreshFilters()
        {
            var filterPerson = filterPersons.Count > 0 ? filterPersons[filterPersonIndex] : allStr;
            filterPersons.Clear();
            filterPersons.Add(allStr);

            var filterExtension = filterExtensions.Count > 0 ? filterExtensions[filterExtensionIndex] : allStr;
            filterExtensions.Clear();
            filterExtensions.Add(allStr);

            foreach (var r in Data.requirementList)
            {
                if (!string.IsNullOrWhiteSpace(r.responsiblePerson) && !filterPersons.Contains(r.responsiblePerson))
                {
                    filterPersons.Add(r.responsiblePerson);
                }
                var match = extEx.Match(r.path);
                if (match.Success && !filterExtensions.Contains(match.Groups[1].Value))
                {
                    filterExtensions.Add(match.Groups[1].Value);
                }
            }

            filterPersonIndex = filterPersons.IndexOf(filterPerson);
            if (filterPersonIndex < 0)
            {
                filterPersonIndex = 0;
                RefreshList();
            }

            filterExtensionIndex = filterExtensions.IndexOf(filterExtension);
            if (filterExtensionIndex < 0)
            {
                filterExtensionIndex = 0;
                RefreshList();
            }
        }
        public void RefreshList()
        {
            fileExists.Clear();
            reqList.Clear();

            // apply the filters
            foreach (var r in Data.requirementList)
            {
                if (OnFilter(r))
                {
                    reqList.Add(r);
                }
            }

            // iterate filtered list
            foreach (var r in reqList)
            {
                // detect file
                fileExists.Add(r, System.IO.File.Exists("Assets" + r.path));
            }

            // sort
            reqList.Sort(OnCompare);

            // keep fields consistent
            if (!reqList.Contains(selectedReq)) selectedReq = null;
        }
        public void NewRequirement()
        {
            var req = new Requirement();
            Data.requirementList.Add(req);
            selectedReq = req;
            RefreshList();
            Repaint();
        }
        public void UpdateSelectedTimestamp(string oldPath = null)
        {
            if (!string.IsNullOrWhiteSpace(oldPath))
            {
                if (LocalData.timestampDictionary.ContainsKey(oldPath))
                {
                    LocalData.timestampDictionary.Remove(oldPath);
                    EditorUtility.SetDirty(LocalData);
                }
            }
            if (!string.IsNullOrWhiteSpace(selectedReq.path))
            {
                var timestamp = (System.DateTime.UtcNow - System.DateTime.MinValue).TotalSeconds;
                if (LocalData.timestampDictionary.ContainsKey(selectedReq.path))
                {
                    LocalData.timestampDictionary[selectedReq.path] = timestamp;
                }
                else
                {
                    LocalData.timestampDictionary.Add(selectedReq.path, timestamp);
                }
                EditorUtility.SetDirty(LocalData);
            }
        }
        public void ClearInvalidTimestamp()
        {
            var paths = new HashSet<string>();
            foreach (var r in Data.requirementList)
            {
                if (!string.IsNullOrWhiteSpace(r.path))
                {
                    paths.Add(r.path);
                }
            }
            for (int i = LocalData.timestampDictionary.Count - 1; i >= 0; --i)
            {
                if (!paths.Contains(LocalData.timestampDictionary.Keys[i]))
                {
                    LocalData.timestampDictionary.Remove(LocalData.timestampDictionary.Keys[i]);
                }
            }
            EditorUtility.SetDirty(LocalData);
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