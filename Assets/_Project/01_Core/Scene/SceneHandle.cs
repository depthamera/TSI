using System;

namespace TSI.Core.Scene
{
    public readonly struct SceneHandle : IEquatable<SceneHandle>
    {
        public SceneHandle(string scenePath, int instanceID)
        {
            ScenePath = scenePath;
            InstanceID = instanceID;
        }

        public string ScenePath { get; }
        public int InstanceID { get; }

        public bool Equals(SceneHandle other) => other.ScenePath == ScenePath && other.InstanceID == InstanceID;

        public override bool Equals(object other) => other is SceneHandle handle && Equals(handle);

        public override int GetHashCode() => HashCode.Combine(ScenePath, InstanceID);

        public static bool operator ==(SceneHandle left, SceneHandle right) => left.Equals(right);

        public static bool operator !=(SceneHandle left, SceneHandle right) => !left.Equals(right);

    }
}