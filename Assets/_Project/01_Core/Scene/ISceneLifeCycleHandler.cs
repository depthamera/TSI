using Cysharp.Threading.Tasks;
using System.Threading;

namespace TSI.Core.Scene
{
    interface ISceneLifeCycleHandler
    {
        UniTask OnSuspend(CancellationToken ct);
        UniTask OnResume(CancellationToken ct);
    }
}
