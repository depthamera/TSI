using Cysharp.Threading.Tasks;

namespace TSI.Core.Scene
{
    interface ICustomSceneManager
    {
        UniTask<SceneHandle> LoadScene(SceneRef target, SceneHandle parent, SceneTransitionOptions options = default);
        UniTask<SceneHandle> ReplaceScene(SceneRef target, SceneHandle toReplace, SceneTransitionOptions options = default);
        UniTask UnloadScene(SceneHandle target);
        UniTask SuspendScene(SceneHandle target);
        UniTask ResumeScene(SceneHandle target);
    }
}
