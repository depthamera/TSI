# Custom Scene Management(CSM) 설계

## 1. 목표

데모 게임 루프([CoreGameLoop.md](CoreGameLoop.md))의 씬 전환을 관리하는 시스템.
Bootstrap 씬을 루트로 두는 트리 구조로, 각 씬을 노드로 관리한다.
CSM은 로드된 노드를 Dictionary로 관리하고, 각 노드는 자신의 부모와 자식을 Key로 flat하게 저장한다.

---

## 2. 데모 루프 기반 요구사항

CoreGameLoop.md의 씬 전환을 기반으로 CSM의 요구사항을 도출한다.

```
Bootstrap
  └─ MainMenu
    └─ Gameplay
        ├─ Stage_1
        ├─ Stage_2
        └─ ...
```

| # | 전환 | 동작 | 필요한 기능 |
|---|---|---|---|
| 1 | Bootstrap → MainMenu | MainMenu를 Bootstrap의 자식으로 로드 | 씬 로드 (부모 지정) |
| 2 | MainMenu → Gameplay | MainMenu 언로드 + Gameplay를 Bootstrap의 자식으로 로드 | 씬 교체 (원자적 언로드+로드) |
| 3 | Gameplay → Stage_N | Stage를 Gameplay의 자식으로 Additive 로드 | 씬 추가 (부모 유지) |
| 4 | Stage_N → Stage_N+1 | 현재 Stage 언로드 + 다음 Stage 로드 | 씬 교체 (부모 유지) |
| 5 | 클리어 → MainMenu | Gameplay(+자식 Stage) 언로드 + MainMenu 로드 | 씬 교체 (연쇄 언로드) |
| 6 | 일시정지 | Stage를 Suspend + PauseUI 로드 | Suspend + 씬 로드 |
| 7 | 일시정지 해제 | PauseUI 언로드 + Stage Resume | 씬 언로드 + Resume |

### 도출된 핵심 연산

| 연산 | 설명 | 사용처 |
|---|---|---|
| **LoadScene** | 대상 씬을 특정 부모의 자식으로 Additive 로드 | #1, #3, #6 |
| **ReplaceScene** | 기존 씬(과 자식)을 언로드하고 같은 부모 아래에 새 씬 로드 | #2, #4, #5 |
| **UnloadScene** | 대상 씬과 모든 자식을 언로드 | #7 |
| **SuspendScene** | 대상 씬의 OnSuspend 호출, 비활성화 (메모리 유지) | #6 |
| **ResumeScene** | Suspend된 씬의 OnResume 호출, 활성화 | #7 |

---

## 3. 씬 식별

### SceneProfile — 로드 대상 지정 (ScriptableObject)

아직 로드되지 않은 씬을 지정할 때 사용한다. 씬 에셋 경로를 보관하는 ScriptableObject.
Inspector에서 경로를 설정하며, 로딩 씬도 동일한 타입을 사용한다.

```csharp
[CreateAssetMenu(menuName = "TSI/Scene/Scene Profile")]
public class SceneProfile : ScriptableObject
{
    [SerializeField] private string scenePath;
    public string ScenePath => scenePath;
}
```

### SceneHandle — 로드된 씬 식별

CSM이 씬을 로드할 때 생성하여 반환하는 핸들. 이후 해당 씬을 참조할 때 사용한다.

```csharp
public readonly struct SceneHandle : IEquatable<SceneHandle>
{
    public string ScenePath { get; }
    public int InstanceId { get; }  // CSM 내부 auto-increment
}
```

### 분리 이유

- **SceneProfile**: "무엇을 로드할지" — Inspector에서 설정, 호출자가 참조
- **SceneHandle**: "로드된 것을 어떻게 참조할지" — CSM이 생성하여 반환

---

## 4. API 설계

### 설계 원칙

- **명시적 지정**: 호출자가 부모, 교체 대상 등을 직접 지정한다. "현재 씬" 같은 암시적 전역 상태에 의존하지 않는다.
- **단일 책임**: 각 API는 하나의 연산만 수행한다 (Load, Replace, Unload, Suspend, Resume).
- **잘못된 조합의 구조적 차단**: API 형태로 불가능하게 만든다. 예를 들어, LoadScene과 ReplaceScene을 분리하여 parent-child 불일치를 구조적으로 방지한다.

### 4-1. LoadScene

대상 씬을 지정한 부모의 자식으로 Additive 로드한다. 기존 자식은 유지된다.

```csharp
UniTask<SceneHandle> LoadScene(SceneProfile target, SceneHandle parent, SceneTransitionOptions options = default);
```

```csharp
public readonly struct SceneTransitionOptions
{
    public SceneProfile LoadingScene { get; }     // null이면 로딩 화면 없이 전환
}
```

### 4-2. ReplaceScene

기존 씬(과 모든 자식)을 언로드하고, 같은 부모 아래에 새 씬을 로드한다.
부모는 교체 대상의 부모에서 자동 추론되므로, parent-child 불일치가 구조적으로 불가능하다.

```csharp
UniTask<SceneHandle> ReplaceScene(SceneProfile target, SceneHandle toReplace, SceneTransitionOptions options = default);
```

- `SceneTransitionOptions.LoadingScene`에 로딩 씬 프로필을 지정하면 로딩 화면을 표시한다.

- 내부적으로 로딩 화면 표시 → 기존 씬 언로드 → 새 씬 로드 → 로딩 화면 해제 순서로 수행한다.
- `toReplace`에 자식 노드가 있으면 연쇄 언로드된다.

### 4-3. UnloadScene

대상 씬과 모든 자식 노드를 연쇄 언로드한다.

```csharp
UniTask UnloadScene(SceneHandle target);
```

- 트리에서 해당 노드와 모든 하위 노드를 제거한다.
- Suspend 상태인 씬도 언로드 가능하다.

### 4-4. SuspendScene

대상 씬의 `ISceneLifecycleHandler.OnSuspend`를 호출하고 비활성화한다.

```csharp
UniTask SuspendScene(SceneHandle target);
```

### 4-5. ResumeScene

Suspend된 씬의 `ISceneLifecycleHandler.OnResume`을 호출하고 활성화한다.

```csharp
UniTask ResumeScene(SceneHandle target);
```

### 데모 루프 적용

```csharp
// 1. Bootstrap → MainMenu (Additive 로드)
var mainMenu = await csm.LoadScene(Scenes.MainMenu, bootstrapHandle);

// 2. MainMenu → Gameplay (MainMenu를 교체 — 부모 bootstrap은 자동 추론)
var gameplay = await csm.ReplaceScene(Scenes.Gameplay, mainMenu);

// 3. Gameplay → Stage_1 (Additive 로드)
var stage = await csm.LoadScene(Scenes.Stage1, gameplay);

// 4. Stage_1 → Stage_2 (Stage_1을 교체 — 부모 gameplay는 자동 추론)
stage = await csm.ReplaceScene(Scenes.Stage2, stage);

// 5. 클리어 → MainMenu (Gameplay + 자식 Stage 연쇄 교체)
mainMenu = await csm.ReplaceScene(Scenes.MainMenu, gameplay,
    new SceneTransitionOptions(loadingScene: Scenes.DefaultLoading));

// 6. 일시정지
await csm.SuspendScene(stage);
var pauseUI = await csm.LoadScene(Scenes.PauseUI, gameplay);

// 7. 일시정지 해제
await csm.UnloadScene(pauseUI);
await csm.ResumeScene(stage);
```

---

## 5. Suspend / Resume 생명주기

### ISceneLifecycleHandler

씬의 Suspend/Resume을 처리하는 인터페이스. **씬당 하나의 핸들러**를 권장한다.

```csharp
public interface ISceneLifecycleHandler
{
    UniTask OnSuspend(CancellationToken ct);
    UniTask OnResume(CancellationToken ct);
}
```

- `CancellationToken`: Suspend 중 씬이 강제 Unload될 경우(예: 앱 종료) 정리 작업을 중단할 수 있어야 한다.
- `UniTask` 반환: 페이드아웃, 오브젝트 풀 정리 등 비동기 작업이 필요할 수 있다.

### 등록 방식 (VContainer)

각 씬의 LifetimeScope에서 `ISceneLifecycleHandler` 구현체를 컨테이너에 등록한다.

```csharp
public class GameplayLifetimeScope : LifetimeScope
{
    protected override void Configure(IContainerBuilder builder)
    {
        builder.Register<GameplaySceneController>(Lifetime.Scoped)
               .As<ISceneLifecycleHandler>();
    }
}
```

### 설계 결정

- **씬당 단일 핸들러 권장**: 복수 핸들러(`IEnumerable<ISceneLifecycleHandler>`)는 호출 순서 보장이 어렵고, 하나가 실패했을 때 나머지 처리가 모호하다. 단일 조율자(Coordinator)가 내부에서 순서를 제어하는 것이 명확하다.
- **Optional 등록**: 핸들러가 없는 씬도 허용한다 (경고 로그 출력). 단순 UI 씬은 Suspend 핸들링이 불필요할 수 있다.

### CSM 내부 흐름

CSM은 씬의 `LifetimeScope`를 노드에 캐싱하지 않고, 필요 시 `Scene`에서 직접 탐색한다.
Suspend/Resume은 씬 전환 시에만 발생하므로 캐싱의 이점이 없어 단순한 방식을 취한다.

```csharp
LifetimeScope FindLifetimeScope(Scene scene)
{
    foreach (var go in scene.GetRootGameObjects())
    {
        var scope = go.GetComponentInChildren<LifetimeScope>();
        if (scope != null) return scope;
    }
    return null;
}

// Suspend 수행 시
var scope = FindLifetimeScope(targetScene);
if (scope != null && scope.Container.TryResolve<ISceneLifecycleHandler>(out var handler))
    await handler.OnSuspend(ct);
else
    Debug.LogWarning($"No ISceneLifecycleHandler for scene: {handle}");
```

---

## 6. 로딩 화면

모든 로딩 화면은 독립된 씬으로 존재하며, CSM의 노드 트리에 포함되지 않는다.
Unload 연쇄에 말려들지 않고, CSM이 전환 과정에서 내부적으로 Additive 로드/언로드를 관리한다.

### ILoadingScreen

```csharp
public interface ILoadingScreen
{
    UniTask Show(CancellationToken ct);
    UniTask Hide(CancellationToken ct);
    void ReportProgress(float progress);    // 0~1 진행도
}
```

### 등록 방식

로딩 씬도 `SceneProfile` SO로 관리한다. 전환 시 `SceneTransitionOptions.LoadingScene`에 로딩 씬 프로필을 전달하면 된다.
종류가 늘어나면 전환마다 다른 `SceneProfile`을 지정할 수 있다.

```csharp
// 호출 시 로딩 씬 지정
var options = new SceneTransitionOptions(loadingScene: loadingSceneProfile);
await csm.ReplaceScene(Scenes.MainMenu, gameplay, options);
```

### 내부 플로우

```
1. 로딩 씬을 Additive로 로드 (트리 외부)
2. ILoadingScreen Resolve
3. await Show()
4. 실제 씬 전환 수행 (Replace 대상 언로드 → 새 씬 로드)
   └─ 중간중간 ReportProgress() 호출
5. await Hide()
6. 로딩 씬 Unload
```
