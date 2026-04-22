using System.Reflection;
using UnityEngine;
using Potan.CoreUtils;

namespace Potan.CoreUtils
{
    /// <summary>
    /// MonoBehaviour 기반의 싱글톤입니다. DontDestroyOnLoad를 기본으로 사용합니다.
    /// </summary>
    public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static bool _quitting = false;

        // 리플렉션 결과를 저장할 캐시 (제네릭 타입 T마다 독립적으로 존재함)
        private static bool? _autoCreateConfig;

        // 어트리뷰트를 읽어오는 함수
        private static bool GetAutoCreateSetting()
        {
            if (_autoCreateConfig.HasValue) return _autoCreateConfig.Value;

            var attribute = typeof(T).GetCustomAttribute<SingletonSettingsAttribute>();
            _autoCreateConfig = attribute != null ? attribute.AutoCreate : true; // 어트리뷰트가 없으면 기본값 true

            return _autoCreateConfig.Value;
        }

        private static T _instance;

        public static T Instance
        {
            get
            {
                if (_quitting)
                {
                    DevLog.LogWarning($"[{typeof(T).Name}] 앱이 종료 중이므로 인스턴스를 반환하지 않습니다.");
                    return null;
                }

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
                    // 어트리뷰트 설정에 따라 자동 생성 분기
                    if (GetAutoCreateSetting())
                    {
                        var go = new GameObject($"[{typeof(T).Name}]", typeof(T));
                        _instance = go.GetComponent<T>();
                        DontDestroyOnLoad(go);
                    }
                    else
                    {
                        // 자동 생성이 꺼져있다면 null 반환 (또는 필요시 에러 로그)
                        DevLog.LogWarning($"[{typeof(T).Name}] 인스턴스가 존재하지 않으며, 자동 생성이 비활성화되어 있습니다.");
                        return null;
                    }
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
                if (transform.parent != null)
                {
                    transform.SetParent(null);
                }
                DontDestroyOnLoad(gameObject);
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