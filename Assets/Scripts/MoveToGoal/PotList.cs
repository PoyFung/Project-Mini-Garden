using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PotList : MonoBehaviour
{
    public List<Transform> list;
    public bool allEmpty = true;
    private void Awake()
    {
        list = new List<Transform>();

        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);
            list.Add(child);
        }
    }

    void Update()
    {
        checkPots();
    }

    public void checkPots()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);
            PotState potState = child.GetComponent<PotState>();
            if (potState.isPlanted==false)
            {
                allEmpty = true;
            }

            else
            {
                allEmpty=false;
                return;
            }
        }
    }
}
