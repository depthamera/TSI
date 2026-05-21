using TSI.App.Physics;
using UnityEditor;
using UnityEngine;

namespace TSI.Editor
{
    [CustomEditor(typeof(Rigidbody))]
    public class RigidbodyEditorExtension : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            Rigidbody rb = (Rigidbody)target;
            var go = rb.gameObject;

            if (rb.interpolation != RigidbodyInterpolation.None)
            {
                if (!go.GetComponent<PhysicsBodyComponent>())
                    go.AddComponent<PhysicsBodyComponent>();
            }
            else
            {
                var script = go.GetComponent<PhysicsBodyComponent>();
                if (script)
                    DestroyImmediate(script);
            }
        }
    }
}