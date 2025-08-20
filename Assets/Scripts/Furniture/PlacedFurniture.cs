using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlacedFurniture : MonoBehaviour
{
    [HideInInspector] public string id;
    [HideInInspector] public Vector2Int Start;              // ���ϴ� ����
    public int width;                                       // ��
    public int depth;                                       // ����

    public Vector2Int Size => new Vector2Int(width, depth);
    [HideInInspector] public int Rotation;                  // 0, 90, 180, 270
}

// ����� class
public class PlacedFurnitureData
{
    public string id;
    public Vector2Int start;
    public int rotation;

    public PlacedFurnitureData(string id, Vector2Int start, int rotation)
    {
        this.id = id;
        this.start = start;
        this.rotation = rotation;
    }
}