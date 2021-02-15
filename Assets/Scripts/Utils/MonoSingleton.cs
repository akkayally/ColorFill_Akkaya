using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
{
    private static T _instance;
    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<T>();
                if (_instance == null)
                {
                    _instance = new GameObject(name: "Instance of " + typeof(T)).AddComponent<T>();
                }
            }
            return _instance;
        }
    }

    public static bool IsInitialized
    {
        get { return _instance != null; }
    }
    private void Awake()
    {
        if (_instance != null && _instance != (T)this)
        {
            Debug.LogError("[Singleton] Trying to instantiate a second instance of a singleton class.");
            Destroy(this.gameObject);
            return;
        }
        else
        {
            _instance = (T)this;
        }
        DontDestroyOnLoad(gameObject);
        Init();
    }
    public virtual void Init()
    {

    }
    protected virtual void OnDestroy()
    {
        if (_instance == this)
        {
            _instance = null;
        }
    }
}
