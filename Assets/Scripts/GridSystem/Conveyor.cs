using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Conveyor : MonoBehaviour
{
    private PlacedFurniture furniture;

    private Queue<GameObject> moneyObjs = new Queue<GameObject>();
    [SerializeField]private GameObject target;
    private GridManager gridManager;

    private void OnDestroy()
    {
        if (target != null)
        ObjectPool.Instance.Despawn("Money", target);
    }

    // Start is called before the first frame update
    void Start()
    {
        furniture = GetComponent<PlacedFurniture>();
        gridManager = FindObjectOfType<GridManager>();
    }

    private void Update()
    {
        if(target != null)
        {
            Move();
        }
        //else if(moneyObjs.Count > 0)
        //{
        //    target = moneyObjs.Dequeue();
        //}
    }

    private void Move()
    {
        // 목표 위치 (컨베이어 위치 + y 오프셋)
        Vector3 targetPos = transform.position + Vector3.up * 0.5f;

        target.transform.position = Vector3.MoveTowards(
            target.transform.position,
            targetPos,
            Time.deltaTime
        );

        // 회전 (컨베이어가 바라보는 방향 쪽으로 맞추기)
        //Quaternion targetRot = transform.rotation;
        //target.transform.localRotation = Quaternion.Slerp(
        //    target.transform.rotation,
        //    targetRot,
        //    Time.deltaTime * 5f
        //);

        // 다 왔으면 다음 컨베이어로 아이템 넘기기
        if (Vector3.Distance(target.transform.position, targetPos) < 0.01f)
        {
            Conveyor nextConveyor = GetNextConveyor();

            if (nextConveyor != null)
            {
                if (nextConveyor.Enter(target))
                {
                    target = null;
                }
            }
        }
    }

    private Conveyor GetNextConveyor()
    {
        if(gridManager.GetBuildingAtPosition(NextConveyorPos().x, NextConveyorPos().y, out PlacedFurniture item))
        {
            Conveyor conveyor = item.GetComponent<Conveyor>();
            if (conveyor != null)
            {
                return conveyor;
            }
        }

        return null;
    }

    private Vector2Int NextConveyorPos()
    {
        if (furniture == null) return new Vector2Int(-1, -1);       // 값을 찾을 수 없도록 음수 자리로 return

        Vector2Int nextPos;
        nextPos = furniture.Start;

        Vector2Int dir = furniture.Rotation switch
        {
            0 => new Vector2Int(0, -1),
            90 => new Vector2Int(-1, 0),
            180 => new Vector2Int(0, 1),
            270 => new Vector2Int(1, 0),
            _ => new Vector2Int(0, 0)
        };

        if(dir == new Vector2Int(0, 0)) return new Vector2Int(-1, -1);       // 값을 찾을 수 없도록 음수 자리로 return

        return nextPos + dir;
    }

    public virtual bool Enter(GameObject money)
    {
        if (target == null)
        {
            target = money;
            return true;
        }
        else
        {
            return false;
        }
    }
}
