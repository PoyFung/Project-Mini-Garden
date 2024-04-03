using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxState : MonoBehaviour
{
    public GameObject seedbox;
    public GameObject waterBox;
    public GameObject cropBox;

    public static float cropsCollected = 0;
    
    void Start()
    {
        seedbox.SetActive(false);
        waterBox.SetActive(false);
        cropBox.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (PotList.allSeeded == false || PotList.seedsExist == false)
        {
            seedbox.SetActive(true);
        }

        else if (PotList.allSeeded == true && PotList.allWatered == false)
        {
            seedbox.SetActive(false);
            waterBox.SetActive(true);
        }

        if (PotList.cropsGrowing == true)
        {
            cropBox.SetActive(true);
        }
    }
}
