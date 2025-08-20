using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceGenerator : MonoBehaviour
{
    private BuildingData buildingData;

    private float timer = 0;

    public void Initialized(BuildingData buildingData)
    {
        this.buildingData = buildingData;
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if(timer >= buildingData.intervalTime)
        {
            timer = 0;

            // °ñµå »ý¼º ·ÎÁ÷
            Debug.Log($"{buildingData.goldAmount}G È¹µæ");
        }
    }
}
