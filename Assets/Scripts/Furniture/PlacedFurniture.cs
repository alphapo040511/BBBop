using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlacedFurniture : MonoBehaviour
{
    [HideInInspector] public string id;
    [HideInInspector] public Vector2Int Start;              // ÁÂÇÏ´Ü ±âÁØ
    public int width;                                       // Æø
    public int depth;                                       // ±íÀÌ

    public Vector2Int Size => new Vector2Int(width, depth);
    [HideInInspector] public int Rotation;                  // 0, 90, 180, 270
}

// ÀúÀå¿ë class
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