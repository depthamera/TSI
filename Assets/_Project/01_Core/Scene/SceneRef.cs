namespace TSI.Core.Scene
{
    public readonly struct SceneRef
    {
        public string ScenePath { get; }

        public SceneRef(string scenePath)
        {
            ScenePath = scenePath;
        }
    }
}