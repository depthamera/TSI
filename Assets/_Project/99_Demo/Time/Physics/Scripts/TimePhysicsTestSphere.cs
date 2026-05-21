using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace TSI.Demo.Time
{
#if UNITY_EDITOR
    [CustomEditor(typeof(TimePhysicsTestSphere))]
    public class TimePhysicsTestSphereEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var test = (TimePhysicsTestSphere)target;
        
            if (GUILayout.Button("Reset Sphere", GUILayout.Height(30)))
            {
                test.Reset();
            }
        }
    }
#endif

    public class TimePhysicsTestSphere : MonoBehaviour
    {
        private static readonly int Color1 = Shader.PropertyToID("_BaseColor");
        public Renderer sphereRenderer;

        public Rigidbody sphereRigidbody;
        public Vector3 startPosition;
        public float startSpeed = 10f;

        private MaterialPropertyBlock _mpb;
        private void Awake()
        {
            _mpb = new MaterialPropertyBlock();

            if (sphereRigidbody)
            {
                sphereRigidbody.linearVelocity = Vector3.right * startSpeed;
            }

            startPosition = transform.position;
        }

        private void OnCollisionEnter(Collision other)
        {
            ChangeColor(Color.yellow);
        }

        private void OnTriggerEnter(Collider other)
        {
            Debug.Log($"OnTriggerEnter: frame={UnityEngine.Time.frameCount}");
        
            ChangeColor(Color.red);
        }

        private void OnTriggerExit(Collider other)
        {

            ChangeColor(Color.white);
        }

        public void Reset()
        {
            sphereRigidbody.position = startPosition;
            if(sphereRigidbody) sphereRigidbody.linearVelocity = Vector3.right * startSpeed;
        }
    
        private void ChangeColor(Color color)
        {
            _mpb.SetColor(Color1, color);
            sphereRenderer.SetPropertyBlock(_mpb);
        }
    
    }
}
