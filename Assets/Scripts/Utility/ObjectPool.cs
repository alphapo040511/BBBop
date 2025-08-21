using System.Collections;
using System.Collections.Generic;
using UnityEditor.Search;
using UnityEngine;
using static UnityEditor.PlayerSettings;
using static UnityEngine.Rendering.DebugUI.Table;

[System.Serializable]
public class PoolData
{
    public string id;
    public GameObject Prefab;
    public int InitialCount = 10;       // 초기 생성 개수

}

public class ObjectPool : SingletonMonoBehaviour<ObjectPool>
{
    protected override void Awake()
    {
        if (_instance == null)
        {
            _instance = this;                  // this(이 객체)를 T 형식으로 변환
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }

    public List<PoolData> poolDatas = new List<PoolData>();
    private Dictionary<string, Queue<GameObject>> objectPools = new Dictionary<string, Queue<GameObject>>();

    private void Start()
    {
        PoolInitialize();
    }

    private void PoolInitialize()
    {
        foreach (var data in poolDatas)
        {
            var queue = new Queue<GameObject>();
            for (int i = 0; i < data.InitialCount; i++)
            {
                var go = Instantiate(data.Prefab, transform);
                go.SetActive(false);
                queue.Enqueue(go);
            }

            if (objectPools.ContainsKey(data.id))
            {
                Debug.LogWarning($"ID:{data.id}의 Pool이 이미 존재하고 있습니다.");
            }

            objectPools[data.id] = queue;
        }
    }

    public GameObject Spawn(string id, Vector3 position, Quaternion ratote)
    {
        if (!objectPools.ContainsKey(id))
        {
            Debug.LogWarning($"Pool에 등록되지 않는 ID입니다. (ID:{id})");
            return null;
        }

        var queue = objectPools[id];

        GameObject go;

        if (queue.Count > 0)
        {
            go = queue.Dequeue();
        }
        else
        {
            // 혹시 부족할 경우 추가 생성
            Debug.LogWarning($"ID:{id} 오브젝트의 Pool 초기 값 확장 필요");
            var data = poolDatas.Find(d => d.id == id);
            if (data == null) return null;
            go = Instantiate(data.Prefab, transform);
        }

        go.transform.SetPositionAndRotation(position, ratote);
        go.SetActive(true);
        return go;
    }

    public void Despawn(string id, GameObject obj)
    {
        obj.SetActive(false);
        if(objectPools.ContainsKey(id))
        {
            objectPools[id].Enqueue(obj);
        }
        else
        {
            Destroy(obj);
        }
    }
}
