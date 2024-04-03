using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.VisualScripting;
using UnityEngine;

public class PotState : MonoBehaviour
{
    public bool isPlanted = false;
    public bool isWatered = false;
    public bool isGrowing = false;
    public bool hasCrop = false;

    public GameObject finalPotato;

    public Stopwatch timer = new Stopwatch();

    private void Update()
    {
        if (isPlanted && isWatered || isGrowing==true)
        {
            timer.Start();
            Transform seed = transform.Find("Seed(Clone)");
            Transform water = transform.Find("WaterDrop(Clone)");

            if (seed != null)
                Destroy(seed.gameObject);

            if (water != null)
                Destroy(water.gameObject);

            isGrowing = true;
            Transform child1 = transform.Find("potato1");
            Transform child2 = transform.Find("potato2");
            Transform child3 = transform.Find("potato3");
            Transform child4 = transform.Find("potato4");
            
            GameObject p1 = child1.gameObject;
            GameObject p2 = child2.gameObject;
            GameObject p3 = child3.gameObject;
            GameObject p4 = child4.gameObject;


            p1.SetActive(true);

            if (timer.Elapsed.TotalSeconds>= 5)
            {
                p1.SetActive(false);
                p2.SetActive(true);
            }

            if (timer.Elapsed.TotalSeconds >= 10)
            {
                p2.SetActive(false);
                p3.SetActive(true);
            }

            if (timer.Elapsed.TotalSeconds >= 15)
            {
                p3.SetActive(false);
                p4.SetActive(true);
                isGrowing = false;
                isPlanted = false;
                isWatered = false;
                hasCrop = true;
                finalPotato = p4;
                PotList.PotChange();
                timer.Reset();
            }
            isPlanted=false;
            isWatered=false;

        }
    }
}
