using System;
using Cysharp.Threading.Tasks;
using VContainer;

namespace TSI.Core.Scene
{
    public interface ICustomSceneManager
    {
        UniTask<SceneHandle> LoadScene(SceneProfile target, SceneHandle parent, SceneTransitionOptions options = default, Action<IContainerBuilder> extraRegistrations = null);
        UniTask<SceneHandle> ReplaceScene(SceneProfile target, SceneHandle toReplace, SceneTransitionOptions options = default, Action<IContainerBuilder> extraRegistrations = null);
        UniTask UnloadScene(SceneHandle target);
        UniTask SuspendScene(SceneHandle target);
        UniTask ResumeScene(SceneHandle target);

        SceneHandle Resolve(UnityEngine.SceneManagement.Scene scene);
        SceneHandle RegisterRootScene(UnityEngine.SceneManagement.Scene bootstrapScene);
        SceneHandle GetParent(SceneHandle handle);
    }
}
