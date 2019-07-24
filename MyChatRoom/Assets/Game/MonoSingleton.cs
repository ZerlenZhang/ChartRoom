using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
namespace Game
{
    /// <summary>
    /// MonoBehavior单例泛型类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MonoSingleton<T> : MonoBehaviour
        where T : MonoSingleton<T>, new()
    {
        private static T instance;
        public static T Instance
        {
            get { return instance; }
        }
        protected virtual void Awake()
        {
            instance = this as T;
        }
    }
}
