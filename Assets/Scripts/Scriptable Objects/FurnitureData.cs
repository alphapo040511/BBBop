using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Furniture Data", menuName = "Custom Asset/Furniture Data")]
public class FurnitureData : ScriptableObject
{
    [Header("�⺻ ����")]
    public string id;
    public PlacedFurniture buildingPrefab;
    public Sprite icon;

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
    public int goldAmount = 1;              // ��� ���귮
    public float intervalTime = 1;          // ��� ���� �ֱ�
}
