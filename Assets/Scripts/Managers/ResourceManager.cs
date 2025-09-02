using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using TMPro;
using UnityEngine;

public class ResourceManager : SingletonMonoBehaviour<ResourceManager>
{
    public TextMeshProUGUI goldUI;
    public BigInteger gold { get; private set; }

    private void Start()
    {
        InitializedResources();
    }

    public void InitializedResources()
    {
        SaveManager.Instance.LoadData();
        gold = BigInteger.Parse(SaveManager.Instance._saveData.gold);


        if (goldUI != null)
        {
            goldUI.text = gold.ToString("N0") + 'G';
        }
    }

    public void GetGold(int amount)
    {
        if(amount < 0) return;
        gold += amount;

        if(goldUI != null)
        {
            goldUI.text = gold.ToString("N0") + 'G';
        }
    }

    public bool UseGold(int amount)
    {
        if(gold - amount < 0) return false; 

        gold -= amount;

        if (goldUI != null)
        {
            goldUI.text = gold.ToString("N0") + 'G';
        }

        return true;
    }
}
