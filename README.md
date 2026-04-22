# Potan CoreUtils

Unity 프로젝트 초기 설정 시 필수적으로 필요한 유틸리티 모음입니다. 성능 최적화와 개발 편의성에 초점을 맞추어 설계되었습니다.

## ⚙️ 요구 사항 (Requirements)
- Unity 2022.3부터 가능하나 Unity 6 (6000.0) 이상 권장
- 의존성 패키지: 없음 (Zero Dependencies)

## 주요 기능

###  EventBus (Zero-Allocation)
- **성능 최적화**: `struct`와 정적 제네릭을 사용하여 런타임 Dictionary 조회가 없으며 가비지 컬렉션(GC) 할당이 발생하지 않습니다.
- **안전성**: 역순 순회를 통해 이벤트 처리 중 구독 해제가 일어나도 안전합니다.

### DevLog (Conditional Logging)
- **빌드 최적화**: `[Conditional]` 어트리뷰트를 사용하여 배포용 빌드에서 로그 코드가 자동으로 제거됩니다.
- **편의성**: `UnityEngine.Color` 지원 및 컨텍스트 오브젝트 연결을 통해 에디터에서 로그 발생 지점을 쉽게 찾을 수 있습니다.

### Singletons
- **MonoSingleton**: 자동 생성 지원 및 `DontDestroyOnLoad`가 적용된 싱글톤입니다. 앱 종료 시 발생할 수 있는 오브젝트 생성 버그(Ghost Singleton)가 방지되어 있습니다.
- **MonoSceneSingleton**: 씬 라이프사이클에 종속적인 MonoBehaviour 싱글톤입니다.
- **Singleton<T>**: 일반 C# 클래스를 위한 Thread-safe 싱글톤입니다.

## 설치 방법 (UPM)

1. Unity Editor에서 `Window > Package Manager`를 엽니다.
2. `+` 버튼을 누르고 `Add package from git URL...`을 선택합니다.
3. 아래 URL을 입력합니다:
   ```
   https://github.com/Potan7/com.potan.coreutils.git
   ```

## 사용 예시

### EventBus
```csharp
// struct를 사용해서 이벤트 정의
public struct PlayerLevelUpEvent : IEvent { public int level; }

public class PlayerUI : MonoBehaviour
{
    private void OnEnable()
    {
        // 이벤트 구독
        EventBus.Subscribe<PlayerLevelUpEvent>(OnLevelUp);
    }

    private void OnDisable()
    {
        // 이벤트 구독 해제
        EventBus.Unsubscribe<PlayerLevelUpEvent>(OnLevelUp);
    }

    private void OnLevelUp(PlayerLevelUpEvent e)
    {
        DevLog.Log($"Level up to: {e.level}");
    }
}

// 발행
EventBus.Publish(new PlayerLevelUpEvent { level = 10 });
```

### DevLog
```csharp
// 색상 로그 (Color 구조체 지원)
DevLog.LogColor("Success!", Color.green, this);

// 경고 및 에러 (빌드 시 자동 제거)
DevLog.LogWarning("Potential issue detected.");
```

### MonoSingleton
```csharp
[SingletonSettings(AutoCreate = true)]  // Instance가 null 일 경우 자동으로 생성합니다. (기본값 true)
public class GameManager : MonoSingleton<GameManager>
{
    public void StartGame() { ... }
}

// 접근
GameManager.Instance.StartGame();
```

## 라이선스
이 프로젝트는 [MIT License](LICENSE)를 따릅니다.
