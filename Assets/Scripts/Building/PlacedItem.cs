using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]

public class PlacedItem
{
    public GameObject Item;
    public Vector2Int Start;   // 좌하단 기준
    public Vector2Int Size;    // (width, height)
    public int Rotation;       // 0, 90, 180, 270

    public IEnumerable<Vector2Int> GetCoveredTiles()
    {
        int w = Size.x;
        int h = Size.y;

        for (int x = 0; x < w; x++)
        {
            for (int y = 0; y < h; y++)
            {
                Vector2Int offset = new Vector2Int(x, y);

                // Rotation 처리
                switch (Rotation % 360)
                {
                    case 0: // 그대로
                        break;

                    case 90:
                        offset = new Vector2Int(y, -x);
                        break;

                    case 180:
                        offset = new Vector2Int(-x, -y);
                        break;

                    case 270:
                        offset = new Vector2Int(-y, x);
                        break;
                }

                yield return Start + offset;
            }
        }
    }
}
