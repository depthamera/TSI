# Core Game Loop — 데모 설계

## 1. 개요

TSI(Time, Scene, Input) 프레임워크의 실동작을 증명하기 위한 데모 게임 루프를 설계한다.
Time은 어느정도 완성도를 갖췄고, Scene(CSM)은 방향성을 잡았으며, Input은 게임 플레이와 직접적으로 연관되어 있다.
추상적인 설계 확장 대신, 구체적인 게임 루프를 먼저 정의하고 이에 맞춰 각 시스템을 구현한다.

---

## 2. 게임 컨셉

- **장르**: 2D 횡스크롤 액션
- **레퍼런스**: Devil May Cry, Ultrakill — 스타일리쉬하고 스피디한 전투
- **핵심 판타지**: 복잡한 콤보로 장비를 교체하고, 각 장비의 고유 기술을 구사하며, 시간을 조작하여 전투를 유리하게 이끌어간다.

---

## 3. 데모 루프 플로우

```
Bootstrap
  └─ MainMenu (메인 화면 + 레벨 선택)
    └─ Gameplay (게임 관리자 — 유저 정보, 스테이지 진행 관리)
        ├─ Stage_1 (스테이지 씬)
        ├─ Stage_2 (스테이지 씬)
        └─ ...
```

### 씬 전환 흐름

1. **Bootstrap → MainMenu**: 앱 시작 시 로드
2. **MainMenu → Gameplay**: 레벨 선택 후 전환 (MainMenu는 Unload)
3. **Gameplay → Stage_N**: Gameplay 씬은 KeepActive, 스테이지 씬을 자식으로 Additive 로드
4. **Stage_N → Stage_N+1**: 현재 스테이지 Unload, 다음 스테이지 로드 (Gameplay 유지)
5. **스테이지 클리어 → MainMenu**: Gameplay + 마지막 스테이지 Unload, MainMenu 로드

### CSM 활용 포인트

| 전환 | CSM API | 증명하는 기능 |
|---|---|---|
| MainMenu → Gameplay | `LoadScene` (Unload) | 기본 씬 전환 |
| Gameplay → Stage_N | `LoadScene` (KeepActive, ParentNode 지정) | Additive 로드, 트리 구조 |
| Stage_N → Stage_N+1 | `LoadScene` (Unload) | 자식 노드 전환 |
| 일시정지 UI | `LoadScene` (Suspend) + `ResumeScene` | Suspend/Resume 생명주기 |
| 전체 전환 시 | `LoadingScreen` 연동 | 로딩 화면 시스템 |

---

## 4. TSI 시스템별 역할

### Time — 시간 조작

플레이어는 게임의 **시간 배속을 능동적으로 조절**한다.

- **슬로우 모션**: 긴 커맨드 입력을 위한 시간 확보. 리듬게임 급의 빠른 입력이 아니라면 threshold 안에 치기 어려운 커맨드가 존재하며, 슬로우 모션으로 입력 창을 벌릴 수 있다.
- **시간 정지**: 적들만 정지시켜 복잡한 탄막 사이에서 콤보를 완성하거나, 위치를 잡는 전략적 판단 시간을 확보한다.
- **핵심 시너지**: Time은 단순 연출이 아니라 **Input 시스템과 직결된 메카닉**이다. 시간을 늦추면 콤보 threshold가 사실상 완화되어 더 긴 커맨드를 성공시킬 수 있다.

### Scene — 씬 관리

CSM의 트리 구조와 생명주기를 데모 안에서 실증한다.

- **Gameplay + Stage 분리**: Gameplay 씬이 관리자 역할(유저 정보, 스테이지 진행 상태)을 하고, 실제 플레이 씬들은 자식 노드로 로드/언로드한다.
- **Suspend/Resume**: 일시정지 화면이나 인벤토리 UI 진입 시 Gameplay를 Suspend하고, 복귀 시 Resume한다.
- **로딩 화면**: 스테이지 간 전환 시 로딩 씬을 연동하여 ILoadingScreen 인터페이스를 실증한다.

### Input — 콤보 시스템

격투 게임 / DMC 스타일의 복잡한 커맨드 입력을 처리한다.

- **콤보별 Threshold**: 각 콤보마다 입력 허용 시간이 다르게 설정된다. 단순한 콤보는 넉넉한 threshold, 강력한 기술은 빡빡한 threshold.
- **입력 버퍼 + 패턴 매칭**: 매니저가 입력 버퍼를 관리하고, 등록된 패턴과 대조하여 콤보를 판정한다.
- **장비 교체**: 콤보 입력을 통해 장비를 전환하고, 장비별로 다른 기술 세트를 사용한다.

---

## 5. 보류 사항

- **과거 씬 오버레이**: 현재 상황을 몇 초 딜레이로 따라오는 "과거" 씬을 Additive로 띄워 오버레이 표시하고, 그 시점으로 되돌아가는 메카닉. Time + Scene 시스템의 고급 활용이지만, 데모 1차 스코프에서는 제외한다. 핵심 루프 완성 후 검토.