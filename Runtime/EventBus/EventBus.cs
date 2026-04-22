
using System;
using System.Collections.Generic;

namespace Potan.CoreUtils
{
    /// <summary>
    /// 모든 이벤트 구조체가 상속 받아야하는 인터페이스입니다.
    /// 구조체(struct)를 사용하여 매 이벤트 발생 시 가비지 컬렉션(GC) 할당을 방지합니다.
    /// </summary>
    public interface IEvent { }

    /// <summary>
    /// 특정 이벤트 타입(T) 전용 이벤트 버스입니다.
    /// 타입별로 독립적인 정적(Static) 리스트를 생성하기 때문에 런타임에 Dictionary를 조회하는 비용 없이 O(1) 성능으로 이벤트를 처리할 수 있습니다.
    /// </summary>
    /// <typeparam name="T">IEvent 인터페이스를 구현하는 이벤트 구조체(struct)</typeparam>
    public static class EventBus<T> where T : struct, IEvent
    {
        // 이벤트 구독 콜백을 보관하는 리스트
        // 잦은 내부 배열 재할당(Re-allocation)을 피하기 위해 리스트의 초기 가용 용량(Capacity)을 64로 넉넉하게 설정합니다.
        private static readonly List<Action<T>> _listeners = new List<Action<T>>(64);

        /// <summary>
        /// 특정 이벤트에 대한 리스너를 구독(추가)합니다.
        /// 동일한 콜백이 중복으로 등록되지 않도록 사전에 검사합니다.
        /// </summary>
        /// <param name="listener">해당 타입의 이벤트가 발행될 때 호출될 액션(콜백)</param>
        public static void Subscribe(Action<T> listener)
        {
            // Contains는 O(N) 비용이 들지만 리스너 수가 적을 때는 빠르며, 메모리 파편화를 줄이는 데 유리합니다.
            if (!_listeners.Contains(listener))
            {
                _listeners.Add(listener);
            }
        }

        /// <summary>
        /// 특정 이벤트에 대한 리스너의 구독을 해제합니다.
        /// </summary>
        /// <param name="listener">이벤트 목록에서 제거할 콜백 함수</param>
        public static void Unsubscribe(Action<T> listener)
        {
            _listeners.Remove(listener);
        }

        /// <summary>
        /// 구독 중인 모든 리스너에게 이벤트를 발행(전파)합니다.
        /// 역순으로 순회하여 콜백 내부에서 Unsubscribe가 일어나도 안전하며, 
        /// Publish 도중 Subscribe로 추가된 리스너는 현재 발행 사이클에서는 호출되지 않습니다.
        /// </summary>
        /// <param name="eventData">리스너에 전달 될 실제 이벤트 데이터 객체(구조체)</param>
        public static void Publish(ref T eventData)
        {
            // 이벤트 전송의 안정성을 위해 역순 반복
            // 콜백 안에서 _listeners 요소가 제거되더라도 현재 앞쪽 인덱스들에 영향을 주지 않으므로 안전합니다.
            for (int i = _listeners.Count - 1; i >= 0; i--)
            {
                _listeners[i].Invoke(eventData);
            }
        }

        /// <summary>
        /// 값 복사 방지를 적용하지 않은 Publish 함수입니다.
        /// </summary>
        public static void Publish(T eventData)
        {
            for (int i = _listeners.Count - 1; i >= 0; i--)
            {
                _listeners[i].Invoke(eventData);
            }
        }
    }

    /// <summary>
    /// 전역 이벤트 시스템의 접근성을 높여주는 헬퍼(Facade) 클래스입니다.
    /// 클래스명 명시만으로 편리하게 이벤트를 관리할 수 있게 해줍니다.
    /// </summary>
    public static class EventBus
    {
        /// <summary>
        /// 제공된 콜백을 특정 이벤트(T)에 구독하도록 버스에 등록합니다.
        /// </summary>
        public static void Subscribe<T>(Action<T> listener) where T : struct, IEvent
            => EventBus<T>.Subscribe(listener);

        /// <summary>
        /// 구독 중이던 특정 이벤트(T)의 콜백을 버스에서 해제합니다.
        /// </summary>
        public static void Unsubscribe<T>(Action<T> listener) where T : struct, IEvent
            => EventBus<T>.Unsubscribe(listener);

        /// <summary>
        /// 구독 중인 전체 리스너에게 이벤트를 발생시키고 매개변수를 전달합니다.
        /// </summary>
        public static void Publish<T>(ref T eventData) where T : struct, IEvent
            => EventBus<T>.Publish(ref eventData);

        /// <summary>
        /// 구독 중인 전체 리스너에게 이벤트를 발생시키고 매개변수를 전달합니다.
        /// </summary>
        public static void Publish<T>(T eventData) where T : struct, IEvent
            => EventBus<T>.Publish(eventData);
    }
}