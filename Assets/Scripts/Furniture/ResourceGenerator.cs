using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class ResourceGenerator : MonoBehaviour
{
    private FurnitureData furnitureData;


    private PlacedFurniture furniture;
    [SerializeField] private Vector2Int GenPosition;
    [SerializeField] private Vector2Int OutPosition;
    [SerializeField] private GameObject money;
    private GridManager gridManager;

    private float timer = 0;

    private void OnDestroy()
    {
        if(money != null)
        ObjectPool.Instance.Despawn("Money", money);
    }

    private void Start()
    {
        furniture = GetComponent<PlacedFurniture>();
        gridManager = FindObjectOfType<GridManager>();
        GenPosition = furniture.Start;
        OutPosition = furniture.Start;

        Vector2Int dir = furniture.Rotation switch
        {
            0 => new Vector2Int(0, 1),
            90 => new Vector2Int(1, 0),
            180 => new Vector2Int(0, -1),
            270 => new Vector2Int(-1, 0),
            _ => new Vector2Int(0, 0)
        };

        GenPosition += dir * (furniture.Size.y - 1);        // 출구 위치에 생성
        OutPosition += dir * furniture.Size.y;              // 컨배이어 밸트 위치
    }

    public void Initialized(FurnitureData buildingData)
    {
        this.furnitureData = buildingData;
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if(timer >= furnitureData.intervalTime)
        {
            timer = 0;              // 생성이 안되더라도 초기화


            if (money == null)
            {
                money = ObjectPool.Instance.Spawn("Money"
                    ,new Vector3(GenPosition.x + 0.5f, 0.5f, GenPosition.y + 0.5f)
                    , Quaternion.Euler(0, furniture.Rotation, 0));

                money.GetComponent<Money>().money = furnitureData.goldAmount;
            }

            if (money != null)
            {
                Conveyor conveyor = GetNextConveyor();
                if(conveyor != null)
                {
                    if(conveyor.Enter(money))
                    {
                        money = null;
                    }
                }
            }
        }
    }

    private Conveyor GetNextConveyor()
    {
        if (gridManager.GetBuildingAtPosition(OutPosition.x, OutPosition.y, out PlacedFurniture item))
        {
            Conveyor conveyor = item.GetComponent<Conveyor>();
            if (conveyor != null)
            {
                return conveyor;
            }
        }
        return null;
    }
}
