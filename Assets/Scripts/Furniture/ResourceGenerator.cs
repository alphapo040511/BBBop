using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceGenerator : MonoBehaviour
{
    private FurnitureData furnitureData;

    private float timer = 0;

    public void Initialized(FurnitureData buildingData)
    {
        this.furnitureData = buildingData;
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if(timer >= furnitureData.intervalTime)
        {
            timer = 0;

            // °ñµå »ý¼º ·ÎÁ÷
            Debug.Log($"{furnitureData.goldAmount}G È¹µæ");
        }
    }
}
