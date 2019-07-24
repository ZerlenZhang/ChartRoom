using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.Script
{
    public class UIInputer : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerUpHandler,
        IPointerDownHandler, IPointerClickHandler
    {
        public event Action<PointerEventData> eventOnPointerEnter;
        public event Action<PointerEventData> eventOnPointerExit;
        public event Action<PointerEventData> eventOnPointerUp;
        public event Action<PointerEventData> eventOnPointerDown;
        public event Action<PointerEventData> eventOnPointerClick;


        public void OnPointerEnter(PointerEventData eventData)
        {
            if (eventOnPointerEnter == null) return;
            eventOnPointerEnter(eventData);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (eventOnPointerExit == null) return;
            eventOnPointerExit(eventData);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (eventOnPointerUp == null) return;
            eventOnPointerUp(eventData);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (eventOnPointerDown == null) return;
            eventOnPointerDown(eventData);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventOnPointerClick == null) return;
            eventOnPointerClick(eventData);
        }
    }
}
