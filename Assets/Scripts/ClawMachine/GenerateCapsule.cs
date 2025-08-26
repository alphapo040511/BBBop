using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateCapsule : MonoBehaviour
{
    public GameObject[] capsulePrefab;
    public Transform generateArea;
    public Vector3 areaSize = new Vector3 (6, 2, 6);

    private int generateCount = 10;

    private void Start()
    {
        if (capsulePrefab == null)
        {
            Debug.Log("캡슐 프리팹이 존재하지 않습니다!");
        }

        RandomGenerate();
    }

    void RandomGenerate()
    {
        for (int i = 0; i < generateCount; i++)
        {
            Vector3 randomPos = new Vector3(
                Random.Range(-areaSize.x, areaSize.x),
                Random.Range(-areaSize.y, areaSize.y),
                Random.Range(-areaSize.z, areaSize.z)
            );
            
            Vector3 spawnPos = generateArea.position + generateArea.rotation * randomPos;

            GameObject prefab = capsulePrefab[Random.Range(0, capsulePrefab.Length)];

            Instantiate(prefab, spawnPos, Quaternion.identity);
        }
    }
}
