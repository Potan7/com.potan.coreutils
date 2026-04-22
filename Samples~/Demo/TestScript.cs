using UnityEngine;
using Potan.CoreUtils;

namespace Potan.CoreUtils.Tests
{
    public struct HealthChangedEvent : IEvent
    {
        public int currentHealth;
        public int maxHealth;
    }

    public class DataManager : Singleton<DataManager>
    {
        public string AppVersion = "1.0.0";
    }

    [SingletonSettings(AutoCreate = true)]
    public class GlobalManager : MonoSingleton<GlobalManager>
    {
        public void DoSomething() => DevLog.LogColor("[GlobalManager] 작동 중", Color.green);
    }

    public class SceneManager : MonoSceneSingleton<SceneManager>
    {
        public string SceneName = "MainScene";
    }

    public class TestScript : MonoBehaviour
    {
        private void Start()
        {
            RunDevLogTest();
            RunEventBusTest();
            RunSingletonTest();
        }

        private void RunDevLogTest()
        {
            DevLog.LogColor(">>> 1. DevLog 테스트 시작", Color.cyan);
            DevLog.Log("일반 로그");
            DevLog.LogWarning("경고 로그 (노란색)");
            DevLog.LogError("에러 로그 (빨간색)");
        }

        private void RunEventBusTest()
        {
            DevLog.LogColor(">>> 2. EventBus 테스트 시작", Color.cyan);
            EventBus.Subscribe<HealthChangedEvent>(OnHealthChanged);
            EventBus.Publish(new HealthChangedEvent { currentHealth = 50, maxHealth = 100 });
            EventBus.Unsubscribe<HealthChangedEvent>(OnHealthChanged);
        }

        private void OnHealthChanged(HealthChangedEvent data)
        {
            DevLog.LogColor($"[EventBus] 체력 변경 수신: {data.currentHealth}/{data.maxHealth}", Color.magenta);
        }

        private void RunSingletonTest()
        {
            DevLog.LogColor(">>> 3. Singleton 테스트 시작", Color.cyan);
            DevLog.Log($"[Singleton] 데이터 버전: {DataManager.Instance.AppVersion}");
            GlobalManager.Instance.DoSomething();
        }
    }
}
