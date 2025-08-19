using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//매니저 클래스들의 기본 틀
public class SingletonMonoBehaviour<T> : MonoBehaviour where T : MonoBehaviour      // T는 MonoBehaviour를 상속한 클래스만 가능
{
    private static T _instance;
    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<T>();
                if (_instance == null)
                {
                    GameObject go = new GameObject(typeof(T).Name);         // T 클래스의 이름 문자열로 오브젝트 생성
                    _instance = go.AddComponent<T>();
                }
            }
            return _instance;
        }
    }

    protected virtual void Awake()
    {
        if (_instance == null)
        {
            _instance = this as T;                  // this(이 객체)를 T 형식으로 변환
            DontDestroyOnLoad(gameObject);
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }
}
