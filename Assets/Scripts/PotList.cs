using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class PotList : MonoBehaviour
{
    public static List<Transform> list;
    public static bool seedsExist = false;
    public static bool emptyExist = false;

    public static bool allSeeded = false;
    public static bool allWatered = false;
    public static bool allCrop = false;
    public static bool cropsGrowing = false;

    private static bool listChange = false;

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
                seedsExist = true;
            }

            if (potState.isWatered == true)
            {
                counterWater++;
            }

            if (potState.hasCrop == true)
            {
                counterCrop++;
            }

            if (counterSeed == 0 && counterWater == 0)
            {
                emptyExist = true;
            }
        }
        if (counterSeed == size)
        {
            allSeeded = true;
            emptyExist = false;
        }

        else if (counterSeed == 0)
        {
            seedsExist=false;
        }

        if (counterWater == size)
        {
            allWatered = true;
            emptyExist = false;
        }

        if (counterCrop == size)
        {
            allCrop = true;
            emptyExist = false;
        }
    }

    public static void PotChange()
    {
        listChange = true;
    }
}
