using Cysharp.Threading.Tasks;

namespace TSI.Core.Scene
{
    public interface ICustomSceneManager
    {
        UniTask<SceneHandle> LoadScene(SceneProfile target, SceneHandle parent, SceneTransitionOptions options = default);
        UniTask<SceneHandle> ReplaceScene(SceneProfile target, SceneHandle toReplace, SceneTransitionOptions options = default);
        UniTask UnloadScene(SceneHandle target);
        UniTask SuspendScene(SceneHandle target);
        UniTask ResumeScene(SceneHandle target);

        SceneHandle Resolve(UnityEngine.SceneManagement.Scene scene);
        SceneHandle RegisterRootScene(UnityEngine.SceneManagement.Scene bootstrapScene);
    }
}
