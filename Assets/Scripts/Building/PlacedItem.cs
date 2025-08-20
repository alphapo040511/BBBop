using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlacedItem : MonoBehaviour
{
    public Vector2Int Start;            // ÁÂÇÏ´Ü ±âÁØ
    public int width;                   // Æø
    public int depth;                   // ±íÀÌ

    public Vector2Int Size => new Vector2Int(width, depth);
    public int Rotation;       // 0, 90, 180, 270
}
