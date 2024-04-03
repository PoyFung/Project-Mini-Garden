using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class PotList : MonoBehaviour
{
    public List<Transform> list;
    public bool allSeeded = false;
    public bool allWatered = false;
    public bool allCrop = false;
    public static bool cropsGrowing = false;

    private bool listChange = false;

    private void Awake()
    {
        list = new List<Transform>();

        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);
            list.Add(child);
        }
    }

    private void Update()
    {
        if (listChange==true)
        {
            CheckPots();
            listChange = false;
        }
    }

    public void CheckPots()
    {
        float size = list.Count;
        float counterSeed = 0;
        float counterWater = 0;
        float counterCrop = 0;

        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);
            PotState potState = child.GetComponent<PotState>();

            if (potState.isPlanted == true)
            {
                counterSeed++;
            }

            if (potState.isWatered == true)
            {
                counterWater++;
            }

            if (potState.hasCrop == true)
            {
                counterCrop++;
            }
        }
        if (counterSeed == size)
        {
            allSeeded = true;
        }

        if (counterWater == size)
        {
            allWatered = true;
        }

        if (counterCrop == size)
        {
            allCrop = true;
        }
    }

    public void PotChange()
    {
        listChange = true;
    }
}
