using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Helper
{
    public class UnitySingleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T _instance = null;

        public static T Instance
        {
            get
            {
                if (ReferenceEquals(_instance, null))
                {
                    _instance = GameObject.FindObjectOfType<T>();
                    if (ReferenceEquals(_instance, null))
                    {
                        var t = typeof(T);
                        GameObject go = new GameObject(t.FullName, t);
                    }
                }
                return _instance;
            }
        }

        protected virtual void Awake()
        {
            if (_instance == null)
            {
                _instance = this as T;
                DontDestroyOnLoad(this.gameObject);
            }
            else
            {
                //If a Singleton already exists and you find
                //another reference in scene, destroy it!
                //			if (this != _instance)
                Destroy(this.gameObject);
                Debug.LogWarning("[Singelton]Destroying: " + this.gameObject.name + " " + "New InstanceID :" + this.GetInstanceID() + " Old :" + _instance.GetInstanceID());
            }
        }

        void OnApplicationQuit()
        {
            _instance = null;
        }
    }
}