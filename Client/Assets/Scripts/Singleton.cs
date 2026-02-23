using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour                     
{
    private static T instance = null;
    public static T Instance                                                                                           
    {
        get
        {
            if (!instance)
            {
                instance = (T)FindAnyObjectByType(typeof(T));
                if (!instance)
                {
                    GameObject obj = new GameObject(typeof(T).Name, typeof(T));
                    instance = obj.GetComponent<T>();
                }
            }
            return instance;
        }
    }
}

public class SingletonObject<T> : MonoBehaviour where T : MonoBehaviour                     
{
    private static T instance = null;
    public static T Instance                                                                                           
    {
        get
        {
            if (!instance)
            {
                instance = (T)FindAnyObjectByType(typeof(T));
                if (!instance)
                {
                    GameObject obj = new GameObject(typeof(T).Name, typeof(T));
                    instance = obj.GetComponent<T>();
                }
            }
            return instance;
        }
    }

    public virtual void Awake()
    {
        if (Instance != this) Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
    }
}