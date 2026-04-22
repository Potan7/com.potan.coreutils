using System;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Potan.CoreUtils
{
    public static class DevLog
    {
        #region 기본 로그 (Log)

        [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")]
        public static void Log(object message)
        {
            Debug.Log(message);
        }

        [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")]
        public static void Log(object message, UnityEngine.Object context)
        {
            string contextName = context != null ? context.name : "Null Object";
            Debug.Log($"<b>[{contextName}]</b> {message}", context);
        }

        #endregion

        #region 경고 및 에러 (Warning & Error)

        [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")]
        public static void LogWarning(object message, UnityEngine.Object context = null)
        {
            if (context != null)
            {
                Debug.LogWarning($"<b>[{context.name}]</b> {message}", context);
            }
            else
            {
                Debug.LogWarning(message);
            }
        }

        [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")]
        public static void LogError(object message, UnityEngine.Object context = null)
        {
            if (context != null)
            {
                Debug.LogError($"<b>[{context.name}]</b> {message}", context);
            }
            else
            {
                Debug.LogError(message);
            }
        }

        [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")]
        public static void LogException(Exception exception, UnityEngine.Object context = null)
        {
            Debug.LogException(exception, context);
        }

        #endregion

        #region Color Log
        [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")]
        public static void LogColor(object message, Color color)
        {
            string hex = ColorUtility.ToHtmlStringRGB(color);
            Debug.Log($"<color=#{hex}>{message}</color>");
        }

        [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")]
        public static void LogColor(object message, Color color, UnityEngine.Object context)
        {
            string hex = ColorUtility.ToHtmlStringRGB(color);
            string contextName = context != null ? context.name : "Null Object";
            Debug.Log($"<color=#{hex}><b>[{contextName}]</b> {message}</color>", context);
        }
        #endregion
    }
}