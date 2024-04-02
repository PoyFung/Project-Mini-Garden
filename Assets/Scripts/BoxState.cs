using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxState : MonoBehaviour
{
    public PotList potList;
    public GameObject seedbox;
    public GameObject waterBox;
    
    void Start()
    {
        seedbox.SetActive(false);
        waterBox.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (potList.allFull == false)
        {
            seedbox.SetActive(true);
        }

        else if (potList.allFull == true && potList.allWatered == false)
        {
            seedbox.SetActive(false);
            waterBox.SetActive(true);
        }
    }
}
