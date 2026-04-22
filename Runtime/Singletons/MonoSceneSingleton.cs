using UnityEngine;
using Potan.CoreUtils;

namespace Potan.CoreUtils
{
    /// <summary>
    /// MonoBehaviour 기반의 싱글톤입니다. DontDestroyOnLoad를 사용하지 않아 씬을 넘어가면 삭제됩니다. 자동으로 생성되지 않습니다.
    /// </summary>
    public abstract class MonoSceneSingleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static bool _quitting = false;
        private static T _instance;

        public static T Instance
        {
            get
            {
                if (_quitting) return null;

                if (_instance != null)
                {
                    return _instance;
                }
#if UNITY_6000_0_OR_NEWER
                _instance = FindAnyObjectByType<T>();
#else
                _instance = FindObjectOfType<T>();
#endif
                if (_instance == null)
                {
                    DevLog.LogError($"[{typeof(T).Name}] 인스턴스가 존재하지 않습니다. 씬에 해당 컴포넌트를 추가해주세요.");
                    return null;
                }

                return _instance;
            }
        }

        protected virtual void Awake()
        {
            if (_instance != null)
            {
                if (_instance != this)
                {
                    DevLog.LogWarning($"[{typeof(T).Name}] 인스턴스가 이미 존재합니다. 새로운 인스턴스를 파괴합니다.", this);
                    Destroy(gameObject);
                }
            }
            else
            {
                _instance = this as T;
            }
        }

        protected virtual void OnApplicationQuit()
        {
            _quitting = true;
        }

        protected virtual void OnDestroy()
        {
            if (_instance == this)
            {
                _instance = null;
            }
        }
    }
}