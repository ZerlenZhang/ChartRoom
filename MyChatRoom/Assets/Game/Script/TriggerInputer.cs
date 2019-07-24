using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Game.Script
{
    public class TriggerInputer : MonoBehaviour
    {

        public event Action<Collider2D> onTriggerEnterEvent; //如果TriggerEnter会调用这个event
        public event Action<Collider2D> onTriggerStayEvent; //如果TriggerStay会调用这个event
        public event Action<Collider2D> onTriggerExitEvent; //同理

        private void OnTriggerEnter2D(Collider2D col)
        {
            if (onTriggerEnterEvent != null)
                onTriggerEnterEvent(col);
        }


        private void OnTriggerStay2D(Collider2D col)
        {
            if (onTriggerStayEvent != null)
                onTriggerStayEvent(col);
        }


        private void OnTriggerExit2D(Collider2D col)
        {
            if (onTriggerExitEvent != null)
                onTriggerExitEvent(col);
        }


    }
}
