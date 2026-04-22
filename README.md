# Potan CoreUtils

Unity 프로젝트 초기 설정 시 필수적으로 필요한 고성능 유틸리티 모음입니다. 
성능 최적화(Zero-GC)와 개발 편의성에 초점을 맞추어 설계되었으며, `Potan.CoreUtils` 네임스페이스 아래 모든 기능이 통합되어 있습니다.

---

## 주요 기능 (Key Features)

### 1. EventBus
- Struct 기반 이벤트 시스템으로 간단한 이벤트 정의 및 발행이 가능합니다.

### 2. DevLog
- `[Conditional]` 어트리뷰트를 사용하여 Release 빌드 시 로그 코드가 자동으로 제거됩니다.
- `UnityEngine.Color` 지원으로 가시성 높은 로그를 남길 수 있으며, 기존 유니티 Log의 `Object context` 기능을 강화 해 로그 앞부분에 Object의 이름을 자동으로 붙여줍니다.

### 3. Singletons
- **MonoSingleton**: `DontDestroyOnLoad`가 적용된 전역 싱글톤으로 Instance 접근 시 자동으로 생성되며 자동 생성 기능의 OnOff 설정이 가능합니다.
- **MonoSceneSingleton**: 해당 씬에서만 유지되는 싱글톤. (자동 생성 미지원)
- **Singleton<T>**: 일반 C# 클래스용 Thread-safe 싱글톤.

---

## 설치 방법 (Installation)

1. Unity Editor에서 `Window > Package Manager`를 엽니다.
2. `+` 버튼 클릭 후 `Add package from git URL...`을 선택합니다.
3. 아래 URL을 입력합니다:
   ```
   https://github.com/Potan7/com.potan.coreutils.git
   ```

## 요구 사항
- Unity 2022.3 이상이면 가능하나 유니티 6 이상을 권장합니다.

---

## 상세 사용법 (Usage Guide)

### DevLog
```csharp
using Potan.CoreUtils;

// 일반 로그
DevLog.Log("Hello World");
// 컨텍스트 로그 (로그 앞에 Object 이름 자동 추가)
DevLog.Log("Hello World", this);

// 색상 로그 (Color 구조체 지원)
DevLog.LogColor("Success!", Color.green, this);

// 경고 및 에러 (빌드 시 자동 제거)
DevLog.LogWarning("Potential issue detected.");
DevLog.LogError("Critical error!");
```

### EventBus
```csharp
using Potan.CoreUtils;

// 1. 이벤트 정의 (IEvent 인터페이스 상속, struct 권장)
public struct PlayerLevelUpEvent : IEvent { public int level; }

// 2. 구독 및 해제
void OnEnable() => EventBus.Subscribe<PlayerLevelUpEvent>(OnLevelUp);
void OnDisable() => EventBus.Unsubscribe<PlayerLevelUpEvent>(OnLevelUp);

// 3. 발행
EventBus.Publish(new PlayerLevelUpEvent { level = 10 });

// 4. 콜백 함수
void OnLevelUp(PlayerLevelUpEvent data) {
    DevLog.Log($"Level Up: {data.level}");
}
```

### Singletons
```csharp
using Potan.CoreUtils;

// 자동 생성 기능 비활성화 시 어트리뷰트 사용 (기본값은 AutoCreate = true)
[SingletonSettings(AutoCreate = false)]
public class GameManager : MonoSingleton<GameManager>
{
    public void StartGame() { ... }
}

// 접근
GameManager.Instance.StartGame();
```

---

## 중요: 상속 시 주의사항 (Overrides)

`MonoSingleton` 또는 `MonoSceneSingleton`을 상속받아 `Awake`나 `OnDestroy`를 직접 구현할 경우, **반드시 `base` 메서드를 호출**해야 합니다. 이를 누락하면 싱글톤 인스턴스 초기화나 해제가 정상적으로 이루어지지 않습니다.

```csharp
public class MyManager : MonoSingleton<MyManager>
{
    protected override void Awake()
    {
        // 1. 반드시 base.Awake()를 먼저 호출하세요.
        // 이 과정에서 인스턴스 등록 및 중복 인스턴스 파괴가 일어납니다.
        base.Awake(); 

        // 2. 이후에 커스텀 초기화 로직 작성
        InitializeMyManager();
    }

    protected override void OnDestroy()
    {
        // 1. 커스텀 정리 로직 작성
        Cleanup();

        // 2. 반드시 base.OnDestroy()를 호출하여 인스턴스 참조를 해제하세요.
        base.OnDestroy();
    }
}
```

---

## 📄 라이선스
본 프로젝트는 [MIT License](LICENSE)를 따릅니다.
