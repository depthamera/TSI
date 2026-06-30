using TSI.Core;
using UnityEngine;
using VContainer;

namespace TSI.App.Physics
{
    [DisallowMultipleComponent]
    [AddComponentMenu("Physics/Custom Interpolation")]
    public class PhysicsBodyComponent : MonoBehaviour
    {
        [Inject]
        public void Initialize(PhysicsInterpolationManager interpolationManager)
        {
            if (TryGetComponent<Rigidbody>(out var rb3d))
            {
                interpolationManager.Register(transform, () => rb3d.position);
            }
            else if (TryGetComponent<Rigidbody2D>(out var rb2d))
            {
                interpolationManager.Register(transform, () => (Vector3)rb2d.position);
            }
        }
    }
}