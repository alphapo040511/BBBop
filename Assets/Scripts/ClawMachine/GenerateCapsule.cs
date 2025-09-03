using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateCapsule : MonoBehaviour
{
    public GameObject[] capsulePrefab;
    public Transform generateArea;
    public Vector3 areaSize = new Vector3 (6, 2, 6);

    [Range(0, 100)]
    public int generateCount = 10;

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
            
            if(randomPos.x <= - areaSize.x + 4f && randomPos.z >= areaSize.z - 4f)
            {
                randomPos.x = Random.Range(-areaSize.x + 4f, areaSize.x);
            }

            Vector3 spawnPos = generateArea.position + randomPos;

            GameObject prefab = capsulePrefab[Random.Range(0, capsulePrefab.Length)];

            Instantiate(prefab, spawnPos, Quaternion.identity);
        }
    }

    void OnDrawGizmos()
    {
        // 이전 행렬 저장
        Matrix4x4 oldMatrix = Gizmos.matrix;
        Gizmos.color = Color.blue;

        // 오브젝트의 로컬 기준으로 변환
        Gizmos.matrix = generateArea.localToWorldMatrix;

        // 로컬 기준 (0,0,0) 위치에 박스 그리기
        Gizmos.DrawWireCube(Vector3.zero, areaSize * 2);

        // 행렬 복구
        Gizmos.matrix = oldMatrix;
    }
}
