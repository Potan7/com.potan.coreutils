using System;

namespace Potan.CoreUtils
{
    [AttributeUsage(AttributeTargets.Class, Inherited = true)]
    public class SingletonSettingsAttribute : Attribute
    {
        // 자동 생성 여부 (기본값 true)
        public bool AutoCreate { get; set; } = true;
    }
}