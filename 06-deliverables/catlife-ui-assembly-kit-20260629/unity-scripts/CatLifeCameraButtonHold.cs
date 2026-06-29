using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

namespace CatLife.UI
{
    public sealed class CatLifeCameraButtonHold : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
    {
        [SerializeField] private UnityEvent onHoldStart;
        [SerializeField] private UnityEvent onHoldEnd;

        public void OnPointerDown(PointerEventData eventData)
        {
            onHoldStart?.Invoke();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            onHoldEnd?.Invoke();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            onHoldEnd?.Invoke();
        }
    }
}
