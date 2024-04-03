using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxState : MonoBehaviour
{
    public PotList potList;
    public GameObject seedbox;
    public GameObject waterBox;
    public GameObject cropBox;
    
    void Start()
    {
        seedbox.SetActive(false);
        waterBox.SetActive(false);
        cropBox.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (potList.allSeeded == false)
        {
            seedbox.SetActive(true);
        }

        else if (potList.allSeeded == true && potList.allWatered == false)
        {
            seedbox.SetActive(false);
            waterBox.SetActive(true);
        }

        else if (potList.allSeeded == true && potList.allWatered == true)
        {
            seedbox.SetActive(false);
            waterBox.SetActive(false);
            cropBox.SetActive(true);
        }
    }
}