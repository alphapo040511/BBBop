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
        // ��ǥ ��ġ (�����̾� ��ġ + y ������)
        Vector3 targetPos = transform.position + Vector3.up * 0.5f;

        target.transform.position = Vector3.MoveTowards(
            target.transform.position,
            targetPos,
            Time.deltaTime
        );

        // ȸ�� (�����̾ �ٶ󺸴� ���� ������ ���߱�)
        //Quaternion targetRot = transform.rotation;
        //target.transform.localRotation = Quaternion.Slerp(
        //    target.transform.rotation,
        //    targetRot,
        //    Time.deltaTime * 5f
        //);

        // �� ������ ���� �����̾�� ������ �ѱ��
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
        if (furniture == null) return new Vector2Int(-1, -1);       // ���� ã�� �� ������ ���� �ڸ��� return

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

        if(dir == new Vector2Int(0, 0)) return new Vector2Int(-1, -1);       // ���� ã�� �� ������ ���� �ڸ��� return

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
