using System.Collections.Generic;
using TSI.Game.FSM;
using UnityEditor;
using UnityEngine;

namespace TSI.Editor
{
    /// <summary>
    /// FSMTransitionTable SO의 커스텀 인스펙터.
    /// - From State 기준 그룹핑 뷰 (priority 내림차순 정렬)
    /// - 인라인 추가/삭제
    /// - 검증 경고 (null 참조, 중복 전이)
    /// - Raw List 폴백 토글
    /// </summary>
    [CustomEditor(typeof(FSMTransitionTable))]
    public class FSMTransitionTableEditor : UnityEditor.Editor
    {
        private SerializedProperty _initialStateProp;
        private SerializedProperty _transitionsProp;

        private readonly Dictionary<int, bool> _foldoutStates = new();
        private bool _showRawList;

        // ── Helper types ──────────────────────────────────────────

        private class TransitionEntry
        {
            public int ArrayIndex;
            public SerializedProperty Property;
            public Object FromState;
            public Object ToState;
            public Object Condition;
            public int Priority;
        }

        private class TransitionGroup
        {
            public Object FromState;
            public string DisplayName;
            public readonly List<TransitionEntry> Entries = new();
        }

        // ── Lifecycle ─────────────────────────────────────────────

        private void OnEnable()
        {
            _initialStateProp = serializedObject.FindProperty("initialState");
            _transitionsProp = serializedObject.FindProperty("transitions");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            // === Initial State ===
            EditorGUILayout.PropertyField(_initialStateProp);
            EditorGUILayout.Space(8);

            // === View Toggle Header ===
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Transitions", EditorStyles.boldLabel);

            GUILayout.FlexibleSpace();

            int selectedIndex = _showRawList ? 1 : 0;

            // 나란히 붙어있는 버튼 그룹 생성
            selectedIndex = GUILayout.Toolbar(selectedIndex, new string[] { "Grouped", "Raw" }, EditorStyles.miniButton, GUILayout.Width(130));

            // 선택된 인덱스(1)일 때 Raw List 활성화
            _showRawList = (selectedIndex == 1);

            //if (_showRawList)
            //{
            //    if (GUILayout.Button("Grouped View", EditorStyles.miniButton, GUILayout.Width(90)))
            //        _showRawList = false;
            //}
            //else
            //{
            //    if (GUILayout.Button("Raw List", EditorStyles.miniButton, GUILayout.Width(70)))
            //        _showRawList = true;
            //}

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space(4);

            // === Content ===
            if (_showRawList)
            {
                DrawRawList();
            }
            else
            {
                DrawGroupedView();
            }

            serializedObject.ApplyModifiedProperties();
        }

        // ── Raw List (Fallback) ───────────────────────────────────

        private void DrawRawList()
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(_transitionsProp, new GUIContent("Transition List"), true);
            EditorGUI.indentLevel--;
        }

        // ── Grouped View ──────────────────────────────────────────

        private void DrawGroupedView()
        {
            // Validation
            DrawValidation();

            // Build groups
            var groups = BuildGroups();

            // Summary
            var summaryStyle = new GUIStyle(EditorStyles.miniLabel)
            {
                alignment = TextAnchor.MiddleRight
            };
            EditorGUILayout.LabelField(
                $"{_transitionsProp.arraySize} transitions · {groups.Count} groups",
                summaryStyle);
            EditorGUILayout.Space(2);

            // Draw each group
            int deleteIndex = -1;

            foreach (var group in groups)
            {
                deleteIndex = DrawGroup(group, deleteIndex);
            }

            // Bottom: add to new group
            EditorGUILayout.Space(8);
            if (GUILayout.Button("+ Add Transition", GUILayout.Height(26)))
            {
                AddTransition(null);
            }

            // Process pending deletion (한 프레임에 하나만)
            if (deleteIndex >= 0)
            {
                _transitionsProp.DeleteArrayElementAtIndex(deleteIndex);
            }
        }

        // ── Group Building ────────────────────────────────────────

        private List<TransitionGroup> BuildGroups()
        {
            var groupDict = new Dictionary<int, TransitionGroup>();

            for (int i = 0; i < _transitionsProp.arraySize; i++)
            {
                var element = _transitionsProp.GetArrayElementAtIndex(i);
                var fromObj = element.FindPropertyRelative("from").objectReferenceValue;
                var toObj = element.FindPropertyRelative("to").objectReferenceValue;
                var condObj = element.FindPropertyRelative("condition").objectReferenceValue;
                var prio = element.FindPropertyRelative("priority").intValue;

                int key = fromObj != null ? fromObj.GetInstanceID() : 0;

                if (!groupDict.TryGetValue(key, out var group))
                {
                    group = new TransitionGroup
                    {
                        FromState = fromObj,
                        DisplayName = fromObj != null ? fromObj.name : "(No From State)"
                    };
                    groupDict[key] = group;
                }

                group.Entries.Add(new TransitionEntry
                {
                    ArrayIndex = i,
                    Property = element,
                    FromState = fromObj,
                    ToState = toObj,
                    Condition = condObj,
                    Priority = prio
                });
            }

            // Priority 내림차순 정렬 (그룹 내)
            var result = new List<TransitionGroup>();
            foreach (var kvp in groupDict)
            {
                kvp.Value.Entries.Sort((a, b) => b.Priority.CompareTo(a.Priority));
                result.Add(kvp.Value);
            }

            // 그룹 정렬: 이름순, null은 마지막
            result.Sort((a, b) =>
            {
                if (a.FromState == null && b.FromState != null) return 1;
                if (a.FromState != null && b.FromState == null) return -1;
                return string.Compare(a.DisplayName, b.DisplayName, System.StringComparison.Ordinal);
            });

            return result;
        }

        // ── Group Drawing ─────────────────────────────────────────

        private int DrawGroup(TransitionGroup group, int currentDeleteIndex)
        {
            int foldoutKey = group.FromState != null ? group.FromState.GetInstanceID() : 0;
            if (!_foldoutStates.ContainsKey(foldoutKey))
                _foldoutStates[foldoutKey] = true;

            // ── Header ──
            var headerRect = EditorGUILayout.GetControlRect(false, 24);

            // Header background
            var bgColor = EditorGUIUtility.isProSkin
                ? new Color(0.24f, 0.24f, 0.27f)
                : new Color(0.76f, 0.76f, 0.80f);
            EditorGUI.DrawRect(headerRect, bgColor);

            // Left border accent
            var accentRect = new Rect(headerRect.x, headerRect.y, 3, headerRect.height);
            var accentColor = group.FromState != null
                ? new Color(0.35f, 0.65f, 1f)   // 할당됨: 파란색
                : new Color(1f, 0.55f, 0.25f);   // 미할당: 주황색
            EditorGUI.DrawRect(accentRect, accentColor);

            // Foldout
            var foldoutRect = new Rect(headerRect.x + 6, headerRect.y, headerRect.width - 34, headerRect.height);
            _foldoutStates[foldoutKey] = EditorGUI.Foldout(
                foldoutRect,
                _foldoutStates[foldoutKey],
                $"  {group.DisplayName}  ({group.Entries.Count})",
                true,
                EditorStyles.boldLabel);

            // "+" 버튼 (헤더 우측)
            var addBtnRect = new Rect(headerRect.xMax - 26, headerRect.y + 3, 22, 18);
            if (GUI.Button(addBtnRect, "+", EditorStyles.miniButton))
            {
                AddTransition(group.FromState as FSMState);
            }

            if (!_foldoutStates[foldoutKey])
                return currentDeleteIndex;

            // ── Transition Rows ──
            EditorGUI.indentLevel++;

            for (int i = 0; i < group.Entries.Count; i++)
            {
                var entry = group.Entries[i];
                currentDeleteIndex = DrawTransitionRow(entry, currentDeleteIndex);
            }

            EditorGUI.indentLevel--;
            EditorGUILayout.Space(2);

            return currentDeleteIndex;
        }

        // ── Transition Row ────────────────────────────────────────

        private int DrawTransitionRow(TransitionEntry entry, int currentDeleteIndex)
        {
            var toProp = entry.Property.FindPropertyRelative("to");
            var condProp = entry.Property.FindPropertyRelative("condition");
            var prioProp = entry.Property.FindPropertyRelative("priority");

            EditorGUILayout.BeginHorizontal();

            // Arrow
            EditorGUILayout.LabelField("→", GUILayout.Width(32));

            // To State
            EditorGUILayout.PropertyField(toProp, GUIContent.none, GUILayout.MinWidth(80));

            // "when" label
            var whenStyle = new GUIStyle(EditorStyles.miniLabel)
            {
                alignment = TextAnchor.MiddleCenter,
                normal = { textColor = EditorGUIUtility.isProSkin
                    ? new Color(0.6f, 0.75f, 0.9f)
                    : new Color(0.2f, 0.4f, 0.6f) }
            };
            EditorGUILayout.LabelField("when", whenStyle, GUILayout.Width(48));

            // Condition
            EditorGUILayout.PropertyField(condProp, GUIContent.none, GUILayout.MinWidth(80));

            // Priority
            var prioLabel = new GUIContent("P", "Priority (높을수록 먼저 평가)");
            EditorGUILayout.LabelField(prioLabel, GUILayout.Width(24));
            prioProp.intValue = EditorGUILayout.IntField(prioProp.intValue, GUILayout.Width(28));

            // Delete button
            var prevBg = GUI.backgroundColor;
            GUI.backgroundColor = new Color(1f, 0.45f, 0.4f);
            if (GUILayout.Button("×", GUILayout.Width(20), GUILayout.Height(18)))
            {
                currentDeleteIndex = entry.ArrayIndex;
            }
            GUI.backgroundColor = prevBg;

            EditorGUILayout.EndHorizontal();

            return currentDeleteIndex;
        }

        // ── Validation ────────────────────────────────────────────

        private void DrawValidation()
        {
            var warnings = new List<string>();
            var duplicateSet = new HashSet<string>();

            // Initial State 체크
            if (_initialStateProp.objectReferenceValue == null)
                warnings.Add("Initial State가 설정되지 않았습니다.");

            for (int i = 0; i < _transitionsProp.arraySize; i++)
            {
                var element = _transitionsProp.GetArrayElementAtIndex(i);
                var from = element.FindPropertyRelative("from").objectReferenceValue;
                var to = element.FindPropertyRelative("to").objectReferenceValue;
                var cond = element.FindPropertyRelative("condition").objectReferenceValue;

                // Null 체크
                if (from == null)
                    warnings.Add($"[{i}] From State가 비어 있습니다.");
                if (to == null)
                    warnings.Add($"[{i}] To State가 비어 있습니다.");
                if (cond == null)
                    warnings.Add($"[{i}] Condition이 비어 있습니다.");

                // 중복 체크
                if (from != null && to != null && cond != null)
                {
                    var key = $"{from.GetInstanceID()}_{to.GetInstanceID()}_{cond.GetInstanceID()}";
                    if (!duplicateSet.Add(key))
                        warnings.Add($"중복 전이: {from.name} → {to.name} ({cond.name})");
                }
            }

            if (warnings.Count > 0)
            {
                EditorGUILayout.HelpBox(string.Join("\n", warnings), MessageType.Warning);
                EditorGUILayout.Space(4);
            }
        }

        // ── Add / Delete ──────────────────────────────────────────

        private void AddTransition(FSMState fromState)
        {
            int newIndex = _transitionsProp.arraySize;
            _transitionsProp.InsertArrayElementAtIndex(newIndex);
            var newElement = _transitionsProp.GetArrayElementAtIndex(newIndex);

            newElement.FindPropertyRelative("from").objectReferenceValue = fromState;
            newElement.FindPropertyRelative("to").objectReferenceValue = null;
            newElement.FindPropertyRelative("condition").objectReferenceValue = null;
            newElement.FindPropertyRelative("priority").intValue = 0;

            // 해당 그룹 자동 펼침
            int foldoutKey = fromState != null ? fromState.GetInstanceID() : 0;
            _foldoutStates[foldoutKey] = true;
        }
    }
}
