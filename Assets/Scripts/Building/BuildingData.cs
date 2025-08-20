using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Building Data", menuName = "Custom Asset/Building Data")]
public class BuildingData : ScriptableObject
{
    [Header("�ǹ� ����")]
    public PlacedItem buildingPrefab;

    public Vector2Int size
    {
        get
        {
            if (buildingPrefab != null)
            {
                return new Vector2Int(buildingPrefab.width, buildingPrefab.depth);
            }
            else
            {
                Debug.LogWarning("buildingPrefab �� �Ҵ���� �ʾҽ��ϴ�.");
                return new Vector2Int(0, 0);
            }
        }
    }

    [Header("�ڿ� ���� ����")]
    public bool canProduceResource;     // �ڿ� ���� ���� ����
    public int goldAmount;              // ��� ���귮
    public float intervalTime;          // ��� ���� �ֱ�
}
