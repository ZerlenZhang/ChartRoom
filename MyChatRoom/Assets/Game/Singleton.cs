using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    /// <summary>
    /// 单例泛型类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Singleton<T>
    where T : class, new()
    {
        private static T instance = null;
        public static T Instance
        {
            get
            {
                if (instance == null)
                    instance = new T();
                return instance;
            }
        }
    }
}
