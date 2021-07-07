using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    static object m_lock = new object();

    static T m_singleton = null;
    public static T singleton
    {
        get
        {
            lock (m_lock)
            {
                if (m_singleton == null)
                {
                    m_singleton = FindObjectOfType<T>();
                }

                if (m_singleton == null)
                {
                    GameObject go = new GameObject("[Singleton] " + typeof(T).ToString());
                    m_singleton = go.AddComponent<T>();
                }

                return m_singleton;
            }
        }
    }

    protected virtual void Awake()
    {
        // cache ref to existing singleton
        if (m_singleton == null)
        {
            m_singleton = FindObjectOfType<T>();
        }

        // set m_singleton = this component, if no other singleton of type T exists
        if (m_singleton == null)
        {
            this.gameObject.name = "[Singleton] " + typeof(T).ToString();
            m_singleton = this.gameObject.GetComponent<T>();
            DontDestroyOnLoad(this.gameObject);
        }

        // if this isn't the cached singleton, destroy it
        if (m_singleton != this)
        {
            Destroy(this.gameObject);
            return;
        }
    }
}
