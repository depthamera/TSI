using System.Collections.Generic;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using VContainer.Unity;

namespace TSI.Core.Scene
{
	public enum SceneNodeType
	{
		BuiltIn,
        Addressable,
    }

	public class SceneNode
	{
        public SceneNodeType NodeType { get; private set; }

        public AsyncOperationHandle<SceneInstance> AddressableHandle { get; }
        public UnityEngine.SceneManagement.Scene BuildInScene { get; }
        public LifetimeScope LifetimeScope { get; }
        public bool IsActive { get; set; } = true;

		public SceneHandle Parent { get; set; }
		public List<SceneHandle> Children { get; } = new List<SceneHandle>();

		public SceneNode(AsyncOperationHandle<SceneInstance> handle, LifetimeScope lifetimeScope = null)
        {
            NodeType = SceneNodeType.Addressable;
            AddressableHandle = handle;
            LifetimeScope = lifetimeScope;
        }

		public SceneNode(UnityEngine.SceneManagement.Scene scene, LifetimeScope lifetimeScope = null)
        {
            NodeType = SceneNodeType.BuiltIn;
            BuildInScene = scene;
            LifetimeScope = lifetimeScope;
        }

    }
}