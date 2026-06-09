using System.Collections.Generic;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;

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
        public bool IsActive { get; set; } = true;

		public SceneHandle Parent { get; set; }
		public List<SceneHandle> Children { get; } = new List<SceneHandle>();

		public SceneNode(AsyncOperationHandle<SceneInstance> handle)
        {
            NodeType = SceneNodeType.Addressable;
            AddressableHandle = handle;
        }

		public SceneNode(UnityEngine.SceneManagement.Scene scene)
        {
            NodeType = SceneNodeType.BuiltIn;
            BuildInScene = scene;
        }

    }
}