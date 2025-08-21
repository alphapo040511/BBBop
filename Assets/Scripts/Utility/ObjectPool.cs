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
    public int InitialCount = 10;       // �ʱ� ���� ����

}

public class ObjectPool : SingletonMonoBehaviour<ObjectPool>
{
    protected override void Awake()
    {
        if (_instance == null)
        {
            _instance = this;                  // this(�� ��ü)�� T �������� ��ȯ
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
                Debug.LogWarning($"ID:{data.id}�� Pool�� �̹� �����ϰ� �ֽ��ϴ�.");
            }

            objectPools[data.id] = queue;
        }
    }

    public GameObject Spawn(string id, Vector3 position, Quaternion ratote)
    {
        if (!objectPools.ContainsKey(id))
        {
            Debug.LogWarning($"Pool�� ��ϵ��� �ʴ� ID�Դϴ�. (ID:{id})");
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
            // Ȥ�� ������ ��� �߰� ����
            Debug.LogWarning($"ID:{id} ������Ʈ�� Pool �ʱ� �� Ȯ�� �ʿ�");
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
