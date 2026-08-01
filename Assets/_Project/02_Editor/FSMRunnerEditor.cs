using TSI.Game.FSM;
using UnityEditor;
using UnityEngine;

namespace TSI.Editor
{
    [CustomEditor(typeof(FSMRunner))]
    public class FSMRunnerEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            if (!Application.isPlaying) return;

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Runtime", EditorStyles.boldLabel);

            var runner = (FSMRunner)target;

            EditorGUI.BeginDisabledGroup(true);

            // 활성 계층 체인 표시
            var chain = runner.ActiveStateChain;
            if (chain != null && chain.Count > 0)
            {
                var chainNames = new string[chain.Count];
                for (int i = 0; i < chain.Count; i++)
                    chainNames[i] = chain[i] != null ? chain[i].name : "(null)";
                EditorGUILayout.TextField("State Chain", string.Join(" > ", chainNames));
            }
            else
            {
                EditorGUILayout.TextField("State Chain", "(none)");
            }

            if (runner.Blackboard != null)
            {
                EditorGUILayout.IntField("Facing Direction", runner.Blackboard.FacingDirection);
            }
            EditorGUI.EndDisabledGroup();

            // 플레이 중 인스펙터 자동 갱신
            Repaint();
        }
    }
}
