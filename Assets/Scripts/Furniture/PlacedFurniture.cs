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