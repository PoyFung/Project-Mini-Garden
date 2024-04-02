using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class PotList : MonoBehaviour
{
    public List<Transform> list;
    public bool allFull = false;
    public bool allWatered = false;

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
        float counter = 0;
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);
            PotState potState = child.GetComponent<PotState>();

            if (potState.isPlanted == true)
            {
                counter++;
            }
        }
        if (counter == size)
        {
            allFull = true;
        }
    }

    public void PotChange()
    {
        listChange = true;
    }
}
