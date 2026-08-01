namespace TSI.Core.Scene
{
	public readonly struct SceneTransitionOptions
    {
		public SceneProfile LoadingScene { get; }
		public float MinimumLoadingTime { get; }

        public SceneTransitionOptions(SceneProfile loadingScene = null, float minimumLoadingTime = .0f)
		{
			LoadingScene = loadingScene;
			MinimumLoadingTime = minimumLoadingTime;
		}
    }
}